using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Sample.Models;
using WireCardNet.Processing;
using WireCardNet.Processing.Transactions;
using WireCardNet.QPay;
using System.Diagnostics;
using WireCardNet;

namespace Sample.Controllers
{

	public class HomeController : Controller
	{

		public HomeController()
		{
			WireCardNet.WireCard.SetupDemoAccount();
		}

		private string GetUrl(string path)
		{
			return string.Format("{0}{1}", "https://213.23.209.110", path);
		}

		public ActionResult Index()
		{
			var model = OrderService.GetQuery().ToList();

			return this.View(model);
		}

		public ActionResult Submit()
		{
			var order = OrderService.Create();

			OrderService.AddOrUpdate(order);

			var checkout = new WireCardNet.QPay.Checkout
			{
				Amount = order.Amount,
				Currency = "EUR",
				Language = WireCardNet.QPay.Language.German,
				SuccessURL = GetUrl(Url.Action("Success")),
				CancelURL = GetUrl(Url.Action("Cancel")),
				FailureURL = GetUrl(Url.Action("Failure")),
				ServiceURL = GetUrl(Url.Action("Terms")),
				ConfirmURL = GetUrl(Url.Action("Confirm")),
				ImageURL = GetUrl("/sites/website/images/common/logo.png"),
				DisplayText = "Thank you for your order.",
				OrderDescription = "Order description",
				// tested with following Payment types: PaymentType.CCard, PaymentType.PayPal and PaymentType.Sofortueberweisung
				// test credit card: 9500000000000001 expiration date: date in the future CVC: 3 random numbers
				PaymentType = PaymentType.CCard,
			};

			checkout.SetCustomParameter("orderId", order.Id.ToString());

			return View(checkout);
		}

		public ActionResult Confirm()
		{
			var response = CheckoutResponse.FromRequest(this.Request, ProcessSuccessResponse, ProcessFailureResponse, ProcessCancelResponse);

			return this.Json(new { }, JsonRequestBehavior.AllowGet);
		}

		private static void ProcessSuccessResponse(CheckoutSuccessResponse successResponse)
		{
			Guid orderId;
			if (Guid.TryParse(successResponse.CustomParameters["orderId"], out orderId))
			{
				var order = OrderService.GetQuery().FirstOrDefault(c => c.Id == orderId);
				if (order != null)
				{
					order.Transactions.Add(successResponse);

					// paypal and sofort überweisung payments dont need to be preathorized they are instantly captured
					if (successResponse.PaymentType != PaymentType.CCard)
					{
						order.State = OrderState.Captured;
					}
					else
					{
						order.State = OrderState.Preauthorized;
					}
					OrderService.AddOrUpdate(order);
				}
			}
			else
			{
				// log error 
			}
		}

		public ActionResult Success()
		{
			return this.RedirectPermanent("Index");
		}

		private static void ProcessFailureResponse(CheckoutFailureResponse failureResponse)
		{
			// log failure
		}

		public ActionResult Failure()
		{
			return this.View();
		}

		private static void ProcessCancelResponse(CheckoutCancelResponse cancelResponse)
		{
			// log cancel
		}

		public ActionResult Cancel()
		{
			return this.RedirectPermanent("Index");
		}

		public ActionResult Terms()
		{
			return this.View();
		}

		public ActionResult ClearOrders()
		{
			OrderService.ClearAll();

			return this.RedirectToAction("Index");
		}

		[HttpPost]
		public ActionResult Capture(Guid orderId)
		{
			var order = OrderService.GetQuery().FirstOrDefault(c => c.Id == orderId);

			if (order != null)
			{
				var trans = order.Transactions.FirstOrDefault();

				if (trans != null && PaymentType.CCard == trans.PaymentType)
				{
					var transaction = new CCTransaction
						{
							GuWID = trans.GatewayReferenceNumber,
							Amount = (double)trans.Amount,
							TransactionId = trans.OrderNumber,
							Mode = TransactionMode.Live
						};

					var capture = new WireCardNet.Processing.Functions.FncCCCapture
						{
							FunctionId = "FN_Capture1"
						};
					capture.AddTransaction(transaction);

					var job = new WireCardNet.Processing.Job();
					job.AddFunction(capture);


					job.JobId = "JOB_Capture1";
					job.BusinessCaseSignature = trans.GatewayContractNumber; // WireCardNet.WireCard.WireCardUsername;

					var processing = new WireCardNet.Processing.ProcessingRequest();
					processing.AddJob(job);

					//processing.Send();
					var response = processing.GetResponse();

					var status = response.FindStatus(job.JobId, capture.FunctionId, transaction.TransactionId);

					Debug.WriteLine("#################################");
					Debug.WriteLine("Capture result: " + status.Result);
					if (status.Error != null)
					{
						Debug.WriteLine("Advice: " + status.Error.Advice);
						Debug.WriteLine("Message: " + status.Error.Message);
					}
					else
					{
						trans.ResponseGuWID = status.GuWID;
						order.State = OrderState.Captured;
					}
					Debug.WriteLine("#################################");
				}
			}
			return this.RedirectToAction("Index");
		}

		[HttpPost]
		public ActionResult Reversal(Guid orderId)
		{
			var order = OrderService.GetQuery().FirstOrDefault(c => c.Id == orderId);

			if (order != null)
			{
				var trans = order.Transactions.FirstOrDefault();

				if (trans != null && PaymentType.CCard == trans.PaymentType)
				{
					var transaction = new CCTransaction
					{
						GuWID = trans.GatewayReferenceNumber,
						Amount = (double)trans.Amount,
						TransactionId = trans.OrderNumber,
						Mode = TransactionMode.Live
					};

					var reversal = new WireCardNet.Processing.Functions.FncCcReversal()
					{
						FunctionId = "FN_Reversal1"
					};
					reversal.AddTransaction(transaction);

					var job = new WireCardNet.Processing.Job();
					job.AddFunction(reversal);


					job.JobId = "JOB_Reversal1";
					job.BusinessCaseSignature = trans.GatewayContractNumber; // WireCardNet.WireCard.WireCardUsername;

					var processing = new WireCardNet.Processing.ProcessingRequest();
					processing.AddJob(job);

					//processing.Send();
					var response = processing.GetResponse();

					var status = response.FindStatus(job.JobId, reversal.FunctionId, transaction.TransactionId);

					Debug.WriteLine("#################################");
					Debug.WriteLine("Reversal Result: " + status.Result);
					if (status.Error != null)
					{
						Debug.WriteLine("Advice: " + status.Error.Advice);
						Debug.WriteLine("Message: " + status.Error.Message);
					}
					Debug.WriteLine("#################################");
				}
			}

			return this.RedirectToAction("Index");
		}

		[HttpPost]
		public ActionResult Bookback(Guid orderId)
		{
			var order = OrderService.GetQuery().FirstOrDefault(c => c.Id == orderId);

			if (order != null)
			{
				var trans = order.Transactions.FirstOrDefault();

				if (trans != null && PaymentType.CCard == trans.PaymentType)
				{
					var transaction = new CCTransaction
					{
						GuWID = trans.ResponseGuWID,
						Amount = (double)(trans.Amount - 5),
						TransactionId = trans.OrderNumber,
						Mode = TransactionMode.Live
					};

					var bookback = new WireCardNet.Processing.Functions.FncCcBookback()
					{
						FunctionId = "FN_Bookback1"
					};
					bookback.AddTransaction(transaction);

					var job = new WireCardNet.Processing.Job();
					job.AddFunction(bookback);


					job.JobId = "JOB_Bookback1";
					job.BusinessCaseSignature = trans.GatewayContractNumber; // WireCardNet.WireCard.WireCardUsername;

					var processing = new WireCardNet.Processing.ProcessingRequest();
					processing.AddJob(job);

					//processing.Send();
					var response = processing.GetResponse();

					var status = response.FindStatus(job.JobId, bookback.FunctionId, transaction.TransactionId);

					Debug.WriteLine("#################################");
					Debug.WriteLine("Bookback result: " + status.Result);
					if (status.Error != null)
					{
						Debug.WriteLine("Advice: " + status.Error.Advice);
						Debug.WriteLine("Message: " + status.Error.Message);
					}
					Debug.WriteLine("#################################");
				}
			}

			return this.RedirectToAction("Index");
		}

		[HttpPost]
		public ActionResult Query(Guid orderId)
		{
			var order = OrderService.GetQuery().FirstOrDefault(c => c.Id == orderId);

			if (order != null)
			{
				var trans = order.Transactions.FirstOrDefault();

				if (trans != null && PaymentType.CCard == trans.PaymentType)
				{
					var transaction = new CCTransaction
					{
						GuWID = trans.ResponseGuWID,
						Amount = (double)(trans.Amount),
						TransactionId = trans.OrderNumber,
						Mode = TransactionMode.Live
					};

					var query = new WireCardNet.Processing.Functions.FncCcQuery()
					{
						FunctionId = "FN_Query1"
					};
					query.AddTransaction(transaction);

					var job = new WireCardNet.Processing.Job();
					job.AddFunction(query);


					job.JobId = "JOB_Query1";
					job.BusinessCaseSignature = trans.GatewayContractNumber; // WireCardNet.WireCard.WireCardUsername;

					var processing = new WireCardNet.Processing.ProcessingRequest();
					processing.AddJob(job);

					var response = processing.GetResponse();

					var status = response.FindStatus(job.JobId, query.FunctionId, transaction.TransactionId);
				}
			}

			return this.RedirectToAction("Index");
		}

		[HttpPost]
		public ActionResult Refund(Guid orderId, Guid orderLineId)
		{
			var order = OrderService.GetQuery().FirstOrDefault(c => c.Id == orderId);

			if (order != null)
			{
				var trans = order.Transactions.FirstOrDefault();

				if (trans != null && PaymentType.PayPal != trans.PaymentType)
				{
					var transaction = new CCTransaction
					{
						GuWID = trans.GatewayReferenceNumber,
						Amount = (double)trans.Amount,
						TransactionId = trans.OrderNumber,
						Mode = TransactionMode.Live
					};

					var refund = new WireCardNet.Processing.Functions.FncCCRefund()
					{
						FunctionId = "Func1"
					};
					refund.AddTransaction(transaction);

					var job = new WireCardNet.Processing.Job();
					job.AddFunction(refund);
					job.JobId = "Job1";
					job.BusinessCaseSignature = trans.GatewayContractNumber; // WireCardNet.WireCard.WireCardUsername;

					var processing = new WireCardNet.Processing.ProcessingRequest();
					processing.AddJob(job);

					//processing.Send();
					var response = processing.GetResponse();

					var status = response.FindStatus("Capture1", "Func1", trans.OrderNumber);

					if (status.Error == null)
					{

					}

				}
			}

			return this.RedirectToAction("Index");
		}

	}
}

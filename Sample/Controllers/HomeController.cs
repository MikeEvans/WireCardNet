using System;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Sample.Models;
using WireCardNet.Processing;
using WireCardNet.Processing.Transactions;
using WireCardNet.QPay;

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
                PaymentType = PaymentType.CCard
            };

            checkout.SetCustomParameter("orderId", order.Id.ToString());

            return View(checkout);
        }

        public ActionResult Confirm()
        {
            var response = CheckoutResponse.FromRequest(this.Request,
                successResponse =>
                {
                    Guid orderId;
                    if (Guid.TryParse(successResponse.CustomParameters["orderId"], out orderId))
                    {
                        var order = OrderService.GetQuery().FirstOrDefault(c => c.Id == orderId);
                        if (order != null)
                        {
                            order.Transactions.Add(successResponse);

                            // paypal payments dont need to be preathorized they are instantly captured 
                            if (successResponse.PaymentType == PaymentType.PayPal)
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
                },
                failureResponse =>
                {
                    // log failure 
                },
                cancelResponse =>
                {
                    // log cancel 
                }
            );

            return this.Json(new { }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Success()
        {
            return this.RedirectPermanent("Index");
        }

        public ActionResult Bookback()
        {
            return this.View();
        }

        public ActionResult Failure()
        {
            return this.View();
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
        public ActionResult Capure(Guid orderId)
        {
            var order = OrderService.GetQuery().FirstOrDefault(c => c.Id == orderId);

            if (order != null)
            {
                var trans = order.Transactions.FirstOrDefault();

                if (trans != null)
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
                        FunctionId = "Func1"
                    };
                    capture.AddTransaction(transaction);

                    var job = new WireCardNet.Processing.Job();
                    job.AddFunction(capture);


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

        [HttpPost]
        public ActionResult Bookback(Guid orderId)
        {
            var order = OrderService.GetQuery().FirstOrDefault(c => c.Id == orderId);

            if (order != null)
            {
                var trans = order.Transactions.FirstOrDefault();

                if (trans != null)
                {
                    var transaction = new CCTransaction
                    {
                        GuWID = trans.GatewayReferenceNumber,
                        Amount = (double)trans.Amount,
                        TransactionId = trans.OrderNumber,
                        Mode = TransactionMode.Live
                    };

                    var bookback = new WireCardNet.Processing.Functions.FncCcBookback()
                    {
                        FunctionId = "Func1"
                    };
                    bookback.AddTransaction(transaction);

                    var job = new WireCardNet.Processing.Job();
                    job.AddFunction(bookback);


                    job.JobId = "Job1";
                    job.BusinessCaseSignature = trans.GatewayContractNumber; // WireCardNet.WireCard.WireCardUsername;

                    var processing = new WireCardNet.Processing.ProcessingRequest();
                    processing.AddJob(job);

                    //processing.Send();
                    var response = processing.GetResponse();

                    var status = response.FindStatus("Bookback1", "Func1", trans.OrderNumber);

                    if (status.Error == null)
                    {

                    }
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

                if (trans != null)
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

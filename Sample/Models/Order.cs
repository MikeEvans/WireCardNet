using System;
using System.Linq;
using System.Collections.Generic;
using WireCardNet.QPay;

namespace Sample.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public decimal Amount
        {
            get { return this.OrderLines.Sum(c => c.Amount); }
        }

        public List<OrderLine> OrderLines { get; set; }
        public List<CheckoutSuccessResponse> Transactions { get; set; }
        public OrderState State { get; set; }
    }

    public enum OrderState
    {
        Cart,
        Preauthorized,
        Captured
    }

    public class OrderLine
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
    }

    public class Transaction
    {
        public decimal Amount { get; set; }
        public string GatewayContractNumber { get; set; }
        public string GatewayReferenceNumber { get; set; }
        public string TransactionId { get; set; }
    }
}
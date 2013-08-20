using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sample.Controllers;
using Sample.Models;
using WireCardNet.QPay;

namespace Sample
{
    public static class OrderService
    {
        private static readonly ConcurrentDictionary<Guid, Order> Orders = new ConcurrentDictionary<Guid, Order>();

        public static Order Create()
        {
            var order = new Order
            {
                Id = Guid.NewGuid(),
                Transactions = new List<CheckoutSuccessResponse>(),
                OrderLines = new List<OrderLine>
                {
                    new OrderLine { Id = Guid.NewGuid(), Name = "Orderline 1", Amount =  50.0m },
                    new OrderLine { Id = Guid.NewGuid(), Name = "Orderline 2", Amount =  50.0m }
                },
                State = OrderState.Cart
            };

            return order;

        }

        public static void AddOrUpdate(Order order)
        {
            Orders.AddOrUpdate(order.Id, order, (guid, order1) => order);
        }

        public static IQueryable<Order> GetQuery()
        {
            return Orders.Values.AsQueryable();
        }

        public static void ClearAll()
        {
            Orders.Clear();
        }
    }
}


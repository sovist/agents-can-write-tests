using System;
using Shop.Api.Dto;

namespace Shop.Data
{
    public class Order
    {
        public Guid Id { get; set; }

        public Guid CustomerId { get; set; }

        public decimal Subtotal { get; set; }

        public decimal LoyaltyDiscount { get; set; }

        public decimal Total { get; set; }

        public OrderStatus Status { get; set; }
    }
}
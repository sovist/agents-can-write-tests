using System;

namespace Shop.Api.Dto
{
    public class OrderDto
    {
        public Guid Id { get; set; }

        public decimal Subtotal { get; set; }

        public decimal PromoDiscount { get; set; }

        public decimal LoyaltyDiscount { get; set; }

        public decimal Total { get; set; }

        public OrderStatus Status { get; set; }
    }
}
using System;

namespace Shop.Api.Dto
{
    public class OrderLineDto
    {
        public Guid ProductId { get; set; }

        public int Quantity { get; set; }
    }
}
using System;
using System.Collections.Generic;

namespace Shop.Api.Dto
{
    public class PlaceOrderDto
    {
        public Guid CustomerId { get; set; }

        public List<OrderLineDto> Lines { get; set; }
    }
}
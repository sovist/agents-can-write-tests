using System;
using System.Collections.Generic;

namespace Shop.Api.Dto
{
    public class PlaceOrderDto
    {
        public Guid CustomerId { get; set; }

        public List<OrderLineDto> Lines { get; set; }

        /// <summary>
        /// The promo code to apply, or <c>null</c> for none.
        /// </summary>
        public string PromoCode { get; set; }
    }
}
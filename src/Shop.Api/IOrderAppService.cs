using System;
using System.Threading.Tasks;
using Shop.Api.Dto;

namespace Shop.Api
{
    /// <summary>
    /// Placing and reading orders. Prices are in euros.
    /// </summary>
    public interface IOrderAppService : IApplicationService
    {
        /// <summary>
        /// Prices and stores an order. Throws <see cref="ShopException"/> for an unknown customer, product or promo code.
        /// </summary>
        Task<OrderDto> Place(PlaceOrderDto request);

        Task<OrderDto> Get(Guid orderId);
    }
}
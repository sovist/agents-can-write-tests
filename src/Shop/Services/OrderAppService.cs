using System;
using System.Threading.Tasks;
using Shop.Api;
using Shop.Api.Dto;
using Shop.Data;
using Shop.Managers;

namespace Shop.Services
{
    internal class OrderAppService : IOrderAppService
    {
        private readonly ShopDbContext _dbContext;

        private readonly IOrderPricingManager _pricingManager;

        public OrderAppService(ShopDbContext dbContext, IOrderPricingManager pricingManager)
        {
            _dbContext = dbContext;
            _pricingManager = pricingManager;
        }

        public async Task<OrderDto> Place(PlaceOrderDto request)
        {
            var customer = await _dbContext.Customers.FindAsync(request.CustomerId);

            if (customer == null)
            {
                throw new ShopException(ShopExceptionType.UnknownCustomer);
            }

            var subtotal = 0m;

            foreach (var line in request.Lines)
            {
                var product = await _dbContext.Products.FindAsync(line.ProductId);

                if (product == null)
                {
                    throw new ShopException(ShopExceptionType.UnknownProduct);
                }

                subtotal += product.Price * line.Quantity;
            }

            var promoPercentOff = 0;

            var price = _pricingManager.Calculate(subtotal, promoPercentOff, customer.IsLoyaltyMember);

            var order = new Order
            {
                Id = Guid.NewGuid(),
                CustomerId = customer.Id,
                Subtotal = price.Subtotal,
                PromoDiscount = price.PromoDiscount,
                LoyaltyDiscount = price.LoyaltyDiscount,
                Total = price.Total,
                Status = OrderStatus.AwaitingPayment
            };

            _dbContext.Orders.Add(order);

            await _dbContext.SaveChangesAsync();

            return ToOrderDto(order);
        }

        public async Task<OrderDto> Get(Guid orderId)
        {
            var order = await _dbContext.Orders.FindAsync(orderId);

            if (order == null)
            {
                throw new ShopException(ShopExceptionType.UnknownOrder);
            }

            return ToOrderDto(order);
        }

        private static OrderDto ToOrderDto(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                Subtotal = order.Subtotal,
                PromoDiscount = order.PromoDiscount,
                LoyaltyDiscount = order.LoyaltyDiscount,
                Total = order.Total,
                Status = order.Status
            };
        }
    }
}
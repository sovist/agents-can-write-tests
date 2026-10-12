using Shop.Api;
using Shop.Api.Dto;

namespace Shop.Tests.EntityFactory
{
    /// <summary>
    /// The factory writes rows directly, so its defaults are checked against what the API produces for the same input.
    /// Products, customers and promo codes are checked by use: the order tests create them here and place orders through the API.
    /// </summary>
    public class ShopFactoryTests : TestBase
    {
        IOrderAppService OrderService => Resolve<IOrderAppService>();

        [Fact]
        public async Task CreateOrder_ShouldMatchAnOrderPlacedThroughTheApi()
        {
            // Arrange:
            // The same order twice: written by the factory, and placed by a customer through the API
            var orderCreated = await ShopFactory.CreateOrder(subtotal: 85.50m);

            var customer = await ShopFactory.CreateCustomer();

            var product = await ShopFactory.CreateProduct(price: 85.50m);

            var orderPlaced = await OrderService.Place(new PlaceOrderDto
            {
                CustomerId = customer.Id,
                Lines = new List<OrderLineDto>
                {
                    new OrderLineDto
                    {
                        ProductId = product.Id,
                        Quantity = 1
                    }
                }
            });

            // Act
            var fromFactory = await OrderService.Get(orderCreated.Id);

            var fromApi = await OrderService.Get(orderPlaced.Id);

            // Assert
            fromFactory.Subtotal.ShouldBe(fromApi.Subtotal);

            fromFactory.PromoDiscount.ShouldBe(fromApi.PromoDiscount);

            fromFactory.LoyaltyDiscount.ShouldBe(fromApi.LoyaltyDiscount);

            fromFactory.Total.ShouldBe(fromApi.Total);

            fromFactory.Status.ShouldBe(fromApi.Status);
        }
    }
}
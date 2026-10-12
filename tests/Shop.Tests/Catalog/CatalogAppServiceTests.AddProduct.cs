using Shop.Api.Dto;

namespace Shop.Tests.Catalog
{
    public partial class CatalogAppServiceTests
    {
        [Fact]
        public async Task AddProduct_ShouldPutTheProductOnSale_When_TheShopAddsIt()
        {
            // Arrange:
            // The shop adds a mug at 12.50, and a customer who is not a loyalty member orders one
            var customer = await ShopFactory.CreateCustomer();

            // Act
            var productId = await Service.AddProduct(name: "Mug", price: 12.50m);

            // Assert
            var orderPlaced = await OrderService.Place(new PlaceOrderDto
            {
                CustomerId = customer.Id,
                Lines = new List<OrderLineDto>
                {
                    new OrderLineDto
                    {
                        ProductId = productId,
                        Quantity = 1
                    }
                }
            });

            orderPlaced.Total.ShouldBe(12.50m);
        }
    }
}
using Shop.Api.Dto;

namespace Shop.Tests.Catalog
{
    public partial class CatalogAppServiceTests
    {
        [Fact]
        public async Task AddPromoCode_ShouldTakeItsPercentageOffAnOrder_When_ACustomerUsesIt()
        {
            // Arrange:
            // Confirmed example E2 of ORD-2: a customer who is not a loyalty member orders 100.00 with SPRING10
            var customer = await ShopFactory.CreateCustomer();

            var product = await ShopFactory.CreateProduct(price: 100.00m);

            // Act
            await Service.AddPromoCode(code: "SPRING10", percentOff: 10);

            // Assert
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
                },
                PromoCode = "SPRING10"
            });

            orderPlaced.PromoDiscount.ShouldBe(10.00m);

            orderPlaced.Total.ShouldBe(90.00m);
        }
    }
}
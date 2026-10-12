using Shop.Api.Dto;

namespace Shop.Tests.Customers
{
    public partial class CustomerAppServiceTests
    {
        [Fact]
        public async Task Register_ShouldGiveTheLoyaltyDiscount_When_TheCustomerJoinsAsALoyaltyMember()
        {
            // Arrange:
            // Confirmed example E3 of ORD-2: a loyalty member orders 100.00 without a promo code
            var product = await ShopFactory.CreateProduct(price: 100.00m);

            // Act
            var customerId = await Service.Register(name: "Dana", isLoyaltyMember: true);

            // Assert
            var orderPlaced = await OrderService.Place(new PlaceOrderDto
            {
                CustomerId = customerId,
                Lines = new List<OrderLineDto>
                {
                    new OrderLineDto
                    {
                        ProductId = product.Id,
                        Quantity = 1
                    }
                }
            });

            orderPlaced.LoyaltyDiscount.ShouldBe(5.00m);

            orderPlaced.Total.ShouldBe(95.00m);
        }
    }
}
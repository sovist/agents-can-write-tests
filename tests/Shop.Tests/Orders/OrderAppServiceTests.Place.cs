using Shop.Api.Dto;

namespace Shop.Tests.Orders
{
    public partial class OrderAppServiceTests
    {
        /// <summary>
        /// The confirmed examples of ORD-2, copied as written in <c>docs/tickets/ORD-2-promo-codes.md</c>.
        /// The values come from the product owner, not from running the code. Never change one to make a test pass.
        /// </summary>
        public static TheoryData<string, decimal, string, bool, decimal, decimal, decimal> ConfirmedExamples => new TheoryData<string, decimal, string, bool, decimal, decimal, decimal>
        {
            // Example, subtotal, promo code, loyalty member, promo discount, loyalty discount, total
            { "E1", 100.00m, "SPRING10", true, 10.00m, 4.50m, 85.50m },
            { "E2", 100.00m, "SPRING10", false, 10.00m, 0.00m, 90.00m },
            { "E3", 100.00m, null, true, 0.00m, 5.00m, 95.00m },
            { "E4", 19.99m, "SPRING10", true, 2.00m, 0.90m, 17.09m }
        };

        [Theory]
        [MemberData(nameof(ConfirmedExamples))]
        public async Task Place_ShouldPriceTheOrder_As_TheConfirmedExample(
            string example,
            decimal subtotal,
            string promoCode,
            bool isLoyaltyMember,
            decimal promoDiscount,
            decimal loyaltyDiscount,
            decimal total)
        {
            // Arrange:
            // A customer orders one product worth the subtotal, then looks at the order
            await ShopFactory.CreatePromoCode("SPRING10", percentOff: 10);

            var customer = await ShopFactory.CreateCustomer(_ => _.IsLoyaltyMember = isLoyaltyMember);

            var product = await ShopFactory.CreateProduct(subtotal);

            // Act
            var placed = await Service.Place(new PlaceOrderDto
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
                PromoCode = promoCode
            });

            // Assert
            var order = await Service.Get(placed.Id);

            (order.PromoDiscount, order.LoyaltyDiscount, order.Total).ShouldBe((promoDiscount, loyaltyDiscount, total), example);
        }

        [Fact]
        public async Task Place_ShouldRejectTheOrder_When_ThePromoCodeIsUnknown()
        {
            // Arrange:
            // Confirmed example E5: a code the shop never issued
            var customer = await ShopFactory.CreateCustomer(_ => _.IsLoyaltyMember = true);

            var product = await ShopFactory.CreateProduct(100.00m);

            // Act
            // Assert
            await Service.Place(new PlaceOrderDto
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
                PromoCode = "NOSUCHCODE"
            }).ShouldThrowAsync(ShopExceptionType.UnknownPromoCode);
        }
    }
}
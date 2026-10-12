using Shop.Api.Dto;

namespace Shop.Tests.Orders
{
    public partial class OrderAppServiceTests
    {
        [Fact]
        public async Task Get_ShouldFail_When_TheOrderIsUnknown()
        {
            // Arrange:
            // The contract of IOrderAppService.Get: an order the shop never took cannot be read

            // Act
            // Assert
            await Service.Get(Guid.NewGuid()).ShouldThrowAsync(ShopExceptionType.UnknownOrder);
        }
    }
}
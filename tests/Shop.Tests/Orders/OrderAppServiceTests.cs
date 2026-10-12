using Shop.Api;

namespace Shop.Tests.Orders
{
    public partial class OrderAppServiceTests : TestBase
    {
        IOrderAppService Service => Resolve<IOrderAppService>();
    }
}
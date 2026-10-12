using Shop.Api;
using Shop.Tests.Base;

namespace Shop.Tests.Orders
{
    public partial class OrderAppServiceTests : TestBase
    {
        IOrderAppService Service => Resolve<IOrderAppService>();
    }
}
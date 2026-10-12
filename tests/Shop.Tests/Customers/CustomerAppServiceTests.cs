using Shop.Api;

namespace Shop.Tests.Customers
{
    public partial class CustomerAppServiceTests : TestBase
    {
        ICustomerAppService Service => Resolve<ICustomerAppService>();

        IOrderAppService OrderService => Resolve<IOrderAppService>();
    }
}
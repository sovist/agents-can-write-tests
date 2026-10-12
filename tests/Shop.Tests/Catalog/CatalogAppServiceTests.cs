using Shop.Api;

namespace Shop.Tests.Catalog
{
    public partial class CatalogAppServiceTests : TestBase
    {
        ICatalogAppService Service => Resolve<ICatalogAppService>();

        IOrderAppService OrderService => Resolve<IOrderAppService>();
    }
}
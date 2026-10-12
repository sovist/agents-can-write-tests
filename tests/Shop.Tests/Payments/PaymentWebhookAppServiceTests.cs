using Shop.Api;
using Shop.Tests.Base;

namespace Shop.Tests.Payments
{
    public partial class PaymentWebhookAppServiceTests : TestBase
    {
        IPaymentWebhookAppService Service => Resolve<IPaymentWebhookAppService>();

        IOrderAppService OrderService => Resolve<IOrderAppService>();
    }
}
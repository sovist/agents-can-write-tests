using Shop.Api;

namespace Shop.Tests.Payments
{
    public partial class PaymentWebhookAppServiceTests : TestBase
    {
        IPaymentWebhookAppService Service => Resolve<IPaymentWebhookAppService>();

        IOrderAppService OrderService => Resolve<IOrderAppService>();
    }
}
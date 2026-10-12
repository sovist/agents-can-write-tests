using System.Threading.Tasks;
using Shop.Api;
using Shop.Api.Dto;

namespace Shop.Services
{
    internal class PaymentWebhookAppService : IPaymentWebhookAppService
    {
        public Task<WebhookOutcome> Handle(string payload)
        {
            return Task.FromResult(WebhookOutcome.Ignored);
        }
    }
}
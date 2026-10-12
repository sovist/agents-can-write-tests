using System.Threading.Tasks;
using Shop.Api.Dto;

namespace Shop.Api
{
    /// <summary>
    /// Receives ExamplePay webhook events.
    /// </summary>
    public interface IPaymentWebhookAppService : IApplicationService
    {
        /// <summary>
        /// Marks the referenced order paid when a <c>payment.succeeded</c> event carries the order's
        /// exact total. Any other event type, or a redelivery, changes nothing.
        /// </summary>
        Task<WebhookOutcome> Handle(string payload);
    }
}
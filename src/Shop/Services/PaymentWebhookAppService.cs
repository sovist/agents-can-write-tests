using System;
using System.Text.Json;
using System.Threading.Tasks;
using Shop.Api;
using Shop.Api.Dto;
using Shop.Data;

namespace Shop.Services
{
    internal class PaymentWebhookAppService : IPaymentWebhookAppService
    {
        private const string ShopCurrency = "EUR";

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };

        private readonly ShopDbContext _dbContext;

        public PaymentWebhookAppService(ShopDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<WebhookOutcome> Handle(string payload)
        {
            var payment = JsonSerializer.Deserialize<PaymentEvent>(payload, JsonOptions);

            if (payment?.Type != "payment.succeeded")
            {
                return WebhookOutcome.Ignored;
            }

            if (!Guid.TryParse(payment.Data.Reference, out var orderId))
            {
                return WebhookOutcome.Rejected;
            }

            var order = await _dbContext.Orders.FindAsync(orderId);

            if (order == null)
            {
                return WebhookOutcome.Rejected;
            }

            if (order.Status == OrderStatus.Paid)
            {
                return WebhookOutcome.Ignored;
            }

            // ExamplePay sends amounts in the currency's minor unit: 85.50 EUR arrives as 8550.
            if (payment.Data.Currency != ShopCurrency || payment.Data.Amount != order.Total * 100)
            {
                return WebhookOutcome.Rejected;
            }

            order.Status = OrderStatus.Paid;
            order.PaymentId = payment.Data.PaymentId;

            await _dbContext.SaveChangesAsync();

            return WebhookOutcome.Applied;
        }

        private class PaymentEvent
        {
            public string Id { get; set; }

            public string Type { get; set; }

            public PaymentEventData Data { get; set; }
        }

        private class PaymentEventData
        {
            public string PaymentId { get; set; }

            public string Reference { get; set; }

            public long Amount { get; set; }

            public string Currency { get; set; }
        }
    }
}
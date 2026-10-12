using Shop.Api.Dto;
using Shop.Tests.Payments.Fixtures;

namespace Shop.Tests.Payments
{
    public partial class PaymentWebhookAppServiceTests
    {
        [Fact]
        public async Task Handle_ShouldMarkTheOrderPaid_When_ThePaymentSucceeded()
        {
            // Arrange:
            // A customer places an 85.50 order and pays. ExamplePay reports the amount in cents
            var placed = await ShopFactory.CreateOrder(85.50m);

            var payload = ProviderPayload.PaymentSucceeded(placed.Id);

            // Act
            var outcome = await Service.Handle(payload);

            // Assert
            outcome.ShouldBe(WebhookOutcome.Applied);

            var order = await OrderService.Get(placed.Id);

            order.Status.ShouldBe(OrderStatus.Paid);
        }

        [Fact]
        public async Task Handle_ShouldRejectThePayment_When_TheAmountDiffersFromTheOrderTotal()
        {
            // Arrange:
            // The order is 85.00, but ExamplePay reports a payment of 85.50
            var placed = await ShopFactory.CreateOrder(85.00m);

            var payload = ProviderPayload.PaymentSucceeded(placed.Id);

            // Act
            var outcome = await Service.Handle(payload);

            // Assert
            outcome.ShouldBe(WebhookOutcome.Rejected);

            var order = await OrderService.Get(placed.Id);

            order.Status.ShouldBe(OrderStatus.AwaitingPayment);
        }

        [Fact]
        public async Task Handle_ShouldApplyThePaymentOnce_When_TheEventIsDeliveredTwice()
        {
            // Arrange:
            // ExamplePay delivers at least once, so the same event can arrive again
            var placed = await ShopFactory.CreateOrder(85.50m);

            var payload = ProviderPayload.PaymentSucceeded(placed.Id);

            await Service.Handle(payload);

            // Act
            var outcome = await Service.Handle(payload);

            // Assert
            outcome.ShouldBe(WebhookOutcome.Ignored);

            var order = await OrderService.Get(placed.Id);

            order.Status.ShouldBe(OrderStatus.Paid);
        }

        [Fact]
        public async Task Handle_ShouldLeaveTheOrderAwaitingPayment_When_ThePaymentFailed()
        {
            // Arrange:
            // The card is declined. The customer can try again
            var placed = await ShopFactory.CreateOrder(85.50m);

            var payload = ProviderPayload.PaymentFailed(placed.Id);

            // Act
            var outcome = await Service.Handle(payload);

            // Assert
            outcome.ShouldBe(WebhookOutcome.Ignored);

            var order = await OrderService.Get(placed.Id);

            order.Status.ShouldBe(OrderStatus.AwaitingPayment);
        }
    }
}
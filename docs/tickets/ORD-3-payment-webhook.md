# ORD-3 Paid orders from ExamplePay

## Ticket

When ExamplePay tells us a payment went through, mark the order paid.

## Restatement

Written by the agent before any code, from the provider's page,
[`docs/provider/examplepay-webhooks.md`](../provider/examplepay-webhooks.md):

> - R1. `payment.succeeded` for an 85.50 order arrives with `"amount": 8550` and `"currency": "EUR"`
>   (amounts are in minor units): the order is paid.
> - R2. A `payment.succeeded` whose amount or currency differs from the order: rejected, and the
>   order still awaits payment.
> - R3. The same event delivered twice (the provider delivers at least once): applied once.
> - R4. `payment.failed`: nothing changes, and the customer can pay again.
>
> Fixtures: the two examples on the provider's page, copied unchanged. Only `data.reference` is set
> per test; the provider echoes it back from checkout, so it is ours to choose.

## Confirmed examples

R1–R4 as restated.

## Red first

Tests: [`PaymentWebhookAppServiceTests.Handle.cs`](../../tests/Shop.Tests/Payments/PaymentWebhookAppServiceTests.Handle.cs),
run against a handler that ignores every event.

Predicted before the run:

- R1 fails: the outcome is `Ignored`, not `Applied`.
- R2 fails: the outcome is `Ignored`, not `Rejected`.
- R3 fails on the order's status: it is never paid.
- R4 passes: doing nothing is the right answer to a failed payment. The test guards against a
  handler that treats any event as a payment.

Actual (trimmed):

```
Failed Handle_ShouldMarkTheOrderPaid_When_ThePaymentSucceeded            outcome should be Applied but was Ignored
Failed Handle_ShouldRejectThePayment_When_TheAmountDiffersFromTheOrderTotal outcome should be Rejected but was Ignored
Failed Handle_ShouldApplyThePaymentOnce_When_TheEventIsDeliveredTwice     status should be Paid but was AwaitingPayment
Failed: 3, Passed: 1, Total: 4
```

What the provider's examples protect against is in
[`docs/misreads/payment-amount.patch`](../misreads/payment-amount.patch): a parser that reads
`amount` as euros, with a sample payload written to match it.

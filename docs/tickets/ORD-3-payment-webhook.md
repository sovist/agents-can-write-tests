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

# ExamplePay webhooks

> ExamplePay is fictional. This page stands in for a payment provider's published webhook
> reference. In a real project you link the provider's documentation, or keep payloads captured
> from its sandbox. What matters is who wrote it: someone other than the agent that writes the
> parser.

ExamplePay sends an HTTP `POST` with a JSON body to the merchant's webhook URL for every event.

## Delivery

Delivery is at least once. The same event can arrive more than once; a redelivery carries the same
`id`.

## Event object

| Field | Type | Description |
|---|---|---|
| `id` | string | Unique event id. |
| `type` | string | `payment.succeeded` or `payment.failed`. |
| `created_at` | string | ISO 8601, UTC. |
| `data.payment_id` | string | ExamplePay's payment id. |
| `data.reference` | string | The merchant's own reference, passed at checkout and echoed back unchanged. |
| `data.amount` | integer | The amount in the currency's minor unit: `8550` is 85.50 EUR. |
| `data.currency` | string | ISO 4217 code, upper case. |
| `data.failure_code` | string | `payment.failed` only: `card_declined`, `insufficient_funds` or `expired_card`. |

## Examples

### `payment.succeeded`

```json
{
  "id": "evt_7Hq2KxN4",
  "type": "payment.succeeded",
  "created_at": "2026-10-12T09:41:07Z",
  "data": {
    "payment_id": "pay_3Fm9TzR1",
    "reference": "c0a8012e-5b1c-4c7e-9a43-2f6d8e1b7a90",
    "amount": 8550,
    "currency": "EUR"
  }
}
```

### `payment.failed`

```json
{
  "id": "evt_9Lw4PcD8",
  "type": "payment.failed",
  "created_at": "2026-10-12T09:43:52Z",
  "data": {
    "payment_id": "pay_6Vb1QsE5",
    "reference": "c0a8012e-5b1c-4c7e-9a43-2f6d8e1b7a90",
    "amount": 8550,
    "currency": "EUR",
    "failure_code": "card_declined"
  }
}
```

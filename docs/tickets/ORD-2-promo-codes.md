# ORD-2 Promo codes

## Ticket

Loyalty members already get 5% off every order. Add promo codes: a code takes its percentage off
the order. A customer can use a promo code and the loyalty discount on the same order.

## Restatement

Written by the agent before any code, as worked examples:

> - R1. A loyalty member orders 100.00 with `SPRING10` (10%). Both discounts come off the subtotal
>   and add up: 10.00 + 5.00 off, total **85.00**.
> - R2. The same order from a customer who is not a loyalty member: 10.00 off, total 90.00.
> - R3. A code the shop never issued: the order is rejected.

## Confirmed examples

The product owner's answer: R1 is wrong. The loyalty discount comes off what remains after the promo
code, 100.00 → 90.00 → **85.50**. Each discount is rounded to cents, half away from zero.

| Example | Subtotal | Promo code | Loyalty member | Promo discount | Loyalty discount | Total |
|---|---|---|---|---|---|---|
| E1 | 100.00 | `SPRING10` (10%) | yes | 10.00 | 4.50 | 85.50 |
| E2 | 100.00 | `SPRING10` (10%) | no | 10.00 | 0.00 | 90.00 |
| E3 | 100.00 | none | yes | 0.00 | 5.00 | 95.00 |
| E4 | 19.99 | `SPRING10` (10%) | yes | 2.00 (from 1.999) | 0.90 (from 0.8995) | 17.09 |

E5. A code the shop never issued (`NOSUCHCODE`): the order is rejected.

The 50 cents between R1 and E1 is the ticket. No harness could have found them: both readings run
cleanly through the real container and the real database. Only a person reading R1 could.

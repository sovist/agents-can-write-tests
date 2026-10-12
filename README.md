# Agents can write tests

[![CI](https://github.com/sovist/agents-can-write-tests/actions/workflows/ci.yml/badge.svg)](https://github.com/sovist/agents-can-write-tests/actions/workflows/ci.yml)

They can, when the repo makes two things true: the cheapest test to write tells a user story, and the
expected values come from somewhere other than the agent's own reading of the ticket.

When one agent writes both the code and its tests, a passing test can mean two things: the code is
right, or the test checks the code against itself. That happens in two ways, and each needs its own
rule:

| | How the test checks the code against itself | The rule that stops it |
|---|---|---|
| **Mock tautology** | The test stubs a collaborator, then asserts what it told the stub to return. | **1. A test tells a user story.** A harness that boots the real container and database, and hands out only application services, makes that the cheapest test to write. |
| **Interpretation tautology** | The expected value comes from the same reading of the ticket that produced the code. | **2. Someone other than the agent decides the right answer.** A person confirms worked examples, and the other system's documentation defines its payloads. |

Neither rule is enough on its own. The harness can't tell whether the agent read the ticket right.
And a confirmed value asserted against a mock only checks the mock. This repo is a small, runnable
.NET example of both rules, and CI shows where the first one stops and the second one takes over.

## Rule 1: a test tells a user story

Each test reads as something a user, or another system, does, and what they see afterwards. There is
no mock in the suite: no mocking library is referenced, and nothing of the application's own is
substituted.

```csharp
[Fact]
public async Task Handle_ShouldMarkTheOrderPaid_When_ThePaymentSucceeded()
{
    // Arrange:
    // A customer places an 85.50 order and pays. ExamplePay reports the amount in cents
    var orderPlaced = await ShopFactory.CreateOrder(subtotal: 85.50m);

    var payload = ProviderPayload.PaymentSucceeded(orderPlaced.Id);

    // Act
    var outcome = await Service.Handle(payload);

    // Assert
    outcome.ShouldBe(WebhookOutcome.Applied);

    var order = await OrderService.Get(orderPlaced.Id);

    order.Status.ShouldBe(OrderStatus.Paid);
}
```

Agents take the cheapest path. The harness makes the story the cheapest test to write, and closes the
shortcuts around it:

| Rule | How |
|---|---|
| Tests reach only application services | This project's [`TestBase`](tests/Shop.Tests/TestBase.cs) overrides the harness's `Resolve<T>()`, and fails the test when `T` is not an `IApplicationService`, and says what to do instead. [`ResolveTests`](tests/Shop.Tests/Boundaries/ResolveTests.cs) checks that it refuses the database context and the container |
| Nothing behind them is reachable | Contracts, data and implementations are separate projects. `Shop.Api` holds the service interfaces and, in `Dto/`, what they exchange. `Shop.Data` holds the entities and the database context. `Shop` holds the internal services and managers. [`PublicSurfaceTests`](tests/Shop.Tests/Boundaries/PublicSurfaceTests.cs) fails if `Shop` exports anything but its composition root or opens its internals, or if `Shop.Api` or `Shop.Data` export anything else |
| The container is the production one | `TestBase` boots `AddShop`, the composition root a host calls |
| Reads come from the database | Each `Resolve<T>()` opens a new scope, as a new request would. The database is SQLite in-memory, not EF Core's InMemory provider, which is not relational |
| Data comes from a factory, checked against the API | [`ShopFactory`](tests/Shop.Tests.Base/EntityFactory/ShopFactory.cs) is the default and preferred way to create test data. It writes entities through the database context, like a production entity factory, and is the only code that gets a context instance. [`ShopFactoryTests`](tests/Shop.Tests/EntityFactory/ShopFactoryTests.cs) compares its orders with orders placed through the API. Its products, customers and promo codes are checked by use, because the order tests place real orders with them. Data the application derives from other data, such as an order with discounts, is created through the API instead, so the application computes it |

The shared harness keeps the plain `Resolve<T>()` of a production test base, and each test project
decides what its tests may ask for. Here, the check in the project's `TestBase` is where "only
application services" stops being a convention: a test that asks for anything else fails.

## Rule 2: someone other than the agent decides the right answer

The tickets are illustrations, and so is their product owner. The test runs they quote are real. The
history shows them too: each ticket has a red commit and a green one, and checking out the red commit
reproduces the failures its ticket records.

**Confirmed examples.** Before any code, the agent restates the ticket as worked examples with
numbers, and a person confirms or corrects them. In [ORD-2](docs/tickets/ORD-2-promo-codes.md) the
restatement said 85.00 and the product owner said 85.50. Prose like "both discounts apply" would have
hidden that. The tests copy the confirmed table. They were run red first, against a written
prediction of which would fail and why, and the ticket records both.

**The other side's payloads.** [ORD-3](docs/tickets/ORD-3-payment-webhook.md) handles webhooks from
ExamplePay, a stand-in provider whose [documentation](docs/provider/examplepay-webhooks.md) lives in
the repo. The fixtures are its examples, copied unchanged, and
[`FixtureProvenanceTests`](tests/Shop.Tests/Boundaries/FixtureProvenanceTests.cs) fails on any fixture
that isn't one. When the same hand writes the parser and the sample data, the test only proves that
the two agree. That is how a parser ships green and fails on its first real event.

## See the gap

Rule 1 can't catch a misread. Each patch in [`docs/misreads/`](docs/misreads/) introduces a plausible
misread, together with the story test an agent would write from it. Those tests follow every harness
rule.

```bash
git apply docs/misreads/promo-and-loyalty.patch
dotnet test
git apply -R docs/misreads/promo-and-loyalty.patch
```

| Patch | The misread | The agent's own test | What catches it |
|---|---|---|---|
| `promo-and-loyalty.patch` | The loyalty discount comes off the subtotal, not off what remains after the promo code: 85.00 instead of 85.50. | passes | Confirmed examples E1 and E4 from [ORD-2](docs/tickets/ORD-2-promo-codes.md) |
| `payment-amount.patch` | The webhook parser reads `amount` as euros, and the agent's sample payload says `"amount": 85.50`. | passes | The provider's documented payload (`8550`, in cents), and `FixtureProvenanceTests` |

The agent's test and the test that catches it run through the same harness: the real composition
root, a real SQLite database, application services only. The harness can't tell them apart. What
differs is where the expected value came from.

The `misreads` CI job applies each patch and fails the build if the agent's test stops passing or the
catching tests stop failing ([`.github/misread.sh`](.github/misread.sh)).

## Use it with your agent

[`AGENTS.md`](AGENTS.md) holds both rules, written as instructions an agent follows; `CLAUDE.md`
imports it. The rules carry over to any stack. The harness rules name this repo's types. The shape to
copy: a constrained resolve, internal services, one entity factory checked against the API, a public
surface test, and a provenance test for anything another system sends.

## Layout

```
AGENTS.md                        The rules
docs/tickets/                    Tickets: restatement, confirmed examples, red-first record
docs/provider/                   The stand-in provider's webhook documentation
docs/misreads/                   One misread of each kind, as patches
src/Shop.Api/                    Application service interfaces: the only entry points
src/Shop.Api/Dto/                What they exchange: DTOs, enums, the exception
src/Shop.Data/                   Entities and the database context. In tests, only the factory writes them
src/Shop/                        Internal implementations (Services/) and rules (Managers/)
tests/Shop.Tests.Base/           The harness: TestBase, ShopFactory, ShouldlyExtensions, ResourceHelper
tests/Shop.Tests/                The tests, the provider's payload fixtures
tests/Shop.Tests/TestBase.cs     This project's TestBase: the Resolve rule, and helpers only these tests need
tests/Shop.Tests/Boundaries/     Resolve, public surface and fixture provenance checks
tests/Shop.Tests/EntityFactory/  Checks the factory against the API
Imports.Tests.cs                 Global usings, linked into both test projects
```

Run `dotnet test` with the .NET 10 SDK.

Code style: an Api project for contracts, a Data project for entities and a service project for
implementations; a `Manager` that owns the rules behind an `AppService` that orchestrates; block-scoped
namespaces, `_camelCase` private fields and explicit constructors. `.editorconfig` spells it out.

## Related

- [agent-loop](https://github.com/sovist/agent-loop): a plan-gated loop for Claude Code where these
  rules run. The plan fixes the contracts, the regression test must fail as predicted, and the
  reviewer works in a fresh context.
- [resolve-pattern](https://github.com/sovist/resolve-pattern): where the `Resolve<T>()` name comes
  from. This sample uses plain constructor injection, so the harness stands on its own.

## License

[MIT](LICENSE)

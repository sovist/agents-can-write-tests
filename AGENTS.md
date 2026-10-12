# Writing tests in this repository

These rules are for any agent, and for people. There are two, and every test needs both:

1. **A test tells a user story.** This keeps the test honest about the system.
2. **Every expected value comes from outside your reading of the ticket.** This keeps the test honest
   about the ticket.

A test that breaks the first rule checks mocks. A test that breaks the second proves only that your
code agrees with your reading.

## Rule 1: a test tells a user story

### Write the story

- A test reads as a short story: a user, or another system, does something, maybe something else,
  and here is what they see. Arrange sets up the world. Act is the action the test is about. Assert
  reads the outcome the way a user would see it.
- Mark the sections with `// Arrange:` followed by one line that states the case in plain words, then
  `// Act` and `// Assert`. Keep the markers even when a section is one line.
- Name the test `Method_ShouldResult_When_Condition`. For an application service, put it in the
  partial file of the method it acts on: `OrderAppServiceTests.Place.cs` for `Place`.
- Assert what a user can observe: what the service returns, or what a later read through a service
  shows. Never assert which method was called, or with what.
- No mocks of the application's own code: no stubbed service, manager or database. If a test seems to
  need one, the setup belongs in `ShopFactory`, or the action belongs in a different test.
- Substitute only what crosses the process boundary, such as another company's API.

### Go through the harness

- Inherit `TestBase`. Get services only with `Resolve<T>()`; it accepts application services and
  nothing else.
- Create test data with `ShopFactory.Create...` by default. Never write rows yourself. If the factory
  lacks what you need, add a `Create...` method to it.
- When the data is something the application derives from other data, such as an order with
  discounts, create it through the API instead, so the application computes it. If several tests need
  it, give it a named helper on the test base rather than repeating the calls.
- A `Create...` method takes the values a test is about as parameters, and anything else through its
  `update` delegate. Use `update` only for fields the test is about, never for values the application
  computes, such as totals.
- The factory writes rows directly, so its defaults must be what the application would produce. When
  the API can produce the same thing, add a test to `ShopFactoryTests` that compares the two.
- Read results back through an application service. Each `Resolve<T>()` opens a new scope, so the
  read goes to the database.
- Do not make a type public, or add `InternalsVisibleTo`, to reach it from a test.
  `PublicSurfaceTests` fails if you do.

## Rule 2: every expected value comes from outside your reading

### Every expected value needs a source

Before you write an assertion, name where its expected value comes from. It must be something
other than the code under test:

- a confirmed example in the ticket (`docs/tickets/`),
- the documentation of the system on the other side (`docs/provider/`),
- behavior that already exists and that users rely on.

If you got the value by reading the implementation, or by running it and copying the output, the
test checks the code against itself. Find the real source, or ask.

### Before any code, restate the ticket as numbers

1. Under `## Restatement` in the ticket file, restate the ticket as worked examples with concrete
   inputs and outputs. Prose like "both discounts apply" hides the question that matters; "100.00
   becomes 85.00" exposes it.
2. Stop, and ask a person to confirm or correct the examples. Do not start the implementation
   until they have answered.
3. Their answer goes under `## Confirmed examples`. Tests copy those values exactly, and cite the
   example.

Never change a confirmed value to make a test pass. If the code and a confirmed example disagree,
either the code is wrong or the example goes back to the person.

### Red first, failing as predicted

1. Add the contract (signatures, request and response fields), then write the tests from the
   confirmed examples, before the implementation.
2. Before you run them, write down under `## Red first` which tests will fail, and on which
   assertion, and which will pass, and why.
3. Run them. Each test must fail or pass as you predicted. A failure on a compile error, a missing
   registration or a null reference does not count: fix it and run again.
4. Record the actual output next to the prediction. Then implement.

A test that passes before the change is fine only if you predicted it and said why, for example
because it checks existing behavior the change must keep.

### Payloads come from the other side

- A payload from another system is copied from that system's documentation, or captured from it.
  Never write one yourself to fit the parser.
- Copy it unchanged. Replace only fields that are yours to choose, such as `data.reference`, and
  do it at load time.
- Embed it as a resource and give it its own method in `ProviderPayload`, named after the example,
  such as `ProviderPayload.PaymentSucceeded(reference)`. Tests never pass file names around.
- `FixtureProvenanceTests` fails if a fixture is not an example from `docs/provider/`.
- If the documentation has no example for the case you need, ask for one. Do not invent it.

## Why both rules

The harness keeps a test honest about the system: real container, real database, application
services only, no stub answering with what you told it to. It cannot tell whether you read the
ticket correctly. A misread runs through the real pipeline and passes its own test. Only expected
values fixed outside your reading catch it: the confirmed examples, and the provider's payloads.
`docs/misreads/` holds one misread of each kind.

The reverse holds too. A confirmed value asserted against a mock only checks the mock.

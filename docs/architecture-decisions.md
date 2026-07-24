# Architecture Decision Record — NumberRuleEvaluator

> Architectural decisions based on the [design-review-prompt.md](./design-review-prompt.md) specification.
> Date: 2026-07-23

---

## ADR-1: Separator Character for Multiple Matches

### Context

If a number matches multiple rules, the corresponding text values must be returned on a single line in alphabetical order. The specification does not define the separator character.

### Considered Options

| Option | Pros | Cons |
|---|---|---|
| Empty string (direct concatenation) | Classic FizzBuzz tradition (`"FizzBuzz"`) | Alphabetical concatenation might be unreadable (`"JeffreyPeter"`) |
| Space | Readable, simple | Problematic if rule names themselves contain spaces |
| Comma | Clear separation | Less "natural" to read |
| **Configurable separator, with a sensible default** | Maximum flexibility, minimal overhead | One additional configuration parameter |

### Decision

**Configurable separator, with a space (`" "`) as the default value.**

### Rationale

A configurable separator allows the consumer to customize the formatting of multi-match results. This can be solved with a single optional parameter (e.g., a `Separator` property on the configuration object), which adds negligible complexity. A space as the default is the most readable and common choice (`"Jeffrey Peter"`). If the consumer prefers a different format (comma, hyphen, empty string), they can easily override it.

---

## ADR-2: Target .NET Version

### Context

The library needs to be compatible with modern .NET applications while utilizing modern C# language features.

### Considered Options

| Option | Pros | Cons |
|---|---|---|
| .NET Standard 2.0 | Maximum compatibility (.NET Framework 4.6.1+, .NET Core 2.0+) | Lacks access to many modern APIs, older C# language version |
| .NET Standard 2.1 | Better API access, Span\\<T\\>, etc. | Does not support .NET Framework |
| .NET 8 | Widely adopted LTS, modern C# support | Only .NET 8+ consumers |
| Multi-target (.NET 8 + .NET Standard 2.0) | Broad compatibility + modern features | Increased build complexity, conditional compilation |

### Decision

**Strictly .NET 8 (LTS).**

### Rationale

- .NET 8 is a stable, widely adopted Long-Term Support version with broad industry support.
- Applications targeting newer .NET versions (e.g., .NET 9, .NET 10) can reference .NET 8-targeted libraries without compatibility issues.
- Adding .NET Standard 2.0/2.1 via multi-targeting would introduce unnecessary complexity — the specification targets modern .NET, and for such a simple library, .NET Framework compatibility is not a realistic expectation.
- The .NET 8 SDK and compiler support modern C# language elements (e.g., primary constructors, collection expressions), which aligns with the "modern C#" requirement.

---

## ADR-3: Handling Printing

### Context

The original task started with the requirement that "the client wants to print results to the screen." The follow-up assignment specifies that the solution must be a reusable class library. These two requirements create a tension: a reusable library should not depend directly on console output, yet the printing requirement is part of the original business need and cannot simply be discarded.

### Considered Options

| Option | Pros | Cons |
|---|---|---|
| Evaluation only | Simple, no I/O dependencies | Does not fulfill the original printing requirement |
| Direct console output in the library | Fulfills the printing requirement | Violates SoC; the library becomes tightly coupled to `System.Console` |
| **Evaluation + optional printing via Dependency Inversion (DIP)** | Fulfills the printing requirement while maintaining clean architecture; the solution can optionally delegate output without knowing the concrete target | One additional interface and a separate coordination component |

### Decision

**The solution provides both evaluation and optional output orchestration via a separate `NumberRuleEvaluator.Printing` class-library project using the Dependency Inversion Principle.**

`NumberRuleEvaluator.Core` contains only configuration and evaluation concerns. `NumberRuleEvaluator.Printing` references Core and forms an optional adapter/orchestration package containing the output abstraction (`IResultPrinter`) and `NumberRulePrintCoordinator`. The coordination component evaluates through Core and delegates to the abstraction, while the consuming application provides the concrete presentation adapter (e.g., `ConsolePrinter` in the Sample project). Printing is optional — consumers can reference and use Core without referencing the Printing project.

### Rationale

- **Printing was part of the original business requirement.** `IResultPrinter` is introduced not because the Core library owns presentation responsibility, but because printing was an explicit business requirement and DIP allows supporting it without introducing concrete I/O dependencies.
- **The coordinator belongs outside Core.** Evaluate-then-print is application orchestration rather than evaluation domain logic. The Printing project isolates that workflow while keeping the Core project focused and independently reusable.
- **Concrete presentation belongs to the consuming application.** `ConsolePrinter` is a presentation adapter supplied by the consumer. The Printing project is instead an optional adapter/orchestration package: it defines the output port and coordinates evaluation with output through that port.
- **Dependency Inversion keeps the boundary clean.** The Printing project owns the output abstraction because it owns the evaluate-and-output orchestration workflow. The consuming application provides the concrete adapter implementation. The evaluated result is optionally delegated to the injected output abstraction.
- **Both libraries remain reusable**: neither Core nor Printing depends on `System.Console` or any other concrete output target. Any implementation (console, file, logging framework) can be plugged in.
- **Printing is optional**: consumers who only need evaluation can use the `Evaluate` method directly, without providing a printer implementation.

---

## ADR-4: Testing Strategy

### Context

We need to determine the testing structure: how many test projects, what types of tests, and whether component-level testing is necessary.

### Considered Options

| Option | Pros | Cons |
|---|---|---|
| **One unit test project per library project** | Preserves project boundaries and allows each reusable library to be tested independently | One additional test project |
| Single test project, unit tests | Fewer projects | Blurs the Core and Printing project boundaries |
| Unit + component tests | Testing the interaction between components | Does not provide significant additional value in this scope |

### Decision

**Two unit test projects: one for Core and one for Printing.**

### Rationale

- `NumberRuleEvaluator.Core.Tests` tests configuration and evaluation independently from printing.
- `NumberRuleEvaluator.Printing.Tests` tests the coordinator using a test double implementing `IResultPrinter`.
- Component-level testing does not provide significant additional value in this scope.
- Separating tests by library project keeps dependencies and review boundaries explicit without adding unnecessary test types.
- xUnit and Theories will be used as specified.

---

## ADR-5: Handling Range Boundaries (Specification Clarification)

### Context

The specification states: "Values outside the configured range are considered invalid." It provides the example `Minimum: 14`, `Maximum: 72`, but does not explicitly define whether the boundaries are inclusive.

### Decision

**The boundaries are inclusive.** Both the minimum and maximum values are considered valid inputs.

### Rationale

- For user-defined minimum/maximum ranges, inclusive boundaries are the intuitive and most common expectation.
- If someone configures `Min: 14, Max: 72`, it is natural to expect that both `14` and `72` are valid inputs.

---

## ADR-6: API Design — Immutable Configuration and Evaluator Structure

### Context

The specification does not define how rules and the valid range should be passed to the evaluator. This API design decision needs to be made.

### Considered Options

| Option | Pros | Cons |
|---|---|---|
| Builder pattern | Fluent, readable configuration | Excessive complexity for this number of configuration parameters |
| Multiple constructor parameters | Simple | The number of parameters might grow |
| **Immutable configuration object + evaluator constructor injection** | Clean, immutable, easy to document, testable | — |
| Fluent API on the evaluator | "Nice" syntax | Mutable state, more complex lifecycle |

### Decision

**An immutable configuration object passed to the evaluator through constructor injection.**

### Proposed API Draft

```csharp
var config = new RuleEvaluatorConfig(
    range: new NumberRange(14, 72),
    rules:
    [
        new DivisorRule(3, "Peter"),
        new DivisorRule(5, "Jeffrey")
    ],
    separator: " "); // optional, default: " "

var evaluator = new RuleEvaluator(config);
string result = evaluator.Evaluate(15); // → "Jeffrey Peter"
```

> **Note on printing**: The optional printing coordination (see ADR-3) must not be placed directly on the `RuleEvaluator` class (e.g., as an `Execute` method), as that would mix evaluation and output responsibilities. `NumberRulePrintCoordinator` in the separate `NumberRuleEvaluator.Printing` project handles the evaluate-then-print workflow.

### Rationale

- **Immutability**: The configuration does not change after construction, providing thread safety and predictability.
- **Simplicity**: No builder is needed — the configuration is supplied through a small constructor with explicit arguments.
- **Testability**: It is easy to construct with different configurations in tests.
- **Validation**: The entire configuration can be validated in one place during construction (range consistency, rule validity).

---

## ADR-7: Input Validation and Error Handling

### Context

The specification states that an "appropriate exception" must be thrown for invalid out-of-range inputs. Additionally, there might be invalid values at the configuration level.

### Decision

**Use standard .NET exceptions, custom exceptions are not necessary.**

| Case | Exception |
|---|---|
| Input out of range | `ArgumentOutOfRangeException` |
| `min > max` in range configuration | `ArgumentException` |
| Null rule list | `ArgumentNullException` |
| Empty rule list | Valid configuration (every number returns itself as a string) |
| Duplicate divisor value in a rule list | `ArgumentException` |
| Zero or negative divisor | `ArgumentOutOfRangeException` |
| Empty or whitespace rule text | `ArgumentException` |
| Null separator | `ArgumentNullException` |
| Empty separator | Valid configuration (matching texts are concatenated directly) |
| Null configuration | `ArgumentNullException` |

### Rationale

- Standard .NET exceptions are self-describing and familiar to .NET developers.
- Custom exception classes (e.g., `NumberOutOfRangeException`) would introduce unnecessary complexity — standard exceptions with parameterized messages provide sufficient context.
- **Empty rule list is valid**: the specification states "any number of rules" — zero is a valid count. The behavior is well-defined: every number in range returns itself as a string. Throwing an exception here would violate the principle of least astonishment.
- Validation runs in the constructor (configuration) and in the `Evaluate` method (input) — fail-fast principle.
- **`DivisorRule` validates the divisor before the text.** If both are invalid, the constructor throws `ArgumentOutOfRangeException` for the divisor rather than a text-related exception. This precedence is documented on the constructor's XML doc and covered by a dedicated test.

---

## ADR-8: Solution Structure

### Context

The structure of the solution and projects needs to be defined.

### Decision

**Five projects, with Core and Printing separated by responsibility:**

```text
NumberRuleEvaluator.sln
├── src/
│   ├── NumberRuleEvaluator.Core/     # Configuration and evaluation class library
│   ├── NumberRuleEvaluator.Printing/ # Optional output orchestration class library
│   └── NumberRuleEvaluator.Sample/  # Sample console application
└── tests/
    ├── NumberRuleEvaluator.Core.Tests/
    └── NumberRuleEvaluator.Printing.Tests/
```

### Rationale

- The `src/` and `tests/` separation is a conventional .NET project structure.
- Core has no printing or concrete I/O dependency and can be referenced independently by consumers that only need evaluation.
- Printing references Core and encapsulates the optional adapter/orchestration workflow without a concrete I/O dependency.
- The Sample console app provides the concrete `ConsolePrinter` adapter and references the libraries it demonstrates.
- One test project per library project is consistent with the ADR-4 decision and preserves the project boundaries.

---

## ADR-9: Rule Extensibility — YAGNI vs. Open/Closed Principle

### Context

The current design (ADR-6) uses a concrete `DivisorRule` type for configuring rules. An alternative approach would be to introduce an `IEvaluationRule` interface, allowing consumers to define arbitrary rule types (e.g., `ContainsDigitRule`, `PrimeNumberRule`) without modifying the library.

This is a textbook case of the Open/Closed Principle (OCP). However, the specification explicitly states:

- *"The client application must be able to configure any number of **divisor-to-text mappings**."*
- *"Avoid introducing unnecessary frameworks, patterns or abstractions unless they clearly improve the solution."*

### Considered Options

| Option | Pros | Cons |
|---|---|---|
| `IEvaluationRule` interface | OCP-compliant, extensible to arbitrary rule types | Overengineering; the specification only requires divisor-to-text pairs; violates the "avoid unnecessary abstractions" guideline |
| **Concrete `DivisorRule` only** | Simple, directly maps to the specification, no unnecessary abstraction | Not extensible to non-divisor rules without introducing a new abstraction in a future version |

### Decision

**Concrete `DivisorRule` only. No `IEvaluationRule` interface.**

### Rationale

- **The specification does not require arbitrary rules.** The requirement is explicitly "divisor-to-text mappings" — introducing an interface for rule types that no one asked for would be overengineering.
- **The extensibility boundary is intentionally limited to configuration.** Consumers configure divisor-to-text mappings rather than define arbitrary rule behaviors, so an additional rule abstraction would not provide sufficient value to justify its complexity.
- **YAGNI (You Aren't Gonna Need It)**: Designing for hypothetical future requirements adds complexity now without delivering value. The design can evolve to an interface-based rule model in the future if requirements justify the added abstraction.
- **OCP is valuable, but not justified here.** Introducing an abstraction without a current requirement would add unnecessary complexity.

---

## ADR-10: Interfaces for Core Components

### Context

During the library design, a dilemma emerged: should the public API expose interfaces (e.g., `INumberRuleEvaluator`, `INumberRulePrintCoordinator`) for the core evaluation and orchestration components, or is it sufficient to publish the concrete classes?

### Considered Options

| Option | Pros | Cons |
|---|---|---|
| Publish interfaces for all components | Loose coupling; consumers can easily mock the components in their own unit tests | Unnecessary abstraction for deterministic logic; interface pollution |
| **Publish concrete classes for core components, interface only for I/O** | Simple API; reduced maintenance surface; aligns with modern .NET library practices | Consumers cannot replace the evaluator with a test double through dependency injection without introducing their own abstraction. |

### Decision

**Expose concrete classes for core library components (`NumberRuleEvaluator`, `NumberRulePrintCoordinator`), and use an interface only for the I/O boundary (`IResultPrinter`).**

### Rationale

- **Boundaries dictate abstractions**: In modern .NET ecosystem design, interfaces are primarily introduced at architectural boundaries (such as I/O, external systems, or where the consumer must provide an implementation, or where multiple implementations are expected). The `IResultPrinter` is a textbook example of an I/O boundary, justifying an interface.
- **Pure deterministic logic does not require abstraction**: The `NumberRuleEvaluator` is a deterministic component with no side effects or external dependencies, and is easily constructible. Furthermore, a small NuGet library is not obligated to provide interfaces for every public workflow service (like `NumberRulePrintCoordinator`).
- **Testability without mocks**: Consumers usually do not need to mock the evaluator because it has no external dependencies and can be executed directly in unit tests.
- **Industry precedence**: Modern .NET libraries generally introduce abstractions where there is a meaningful substitution point, external dependency, or testing boundary, rather than creating interfaces for every public class. Core framework components like `HttpClient` and `Regex` are commonly exposed as concrete types, while abstractions are introduced for specific extensibility or substitution scenarios.
- **Consumer flexibility**: Consumers remain free to wrap the concrete components behind their own interfaces if their application architecture requires additional abstraction.

---

## Specification Review Checklist

| Question | Answer |
|---|---|
| Is the specification complete? | Mostly yes. The separator character was missing (→ ADR-1), and boundary inclusiveness was implicit (→ ADR-5). |
| Are there ambiguous requirements? | The separator and range boundary handling — both resolved. |
| Are there conflicting requirements? | The conflict between "printing" and "reusable library" resolved via Dependency Inversion (→ ADR-3). |
| Is any part of the proposed design unnecessarily complex? | The DIP-based printing adds a small amount of complexity, but it is justified because printing was part of the original requirement while keeping the library independent from concrete I/O. |
| Can the design be simplified while remaining extensible? | The current design (configuration object + evaluator + optional printing via DIP) provides a good balance between simplicity, configurability, and extensibility. |
| Is the design aligned with SOLID and SoC? | Yes. The evaluator handles evaluation, printing is abstracted via DIP (no concrete I/O dependency), configuration is injected from the outside (DI-friendly), and the concrete printer implementation lives in the consuming application (SoC). |

---

## Summary

| Decision | Outcome |
|---|---|
| ADR-1: Separator | Configurable, default: space |
| ADR-2: .NET Version | .NET 8 (LTS) |
| ADR-3: Printing | Optional Printing project using Dependency Inversion (DIP) |
| ADR-4: Testing | Two unit test projects, one per library |
| ADR-5: Range Boundaries | Inclusive (`[min, max]`) |
| ADR-6: API Design | Immutable configuration object + evaluator constructor injection |
| ADR-7: Error Handling | Standard .NET exceptions, fail-fast; empty rules valid |
| ADR-8: Solution Structure | 5 projects (Core, Printing, Sample, and two test projects) |
| ADR-9: Rule Extensibility | Concrete `DivisorRule` only; YAGNI over OCP |
| ADR-10: Interfaces | Concrete classes for pure logic, interface only for I/O boundary |

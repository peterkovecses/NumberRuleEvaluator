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

**The solution provides both evaluation and optional printing coordination via a separate coordination component using the Dependency Inversion Principle.**

The solution defines an output abstraction (e.g., `IResultPrinter`). The coordination component depends on this abstraction, while the consuming application provides the concrete implementation (e.g., `ConsolePrinter` in the Sample project). Printing is optional — the library can be used for evaluation only, without providing a printer.

### Rationale

- **Printing was part of the original business requirement.** `IResultPrinter` is introduced not because the library owns presentation responsibility, but because printing was an explicit business requirement and DIP allows supporting it without introducing concrete I/O dependencies.
- **Printing is a presentation concern**, which belongs to the consuming application. Dependency Inversion resolves this cleanly: the library defines the abstraction, the consumer provides the implementation. The evaluated result is optionally delegated to an injected output abstraction.
- **The library remains reusable**: it has no dependency on `System.Console` or any other output target. Any implementation (console, file, logging framework) can be plugged in.
- **Printing is optional**: consumers who only need evaluation can use the `Evaluate` method directly, without providing a printer implementation.

---

## ADR-4: Testing Strategy

### Context

We need to determine the testing structure: how many test projects, what types of tests, and whether component-level testing is necessary.

### Considered Options

| Option | Pros | Cons |
|---|---|---|
| **Single test project, unit tests** | Simple structure, fits the size of the library | — |
| Unit + component tests | Testing the interaction between components | Does not provide significant additional value in this scope |

### Decision

**A single test project with unit tests.**

### Rationale

- Although the library now includes an optional printing coordination layer (see ADR-3), the interaction between the evaluator and the printer is a single method call forwarding a string. This interaction can be verified with a test double implementing `IResultPrinter` in unit tests.
- Component-level testing does not provide significant additional value in this scope.
- A single test project with unit tests is fully sufficient.
- xUnit, FluentAssertions, and Theories will be used as specified.

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

## ADR-6: API Design — Configuration and Evaluator Structure

### Context

The specification does not define how rules and the valid range should be passed to the evaluator. This API design decision needs to be made.

### Considered Options

| Option | Pros | Cons |
|---|---|---|
| Builder pattern | Fluent, readable configuration | Excessive complexity for this number of configuration parameters |
| Multiple constructor parameters | Simple | The number of parameters might grow |
| **Configuration record + constructor** | Clean, immutable, easy to document, testable | — |
| Fluent API on the evaluator | "Nice" syntax | Mutable state, more complex lifecycle |

### Decision

**Configuration record (or class) + constructor injection.**

### Proposed API Draft

```csharp
var config = new NumberRuleEvaluatorConfig
{
    Range = new NumberRange(14, 72),
    Rules =
    [
        new DivisorRule(3, "Peter"),
        new DivisorRule(5, "Jeffrey")
    ],
    Separator = " " // optional, default: " "
};

var evaluator = new NumberRuleEvaluator(config);
string result = evaluator.Evaluate(15); // → "Jeffrey Peter"
```

> **Note on printing**: The optional printing coordination (see ADR-3) must not be placed directly on the `NumberRuleEvaluator` class (e.g., as an `EvaluateAndPrint` method), as that would mix evaluation and output responsibilities. A separate coordination component will handle the evaluate-then-print workflow. The exact implementation approach will be determined during implementation based on the final design.

### Rationale

- **Immutability**: The configuration does not change after construction, providing thread safety and predictability.
- **Simplicity**: No builder is needed — the configuration object can be directly initialized using object initializer syntax.
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
| Duplicate divisor in a rule list | `ArgumentException` |
| Zero or negative divisor | `ArgumentOutOfRangeException` |
| Null configuration | `ArgumentNullException` |

### Rationale

- Standard .NET exceptions are self-describing and familiar to .NET developers.
- Custom exception classes (e.g., `NumberOutOfRangeException`) would introduce unnecessary complexity — standard exceptions with parameterized messages provide sufficient context.
- **Empty rule list is valid**: the specification states "any number of rules" — zero is a valid count. The behavior is well-defined: every number in range returns itself as a string. Throwing an exception here would violate the principle of least astonishment.
- Validation runs in the constructor (configuration) and in the `Evaluate` method (input) — fail-fast principle.

---

## ADR-8: Solution Structure

### Context

The structure of the solution and projects needs to be defined.

### Decision

**Three projects, simple and flat structure:**

```text
NumberRuleEvaluator.sln
├── src/
│   ├── NumberRuleEvaluator.Core/    # The class library
│   └── NumberRuleEvaluator.Sample/  # Sample console application
└── tests/
    └── NumberRuleEvaluator.Tests/   # Test project
```

### Rationale

- The `src/` and `tests/` separation is a conventional .NET project structure.
- The class library (`.Core`) and the sample console app (`.Sample`) are in separate projects, ensuring the library can be referenced independently.
- A single test project (`.Tests`), consistent with the ADR-4 decision.

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
| ADR-3: Printing | Optional printing via Dependency Inversion (DIP) |
| ADR-4: Testing | Single test project, unit tests |
| ADR-5: Range Boundaries | Inclusive (`[min, max]`) |
| ADR-6: API Design | Configuration record + constructor injection |
| ADR-7: Error Handling | Standard .NET exceptions, fail-fast; empty rules valid |
| ADR-8: Solution Structure | 3 projects (library, sample, tests) |

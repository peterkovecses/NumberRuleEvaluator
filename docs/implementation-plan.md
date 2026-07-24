# Implementation Plan — NumberRuleEvaluator

## Purpose

This plan implements the requirements and architecture decisions in:

- [Design Review Prompt](./design-review-prompt.md)
- [Architecture Decision Record](./architecture-decisions.md)

Production work is intentionally divided into small, reviewable phases. Each phase is completed, reviewed, and merged before the next phase starts.

## Confirmed Decisions

| Topic | Decision |
|---|---|
| Target framework | .NET 8 |
| Numeric type | `int` |
| Valid range | Inclusive minimum and maximum |
| Rule model | Concrete `DivisorRule`; no generic rule abstraction |
| Multi-match output | Alphabetically sorted rule texts joined with a configurable separator |
| Default separator | Single space (`" "`) |
| Printing | Optional output orchestration through a separate Printing library and output abstraction |
| Tests | Two xUnit test projects using FluentAssertions, one per library |
| Errors | Standard .NET argument exceptions |
| Documentation | XML documentation on all public Core- and Printing-library members |

## Solution Structure

```text
NumberRuleEvaluator.sln
├── src/
│   ├── NumberRuleEvaluator.Core/
│   │   ├── Configuration/
│   │   └── Evaluation/
│   ├── NumberRuleEvaluator.Printing/ # Optional output orchestration class library
│   │   ├── Abstractions/
│   │   │   └── IResultPrinter.cs
│   │   └── NumberRulePrintCoordinator.cs
│   └── NumberRuleEvaluator.Sample/
│       └── ConsolePrinter.cs
└── tests/
    ├── NumberRuleEvaluator.Core.Tests/
    │   ├── Configuration/
    │   └── Evaluation/
    └── NumberRuleEvaluator.Printing.Tests/
```

### Project Responsibilities

| Project | Type | Responsibility |
|---|---|---|
| `NumberRuleEvaluator.Core` | Class library | Reusable configuration, validation, and evaluation. It has no printing or concrete I/O dependency. |
| `NumberRuleEvaluator.Printing` | Class library | Optional adapter/orchestration package. It owns the output abstraction and evaluate-then-output workflow, references Core, and has no concrete I/O dependency. It contains only output abstraction and orchestration logic; it must not contain any concrete output implementation. |
| `NumberRuleEvaluator.Sample` | Console application | Demonstrates library consumption and provides the concrete `ConsolePrinter` presentation adapter. |
| `NumberRuleEvaluator.Core.Tests` | Test project | Unit tests for Core configuration and evaluation behavior. |
| `NumberRuleEvaluator.Printing.Tests` | Test project | Unit tests for Printing coordination behavior using an `IResultPrinter` test double. |

### Dependencies

```text
NumberRuleEvaluator.Printing       ──> NumberRuleEvaluator.Core
NumberRuleEvaluator.Sample         ──> NumberRuleEvaluator.Core
NumberRuleEvaluator.Sample         ──> NumberRuleEvaluator.Printing
NumberRuleEvaluator.Core.Tests     ──> NumberRuleEvaluator.Core
NumberRuleEvaluator.Printing.Tests ──> NumberRuleEvaluator.Printing ──> NumberRuleEvaluator.Core
```

Core has no dependency on Printing, Sample, or test projects. Printing depends only on Core.

## Public API Proposal

The final namespaces and exact class names may be refined during implementation, while preserving these responsibilities.

| Type | Responsibility |
|---|---|
| `NumberRange` | Represents an inclusive integer range and validates that its minimum does not exceed its maximum. |
| `DivisorRule` | Represents one positive divisor-to-text mapping. |
| `NumberRuleEvaluatorConfig` | Holds immutable range, rules, and separator configuration. Its constructor validates all configuration state, creates a private copy of the supplied rules (for example, with `ToArray()`), and exposes that copy as `IReadOnlyList<DivisorRule>`. |
| `NumberRuleEvaluator` | Receives immutable configuration through its constructor, validates input, and evaluates configured divisor rules. |
| `IResultPrinter` | Output port owned by the Printing library for the output orchestration workflow. The consuming application provides the concrete presentation adapter. |
| `NumberRulePrintCoordinator` | Printing-library component that coordinates evaluation followed by optional output through `IResultPrinter`, without adding I/O responsibility to Core. |

Example consumer API:

```csharp
var configuration = new NumberRuleEvaluatorConfig(
    range: new NumberRange(14, 72),
    rules:
    [
        new DivisorRule(3, "Peter"),
        new DivisorRule(5, "Jeffrey")
    ],
    separator: " ");

var evaluator = new NumberRuleEvaluator(configuration);
var result = evaluator.Evaluate(15); // "Jeffrey Peter"
```

The printer abstraction is intentionally kept because printing was part of the original requirement. The separate Printing library owns the evaluate-and-output orchestration workflow and its output port, without introducing printing or concrete I/O dependencies into Core.

## Validation Rules

| Condition | Outcome |
|---|---|
| Evaluator configuration is `null` | `ArgumentNullException` |
| Rule collection is `null` | `ArgumentNullException` |
| A rule is `null` | `ArgumentNullException` |
| Minimum is greater than maximum | `ArgumentException` |
| Divisor is zero or negative | `ArgumentOutOfRangeException` |
| Duplicate divisor | `ArgumentException`; a divisor uniquely identifies a rule, and multiple texts for the same divisor would make evaluation behavior ambiguous |
| Rule text is empty or whitespace | `ArgumentException` |
| Separator is `null` | `ArgumentNullException` |
| Separator is empty | Valid; matching texts are concatenated directly |
| Rule collection is empty | Valid; each in-range number evaluates to itself |
| Evaluated number is outside the inclusive range | `ArgumentOutOfRangeException` |

`NumberRuleEvaluatorConfig` validates all configuration-related values in its constructor, including a `null` rule collection. The evaluator only validates its own constructor argument and numbers supplied to `Evaluate`.

## Evaluation Behavior

1. Validate that the supplied number is within the configured inclusive range.
2. Find all rules whose divisor divides the number without a remainder.
3. Sort the matching rule texts using ordinal alphabetical ordering.
4. Join matches with the configured separator.
5. Return the joined text, or the input number formatted using invariant culture as a string if no rule matched.

## Testing Strategy

Use xUnit, FluentAssertions, the AAA pattern, and the `Method_When_Should` naming convention. Use theories where parameterization improves coverage. XML documentation must cover public API types, constructors, public methods, and important properties; it should explain purpose or behavior rather than repeat member names.

| Area | Required coverage |
|---|---|
| Range | Inclusive boundaries; invalid range construction |
| Rules | Positive divisors; invalid divisors; duplicate divisors; invalid texts |
| Evaluation | Single match; multiple matches; alphabetical ordering; custom and empty separators; no matches; empty rules |
| Input validation | Below-minimum and above-maximum inputs |
| Print coordination | `NumberRuleEvaluator.Printing.Tests` verifies that the coordinator forwards the evaluated result to an `IResultPrinter` test double |
| Sample | Covered by build validation and a manual smoke run that verifies its documented example produces the expected console output. This is demo and documentation validation, not an automated test. |

## Implementation Phases

### Phase 1 — Solution and Project Structure

**Branch:** `feature/number-rule-evaluator-phase-01-structure`  
**Pull request:** One PR for this phase

Create the solution and all five projects:

- `NumberRuleEvaluator.sln`
- `src/NumberRuleEvaluator.Core`
- `src/NumberRuleEvaluator.Printing`
- `src/NumberRuleEvaluator.Sample`
- `tests/NumberRuleEvaluator.Core.Tests`
- `tests/NumberRuleEvaluator.Printing.Tests`

Configure the projects for .NET 8, nullable reference types, and implicit usings. Add a reference from Printing to Core; references from Sample to Core and Printing; and references from each test project to its corresponding library. Add xUnit and FluentAssertions to both test projects. Do not add production behavior in this phase; create the Printing source folders and files in Phase 3 with their first implementations.

**Review focus:** solution layout, project boundaries, package choices, and build configuration.

### Phase 2 — Core Evaluation and Validation

**Branch:** `feature/number-rule-evaluator-phase-02-core`  
**Pull request:** One PR, based on the merged Phase 1 result

Implement the configuration value types and `NumberRuleEvaluator`. Enforce every validation rule listed above. Implement deterministic evaluation, alphabetical ordering, separator handling, and fallback number formatting. Add XML documentation for all public Core types and members.

Add unit tests alongside the implementation of each behavior.

This phase completes the Core public API. Printing remains an optional, separate library added in Phase 3.

**Review focus:** API clarity, immutable configuration, exception contracts, deterministic ordering, and unit-test completeness.

### Phase 3 — Optional Output Orchestration and Sample Application

**Branch:** `feature/number-rule-evaluator-phase-03-printing-sample`  
**Pull request:** One PR, based on the merged Phase 2 result

Create `NumberRuleEvaluator.Printing` as the optional adapter/orchestration package, with `IResultPrinter` and `NumberRulePrintCoordinator`. The coordinator evaluates through `NumberRuleEvaluator` and delegates the result to the injected output adapter. Implement the concrete `ConsolePrinter` presentation adapter solely in the Sample project, then demonstrate a representative configuration and output workflow. The Printing project remains independent from any concrete I/O technology.

Add unit tests in `NumberRuleEvaluator.Printing.Tests` using an in-memory printer test double to verify that the coordinator forwards the evaluated result exactly once. Run the Sample manually as a smoke test and verify that its documented example writes the expected output; this validates the demo and documentation only, not the automated test suite.

**Review focus:** the Printing output-orchestration API, separation of evaluation from I/O, dependency direction, and the Sample's usefulness as executable documentation.

### Phase 4 — README and Release Hardening

**Branch:** `feature/number-rule-evaluator-phase-04-docs-hardening`  
**Pull request:** One PR, based on the merged Phase 3 result

Create the repository README with:

- Project overview and features
- .NET 8 installation and reference guidance
- Basic evaluation example
- Range, rules, and separator configuration examples
- Optional output-orchestration example based on the Sample application
- Test and build instructions

Run the complete build and test suite. Resolve compiler warnings, documentation inaccuracies, and test gaps discovered during this final pass.

**Review focus:** public-facing documentation accuracy, warning-free build, and final behavioral coverage.

## Git and Review Workflow

Direct commits to `master` are not recommended for this implementation. Use one feature branch and one pull request per phase so that each change set remains small, independently reviewable, and easy to revert.

1. Create the phase branch from the latest merged `master`.
2. Implement only the phase scope.
3. Run the relevant tests, then the full solution build and test suite before requesting review.
4. Use concise Conventional Commit messages.
5. Review and approve the pull request.
6. Merge the phase before starting its dependent successor.

No parallel implementation is required: each phase depends on the API and behavior established by the preceding phase. This avoids overlapping edits, reduces merge conflicts, and preserves clear code-review boundaries.

## Completion Criteria

The implementation is complete when:

- All four phases have been merged through reviewed pull requests.
- The Core library is independently reusable and has no printing or concrete console dependency.
- The optional Printing output-orchestration library depends only on Core and has no concrete console dependency.
- The Sample application demonstrates the documented public API.
- All public Core- and Printing-library members have XML documentation.
- The solution builds without compiler warnings.
- The full automated test suite passes.

# NumberRuleEvaluator

A small .NET 8 library for evaluating a number against a set of divisor-to-text rules — inspired by the
classic FizzBuzz problem, generalized to any inclusive range, any number of rules, and a configurable
separator for multiple matches.

## Features

- **Configurable inclusive range** — numbers outside the configured `[minimum, maximum]` range are rejected.
- **Any number of divisor rules** — each rule maps a positive divisor to a text (e.g., `3 -> "Peter"`).
- **Deterministic multi-match output** — when multiple rules match, their texts are sorted alphabetically
  (ordinal) and joined with a configurable separator.
- **Fallback formatting** — if no rule matches, the number itself is returned, formatted using invariant culture.
- **Immutable, validated configuration** — all configuration state is validated up front and copied
  defensively; the evaluator itself only validates its own constructor argument and the numbers it
  evaluates.
- **Standard argument validation** — invalid input raises clear, standard .NET exceptions
  (`ArgumentException`, `ArgumentOutOfRangeException`, `ArgumentNullException`).
- **Optional output orchestration** — a separate `NumberRuleEvaluator.Printing` library can coordinate
  evaluation with output, without adding any I/O dependency to the Core library.
- **Fully documented public API** — every public Core and Printing member has XML documentation.

## Getting Started

Requires the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later.

Add a project reference to the Core library:

```bash
dotnet add reference path/to/src/NumberRuleEvaluator.Core/NumberRuleEvaluator.Core.csproj
```

If you also want the optional output-orchestration workflow, reference the Printing library as well:

```bash
dotnet add reference path/to/src/NumberRuleEvaluator.Printing/NumberRuleEvaluator.Printing.csproj
```

## Basic Evaluation Example

```csharp
using NumberRuleEvaluator.Core.Configuration;
using CoreEvaluator = NumberRuleEvaluator.Core.Evaluation.NumberRuleEvaluator;

var configuration = new NumberRuleEvaluatorConfig(
    range: new NumberRange(14, 72),
    rules:
    [
        new DivisorRule(3, "Peter"),
        new DivisorRule(5, "Jeffrey")
    ],
    separator: " ");

var evaluator = new CoreEvaluator(configuration);

evaluator.Evaluate(15); // "Jeffrey Peter" (divisible by both 3 and 5)
evaluator.Evaluate(18); // "Peter"         (divisible by 3 only)
evaluator.Evaluate(16); // "16"            (no rule matches; falls back to the number itself)
```

## Range, Rules, and Separator Configuration

All range, rule, and separator validation happens once, up front, in the `NumberRuleEvaluatorConfig`
constructor. `NumberRuleEvaluator` itself only validates its own constructor argument and the number
passed to `Evaluate`.

### Range

`NumberRange` represents an inclusive range and validates that the minimum does not exceed the maximum:

```csharp
var range = new NumberRange(minimum: 1, maximum: 100);
range.Contains(50);  // true
range.Contains(101); // false
```

Evaluating a number outside the configured range throws `ArgumentOutOfRangeException`.

### Rules

`DivisorRule` represents a single positive divisor-to-text mapping. The divisor must be positive, and
the text must not be null, empty, or whitespace. Duplicate divisors are rejected when building the
configuration, since a divisor uniquely identifies a rule:

```csharp
var rules = new[]
{
    new DivisorRule(3, "Peter"),
    new DivisorRule(5, "Jeffrey")
};
```

An empty rule collection is valid — every in-range number then evaluates to itself.

### Separator

The separator is used to join the texts of all matching rules, sorted alphabetically (ordinal):

```csharp
// Default single-space separator
new NumberRuleEvaluatorConfig(range, rules, separator: " ");   // "Jeffrey Peter"

// Custom separator
new NumberRuleEvaluatorConfig(range, rules, separator: "-");   // "Jeffrey-Peter"

// Empty separator concatenates matches directly
new NumberRuleEvaluatorConfig(range, rules, separator: "");    // "JeffreyPeter"
```

## Optional Output-Orchestration Example

`NumberRuleEvaluator.Printing` provides `NumberRulePrintCoordinator`, which evaluates a number with a
`NumberRuleEvaluator` and forwards the result to an injected `IResultPrinter`. The Printing library owns
only the orchestration and the output port — it has no concrete I/O dependency; you supply an
`IResultPrinter` implementation. The Sample application provides an example implementation with its own
`ConsolePrinter` adapter, which simply writes the result to the console:

```csharp
using NumberRuleEvaluator.Core.Configuration;
using NumberRuleEvaluator.Printing;
using NumberRuleEvaluator.Printing.Abstractions;
using CoreEvaluator = NumberRuleEvaluator.Core.Evaluation.NumberRuleEvaluator;

var configuration = new NumberRuleEvaluatorConfig(
    range: new NumberRange(14, 72),
    rules:
    [
        new DivisorRule(3, "Peter"),
        new DivisorRule(5, "Jeffrey")
    ],
    separator: " ");

var evaluator = new CoreEvaluator(configuration);

// Any IResultPrinter implementation you provide, e.g. a console adapter or another
// application-specific output adapter.
IResultPrinter printer = new YourResultPrinter(); // implement IResultPrinter for your target output
var coordinator = new NumberRulePrintCoordinator(evaluator, printer);

coordinator.EvaluateAndPrint(15); // Evaluates 15 and forwards "Jeffrey Peter" to `printer`
```

The Sample application's `ConsolePrinter` is one such adapter; you can run it directly to see the full
workflow in action:

```bash
dotnet run --project src/NumberRuleEvaluator.Sample
```

## Building and Testing

Build the whole solution:

```bash
dotnet build
```

Run the full automated test suite:

```bash
dotnet test
```

The solution treats compiler warnings as errors (`Directory.Build.props`), so a successful build implies
a warning-free one.

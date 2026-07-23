# Dynamic Number Evaluation and Printing Library (NumberRuleEvaluator)

## Background

The goal is to design a reusable .NET class library that evaluates numbers based on configurable rules and returns (and optionally prints) the corresponding output.

At this stage, **do not generate any production code**.

The objective is to:

1. Review the specification.
2. Identify ambiguities or missing requirements.
3. Discuss every open architectural decision.
4. Recommend the best approach for each decision with clear reasoning.
5. After all decisions have been finalized, produce a detailed implementation plan.

Do **not** assume that my proposed approaches are correct. Challenge them where appropriate and recommend a simpler or more maintainable design if one exists.

---

# Functional Requirements

## Reusable Class Library

The solution must be implemented as a reusable .NET Class Library that can be referenced by other .NET applications.

---

## Configurable Number Range

The client application must be able to configure the valid input range.

Example:

- Minimum: 14
- Maximum: 72

Values outside the configured range are considered invalid.

---

## Configurable Rules

The client application must be able to configure any number of divisor-to-text mappings.

Examples:

- 3 → "Peter"
- 5 → "Jeffrey"

The number of rules is unlimited.

---

# Evaluation Logic

The evaluation method must behave as follows.

## 1. Range validation

If the input number is outside the configured range, an appropriate exception must be thrown.

---

## 2. Rule evaluation

If the number is divisible by one or more configured divisors, the corresponding text values are returned.

---

## 3. Multiple matches

If multiple rules match, all matching text values must be returned on a single line.

The returned values must be sorted alphabetically.

---

## 4. Default behavior

If the number is inside the valid range but no rule matches, the method returns the original number as a string.

---

# Printing

The original interview exercise required printing the result to the console.

The follow-up assignment, however, explicitly requires creating a reusable class library.

Since a reusable library should not depend directly on console output, the printing functionality requires an architectural decision.

---

# Technical Requirements

## Architecture

The solution should follow:

- SOLID principles
- Separation of Concerns
- Clean Code principles

Keep the design intentionally simple.

Avoid introducing unnecessary frameworks, patterns or abstractions unless they clearly improve the solution.

---

## Automated Testing

Use:

- xUnit
- AAA pattern
- Method_When_Should naming convention
- FluentAssertions
- Theory instead of Fact where parameterization provides value

---

## Modern C#

Write clean, readable and maintainable C# using modern language features where appropriate.

---

## Sample Application

The solution should contain a small Console Application demonstrating how the library is consumed.

This application will also provide sample code for the README.

---

## Documentation

Provide a professional README including:

- project overview
- features
- installation
- usage examples
- configuration examples

---

## XML Documentation

Public classes and public members should contain XML documentation comments.

---

# Open Design Decisions

Please review each decision independently.

For every item:

- explain the trade-offs
- recommend the preferred approach
- justify the recommendation

Also identify any additional architectural decisions that should be made before implementation begins.

---

## Decision 1

The specification requires multiple matching rule names to be returned on a single line in alphabetical order.

The separator is unspecified.

Possible approaches include:

- space
- comma
- configurable separator (with a sensible default)
- another alternative

Recommend the most appropriate approach.

---

## Decision 2

Select the target .NET version(s).

The goal is to maximize compatibility across modern .NET applications while avoiding unnecessary limitations.

Recommend the best target framework strategy.

---

## Decision 3

Should the library expose only evaluation functionality, or should it also include a printing workflow?

One possible design is to keep printing optional by applying the Dependency Inversion Principle:

```text
Core Library
    |
    | depends on abstraction
    ↓
IPrinter

Client Application
    |
    | provides implementation
    ↓
ConsolePrinter
```

Evaluate this approach against the alternative of exposing evaluation only.

Recommend the preferred design.

---

## Decision 4

Determine the optimal testing strategy.

Topics to evaluate:

- single vs multiple test projects
- unit tests only
- whether component-level tests provide sufficient value for this solution

Recommend the most appropriate testing structure.

---

# Review Checklist

Before producing an implementation plan, verify:

- Is the specification complete?
- Are there ambiguous requirements?
- Are there conflicting requirements?
- Is any part of the proposed design unnecessarily complex?
- Can the design be simplified while remaining extensible?
- Is the design aligned with SOLID and Separation of Concerns?

After all architectural decisions have been finalized, produce a detailed implementation plan.

The implementation plan should include:

- solution structure
- project structure
- folder structure
- public API proposal
- class responsibilities
- dependency graph
- testing strategy
- implementation phases
- recommended implementation order

Do not generate production code until explicitly requested.
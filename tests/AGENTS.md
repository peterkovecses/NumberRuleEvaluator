# Test Writing Guidelines

Guidelines for writing tests in this repository's test projects.

## Variable Naming

Use `expected` for the expected value and `actual` for the result being tested.

## Method Naming

Use the `MethodName_WhenScenario_ShouldExpectedResult` pattern (e.g., `TryDeserialize_WhenValidJsonAndTypeIsPassed_ShouldDeserialize`).

Omit the `WhenScenario` part (e.g., `MethodName_ShouldExpectedResult`) if there is no specific scenario or condition being tested. Avoid filler like `WhenCalled`.

## AAA Pattern

Always follow the Arrange, Act, Assert pattern. Do not include any other structural comments.

## Constants in Arrange

Always use constants or well-named variables for magic strings, numbers, or other literal values within the Arrange phase.

## Exception Assertions

Assert only on the exception type, not the message.

## Test Scope

Only test classes that contain logic. Never test simple properties or plain data objects.

## Project Structure

Test projects must mirror the source project structure.

## Constructors

Use primary constructors in classes for injecting dependencies.

**Exception:** Use a traditional constructor if member initialization (e.g., the SUT) requires referencing other fields or involves logic that cannot be expressed cleanly within a primary constructor.

## Data-Driven Tests

Use `[Theory]` when the expected behavior applies to multiple different inputs.

Note: If a test requires if-else or conditional logic within the Assert phase (e.g., in a Theory), prefer splitting it into separate `[Fact]` methods to keep assertions explicit and focused.

## xUnit v3 Features

- Prefer `TheoryDataRow<...>` or tuples over `object[]` for strongly typed theory data.
- Use `TheoryDataRow` metadata properties (e.g., `Skip`, `Timeout`, `DisplayName`) or fluent methods (e.g., `.WithSkip(...)`) when needed.
- Use `MatrixTheoryData<...>` to automatically generate combinations of multiple data sets (2-5 sets).
- Leverage async `MemberData` when data retrieval requires asynchronous operations.

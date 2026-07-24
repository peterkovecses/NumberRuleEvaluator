# AGENTS.md

Guidelines for AI agents (and contributors) working in this repository.

## Git Workflow

- Use **Conventional Commits** for all commit messages (e.g., `feat:`, `fix:`, `chore:`, `refactor:`, `test:`, `docs:`).
- Keep commit messages concise and to the point — summarize the essential change, avoid unnecessary detail.
- Before committing, run the existing test suite (if tests exist). Only commit if **all tests pass**.
- Do not commit while there are unresolved **compiler warnings**. Resolve all warnings first.
- Committing always requires **explicit human approval** — never commit automatically without the user's confirmation.
- Always squash a pull request's commits into a single commit before/at merge.

## C# Code Style

### Return Formatting
If a method contains logic before the `return` statement, always include a blank line before the `return`.

### Early Return
Prefer early returns and avoid `else` or `else if` branches to reduce nesting.

### Null Checks
Always use `is null` instead of `== null`, and `is not null` instead of `!= null`.

**Exception:** Within expression trees (e.g., `Expression<Func<...>>`), use `== null` and `!= null`, since the `is` pattern matching operator is not supported there.

### XML Documentation
Public library code (classes, interfaces, methods, properties, and other public members) must have XML documentation comments.

### GlobalUsings Rules

- Use a `GlobalUsings.cs` file in each project when a namespace is needed across a significant portion of that project's files.
- Only put stable, general-purpose dependencies in global usings: `System.*`, commonly used framework namespaces, and in test projects `Xunit`, `FluentAssertions`, etc.
- Keep project- or feature-specific namespaces as local `using` directives.
- Do not use global usings to hide dependencies or shorten a rarely used reference.
- In library projects, avoid excessive global usings, since readability and dependency visibility of public code matters.
- If a namespace is only needed in a few files, always use a local `using` directive instead.

Note: All projects target .NET 8 with `<ImplicitUsings>enable</ImplicitUsings>`, so the SDK already makes the most common framework namespaces (e.g., `System`, `System.Collections.Generic`, `System.Linq`, `System.Threading.Tasks`) global automatically. `GlobalUsings.cs` files should therefore stay focused only on external packages (e.g., test libraries) not already covered by implicit usings.

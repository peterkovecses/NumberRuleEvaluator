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

# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

All `dotnet` commands must be run from the `./src` directory.

```bash
dotnet restore
dotnet build --no-restore -warnaserror
dotnet format --verify-no-changes             # check code style (CI enforces this)
dotnet format                                 # auto-fix code style
dotnet test --no-build --verbosity normal     # run integration tests (requires Docker for Testcontainers)
dotnet pack --configuration Release -p:PackageVersion=<version> --output .
```

## Architecture

This is a **PostgreSQL storage implementation** for the Pure relational schema ecosystem. It has no public abstractions of its own — it provides concrete types that implement interfaces from `Pure.RelationalSchema.Storage`.

**Key relationships:**

- `PostgreSqlCreatedSchema` wraps an `ISchema` and executes DDL (CREATE SCHEMA, CREATE TABLE, CREATE INDEX, ALTER TABLE … ADD CONSTRAINT) lazily via `SchemaCreationStatement` on first property access. The schema and table names in PostgreSQL are derived from hash codes (via `Pure.RelationalSchema.HashCodes`) rather than the model names, to avoid length and character constraints.
- `PostgreSqlStoredSchemaDataSet` is a dictionary from `ITable` → `IStoredTableDataSet`, where each entry is a `PostgreSqlStoredTableDataSet` backed by a `SELECT *` query.
- `PostgreSqlStoredTableDataSet` implements both `IQueryable<IRow>` and `IAsyncEnumerable<IRow>` via `RowsEnumerable` / `RowsAsyncEnumerable`.
- `PostgreSqlStoredSchemaDataSetWithInsertedRows` and `PostgreSqlStoredTableDataSetWithInsertedData` execute `INSERT INTO` statements and combine inserted rows with existing ones.
- All SQL statement builders (`SchemaCreationStatement`, `TableCreationStatement`, `IndexCreationStatement`, `ForeignKeyCreationStatement`, `InsertStatement`, `SelectAllStatement`, etc.) are **internal** and compose SQL as `IString` values using `Pure.Primitives.String.Operations` combinators.

**Testing:** Integration tests use `Testcontainers.PostgreSql` to spin up a real PostgreSQL instance. There are no unit tests with mocked databases. CI runs mutation testing via `dotnet-stryker` at `Complete` level with a 57% break threshold and requires ≥79% line coverage (warning at 99%).

**Multi-targeting:** net8.0, net9.0, net10.0. `IsAotCompatible` is explicitly `false`.

**Package validation:** `EnablePackageValidation = true` with `PackageValidationBaselineVersion = 0.1.0-preview.7.0.1`. Breaking API changes fail the build.

**Publishing:** triggered by pushing a semver tag matching `*.*.*`. The tag name becomes the `PackageVersion`. Packages are published to both GitHub Packages and NuGet.org.

## Code Style

Enforced via `.editorconfig` and `dotnet format --verify-no-changes` in CI:

- No `var` — always use explicit types
- No expression-bodied methods or constructors — properties, indexers, accessors, and lambdas may be expression-bodied
- File-scoped namespace declarations
- Private fields prefixed with `_` (underscore camelCase)
- No non-private instance fields
- `using` directives outside the namespace, System directives sorted first
- Max line length: 90 characters
- Braces on new lines (`csharp_new_line_before_open_brace = all`)
- No implicit object creation (`new()` without type) when the type is apparent

## Commit Messages

Do not mention Claude or AI assistance in commit messages.

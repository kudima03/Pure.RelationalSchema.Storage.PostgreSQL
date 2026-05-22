# Pure.RelationalSchema.Storage.PostgreSQL

PostgreSQL storage implementation for the **Pure** relational schema ecosystem — creates schemas, manages tables, and reads/writes rows via `IDbConnection`.

[![.NET build & test](https://github.com/kudima03/Pure.RelationalSchema.Storage.PostgreSQL/actions/workflows/build-and-test.yml/badge.svg?branch=main)](https://github.com/kudima03/Pure.RelationalSchema.Storage.PostgreSQL/actions/workflows/build-and-test.yml)
[![publish NuGet](https://github.com/kudima03/Pure.RelationalSchema.Storage.PostgreSQL/actions/workflows/publish-nuget.yml/badge.svg?branch=main)](https://github.com/kudima03/Pure.RelationalSchema.Storage.PostgreSQL/actions/workflows/publish-nuget.yml)
[![NuGet](https://img.shields.io/nuget/v/Pure.RelationalSchema.Storage.PostgreSQL)](https://www.nuget.org/packages/Pure.RelationalSchema.Storage.PostgreSQL)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## Overview

`Pure.RelationalSchema.Storage.PostgreSQL` bridges the abstract relational model defined in `Pure.RelationalSchema` with a live PostgreSQL database. It provides:

- **DDL execution** — `PostgreSqlCreatedSchema` lazily generates and executes `CREATE SCHEMA`, `CREATE TABLE`, `CREATE INDEX`, and `ALTER TABLE … ADD CONSTRAINT` statements derived from an `ISchema`.
- **Row access** — `PostgreSqlStoredTableDataSet` exposes table rows as both `IQueryable<IRow>` and `IAsyncEnumerable<IRow>` via `SELECT *` queries.
- **Row insertion** — `PostgreSqlStoredSchemaDataSetWithInsertedRows` / `PostgreSqlStoredTableDataSetWithInsertedData` execute `INSERT INTO` statements and surface the inserted rows alongside existing ones.

Schema and table identifiers are derived from hash codes (via `Pure.RelationalSchema.HashCodes`) to avoid name-length and special-character constraints in PostgreSQL.

## Public API

| Type | Kind | Description |
|------|------|-------------|
| `IPostgreSqlStoredSchemaDataSet` | interface | Extends `IStoredSchemaDataSet` with an `IDbConnection Connection` property |
| `PostgreSqlStoredSchemaDataSet` | record | Maps each `ITable` in an `ISchema` to a `PostgreSqlStoredTableDataSet`; implements `IPostgreSqlStoredSchemaDataSet` |
| `PostgreSqlStoredSchemaDataSetWithInsertedRows` | record | Wraps an `IPostgreSqlStoredSchemaDataSet` and inserts grouped `IRow` data into the corresponding table datasets |
| `PostgreSqlCreatedSchema` | record | `ISchema` decorator that executes DDL lazily on first property access |
| `PostgreSqlStoredTableDataSet` | record | `IStoredTableDataSet` backed by a PostgreSQL table; supports sync and async enumeration |
| `PostgreSqlStoredTableDataSetWithInsertedData` | record | Wraps a table dataset and inserts rows on first enumeration |
| `PostgreSqlColumnTypeName` | class | Maps Pure schema column types to PostgreSQL type name strings |

## Dependencies

- [`Pure.RelationalSchema.Storage`](https://github.com/kudima03/Pure.RelationalSchema.Storage/tree/0.1.0-preview.7.0.0) — storage abstractions (`IStoredSchemaDataSet`, `IStoredTableDataSet`, `IRow`) that this package implements
- [`Pure.RelationalSchema`](https://github.com/kudima03/Pure.RelationalSchema/tree/2.0.0) — core relational model abstractions: `ISchema`, `ITable`, `IForeignKey`, and related types
- [`Pure.RelationalSchema.HashCodes`](https://github.com/kudima03/Pure.RelationalSchema.HashCodes/tree/3.2.0) — hash code derivation for schema/table/column identifiers used as PostgreSQL object names
- [`Pure.RelationalSchema.Storage.HashCodes`](https://github.com/kudima03/Pure.RelationalSchema.Storage.HashCodes/tree/0.1.0-preview.2.1.0) — hash codes for storage-layer types
- [`Pure.Primitives.Abstractions`](https://github.com/kudima03/Pure.Primitives.Abstractions/tree/4.3.0) — base interfaces for primitive types (`IString`, `IBool`, `INumber`, etc.)
- [`Pure.Primitives`](https://github.com/kudima03/Pure.Primitives/tree/3.6.3) — concrete primitive type implementations
- [`Pure.Primitives.String.Operations`](https://github.com/kudima03/Pure.Primitives.String.Operations/tree/1.4.1) — string composition utilities (`JoinedString`, `ConcatenatedString`, `WrappedString`, etc.) used to build SQL statements
- [`Pure.Primitives.Choices`](https://github.com/kudima03/Pure.Primitives.Choices/tree/1.2.0) — discriminated union / choice types
- [`Pure.Primitives.Switches`](https://github.com/kudima03/Pure.Primitives.Switches/tree/1.1.0) — switch / pattern matching utilities
- [`Pure.Collections.Generic`](https://github.com/kudima03/Pure.Collections.Generic/tree/0.1.0-preview.3.0.0) — generic collection implementations (e.g. `Dictionary<TKey1, TKey2, TValue>` with dual-key lookup)

## Target Frameworks

- .NET 8
- .NET 9
- .NET 10

## Installation

```
dotnet add package Pure.RelationalSchema.Storage.PostgreSQL
```

## Usage

```csharp
using System.Data;
using Npgsql;
using Pure.RelationalSchema.Storage.PostgreSQL;

// schema is your ISchema; connection is an open IDbConnection
IDbConnection connection = new NpgsqlConnection(connectionString);
connection.Open();

// Execute DDL lazily on first access
ISchema createdSchema = new PostgreSqlCreatedSchema(schema, connection);

// Build a dataset backed by the created schema
IPostgreSqlStoredSchemaDataSet dataset =
    new PostgreSqlStoredSchemaDataSet(createdSchema, connection);

// Read rows asynchronously from a table
IStoredTableDataSet tableDataset = dataset[myTable];
await foreach (IRow row in tableDataset)
{
    object value = row.Cells[myColumn].Value;
}

// Insert rows grouped by table
IEnumerable<IGrouping<ITable, IRow>> rowGroups = rows.GroupBy(r => r.Table);
IPostgreSqlStoredSchemaDataSet withInserted =
    new PostgreSqlStoredSchemaDataSetWithInsertedRows(dataset, rowGroups);
```

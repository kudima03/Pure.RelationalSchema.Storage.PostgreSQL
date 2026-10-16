# Pure.RelationalSchema.Storage.PostgreSQL

PostgreSQL storage for the **Pure** relational schema ecosystem. This package has been **split into four packages**; `Pure.RelationalSchema.Storage.PostgreSQL` is now a meta-package that contains no code and references all of them, and this repository aggregates their sources as submodules.

[![.NET build](https://github.com/kudima03/Pure.RelationalSchema.Storage.PostgreSQL/actions/workflows/build.yml/badge.svg?branch=main)](https://github.com/kudima03/Pure.RelationalSchema.Storage.PostgreSQL/actions/workflows/build.yml)
[![NuGet](https://img.shields.io/nuget/v/Pure.RelationalSchema.Storage.PostgreSQL)](https://www.nuget.org/packages/Pure.RelationalSchema.Storage.PostgreSQL)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## Packages

| Package | Repository | Contents |
|---------|------------|----------|
| [`Pure.RelationalSchema.Storage.PostgreSQL.Abstractions`](https://www.nuget.org/packages/Pure.RelationalSchema.Storage.PostgreSQL.Abstractions) | [source](https://github.com/kudima03/Pure.RelationalSchema.Storage.PostgreSQL.Abstractions) | `IPostgreSqlStoredSchemaDataSet` |
| [`Pure.RelationalSchema.Storage.PostgreSQL.Creation`](https://www.nuget.org/packages/Pure.RelationalSchema.Storage.PostgreSQL.Creation) | [source](https://github.com/kudima03/Pure.RelationalSchema.Storage.PostgreSQL.Creation) | `PostgreSqlCreatedSchema` (DDL) |
| [`Pure.RelationalSchema.Storage.PostgreSQL.Querying`](https://www.nuget.org/packages/Pure.RelationalSchema.Storage.PostgreSQL.Querying) | [source](https://github.com/kudima03/Pure.RelationalSchema.Storage.PostgreSQL.Querying) | `PostgreSqlStoredSchemaDataSet`, `PostgreSqlStoredTableDataSet` |
| [`Pure.RelationalSchema.Storage.PostgreSQL.Insertion`](https://www.nuget.org/packages/Pure.RelationalSchema.Storage.PostgreSQL.Insertion) | [source](https://github.com/kudima03/Pure.RelationalSchema.Storage.PostgreSQL.Insertion) | `PostgreSqlStoredSchemaDataSetWithInsertedRows` |

Installing `Pure.RelationalSchema.Storage.PostgreSQL` pulls in all four. To take only what you need, reference the individual packages instead. Insertion and Querying depend on Abstractions; they do not depend on each other or on Creation.

## Migrating from 0.1.0-preview.8.x

The last release that contained code is `0.1.0-preview.8.1.2`. Every public type moved to a new namespace, so after updating the package reference you must change your `using` directives:

| Old type | New namespace |
|----------|---------------|
| `IPostgreSqlStoredSchemaDataSet` | `Pure.RelationalSchema.Storage.PostgreSQL.Abstractions` |
| `PostgreSqlCreatedSchema` | `Pure.RelationalSchema.Storage.PostgreSQL.Creation` |
| `PostgreSqlStoredSchemaDataSet`, `PostgreSqlStoredTableDataSet` | `Pure.RelationalSchema.Storage.PostgreSQL.Querying` |
| `PostgreSqlStoredSchemaDataSetWithInsertedRows` | `Pure.RelationalSchema.Storage.PostgreSQL.Insertion` |

Replace `using Pure.RelationalSchema.Storage.PostgreSQL;` with the namespaces above.

**Behaviour change:** asynchronous enumeration in Querying (`await foreach` over a `PostgreSqlStoredTableDataSet`) no longer disposes the `IDbConnection` you passed in. Previously the connection was closed after the first async enumeration; you now own its lifetime.

## Usage

```csharp
using System.Data;
using Pure.RelationalSchema.Abstractions.Schema;
using Pure.RelationalSchema.Abstractions.Table;
using Pure.RelationalSchema.Storage.Abstractions;
using Pure.RelationalSchema.Storage.PostgreSQL.Abstractions;
using Pure.RelationalSchema.Storage.PostgreSQL.Creation;
using Pure.RelationalSchema.Storage.PostgreSQL.Insertion;
using Pure.RelationalSchema.Storage.PostgreSQL.Querying;

// schema is your ISchema; connection is an open IDbConnection
ISchema createdSchema = new PostgreSqlCreatedSchema(schema, connection);

IPostgreSqlStoredSchemaDataSet dataset =
    new PostgreSqlStoredSchemaDataSet(createdSchema, connection);

IStoredTableDataSet tableDataset = dataset[myTable];
await foreach (IRow row in tableDataset)
{
    // ...
}

// rowGroups: IEnumerable<IGrouping<ITable, IRow>>
IPostgreSqlStoredSchemaDataSet withInserted =
    new PostgreSqlStoredSchemaDataSetWithInsertedRows(dataset, rowGroups);
```

## Working on the packages together

The package repositories are included as submodules under `modules/`, pinned to their released tags, and `src/Pure.RelationalSchema.Storage.PostgreSQL.slnx` opens all of them in one solution.

```
git clone --recurse-submodules https://github.com/kudima03/Pure.RelationalSchema.Storage.PostgreSQL
cd Pure.RelationalSchema.Storage.PostgreSQL/src
dotnet build
```

Changes to a package are made in its own repository and released there. Dependabot bumps the submodule pointers and the meta-package's package references. Tests run in each package's repository CI.

## Target Frameworks

- .NET 8
- .NET 9
- .NET 10

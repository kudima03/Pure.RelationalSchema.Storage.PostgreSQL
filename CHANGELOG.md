# Changelog

All notable changes to Pure.RelationalSchema.Storage.PostgreSQL are documented here.

Format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

---

## [0.1.0-preview.8.1.2] — 2026-08-04

- Maintenance release: dependency and build updates.

## [0.1.0-preview.8.1.1] — 2026-06-25

- Maintenance release: dependency and build updates.

## [0.1.0-preview.8.1.0] — 2026-06-08

- Maintenance release: dependency and build updates.

## [0.1.0-preview.8.0.0] — 2026-06-07

- Maintenance release: dependency and build updates.

## [0.1.0-preview.7.0.1] — 2026-05-07

- Maintenance release: dependency and build updates.

## [0.1.0-preview.7.0.0] — 2026-03-14

### Fixed

- Index names generated during schema creation now include a table-specific
  prefix (each segment truncated to fit PostgreSQL's identifier length
  limit), preventing name collisions between indexes on different tables.

## [0.1.0-preview.6.1.0] — 2026-03-11

### Added

- Column type mapping support for `double`, `float`, and `uuid` column
  types.

## [0.1.0-preview.6.0.0] — 2026-03-11

### Added

- The package now multi-targets `net8.0`, `net9.0`, and `net10.0`
  (previously `net9.0` only).

### Changed

- **Breaking:** Table creation no longer auto-generates an implicit
  primary key column, and `INSERT` statements no longer auto-compute a
  hash value for it. `PrimaryKeyStatement` and
  `PrimaryColumnCreationStatement` have been removed.

## [0.1.0-preview.5.2.1] — 2025-11-19

### Fixed

- Corrected the order of operations during row enumeration so that column
  metadata is captured before the query executes.

## [0.1.0-preview.5.2.0] — 2025-11-19

### Fixed

- The column list is now read once per query instead of being
  re-enumerated for every row, avoiding inconsistent results if the
  underlying column sequence yielded different values across
  enumerations.

## [0.1.0-preview.5.1.0] — 2025-11-18

### Changed

- Reverted the eager row-materialization fix introduced in
  `0.1.0-preview.5.0.1`, restoring lazy evaluation of per-table inserted
  rows.

## [0.1.0-preview.5.0.1] — 2025-11-17

### Fixed

- Table row data during schema-level insertion was lazily re-evaluated
  multiple times, which could produce incorrect results; rows are now
  materialized eagerly.

## [0.1.0-preview.5.0.0] — 2025-11-04

- Maintenance release: dependency and build updates.

## [0.1.0-preview.4.0.2] — 2025-10-28

### Fixed

- Query results are now fully read before the underlying data reader is
  released, preventing issues when the result enumerable is iterated
  more than once.

## [0.1.0-preview.4.0.1] — 2025-10-24

### Fixed

- Byte array (`bytea`) column values are now correctly decoded as text
  instead of being mis-formatted.

## [0.1.0-preview.4.0.0] — 2025-10-17

### Fixed

- Rows already present in a table dataset are no longer re-inserted when
  inserting the same data again; previously only duplicates within the
  new batch were skipped.
- Values read from PostgreSQL are now parsed using the invariant culture,
  fixing incorrect formatting on systems with non-English locale
  settings.

## [0.1.0-preview.3.0.0] — 2025-10-16

### Changed

- **Breaking:** `PostgreSqlStoredSchemaDataSet.TablesDatasets` property
  removed; `PostgreSqlStoredSchemaDataSet` now directly implements
  `IReadOnlyDictionary<ITable, IStoredTableDataSet>` (indexer, `Keys`,
  `Values`, `Count`, enumeration) instead of exposing it as a separate
  property.

## [0.1.0-preview.2.1.2] — 2025-10-12

### Fixed

- Corrected table ordering in
  `PostgreSqlStoredSchemaDataSetWithInsertedRows` so tables are
  enumerated in insertion order rather than the underlying dataset's
  iteration order.

## [0.1.0-preview.2.1.1] — 2025-10-12

### Fixed

- Table insertion order is now preserved by using an ordered dictionary
  internally in `PostgreSqlStoredSchemaDataSetWithInsertedRows`.

## [0.1.0-preview.2.1.0] — 2025-10-12

- Maintenance release: dependency and build updates.

## [0.1.0-preview.2.0.0] — 2025-10-10

### Added

- `PostgreSqlStoredSchemaDataSetWithInsertedRows` for tracking inserted
  rows across all of a schema's tables.

### Changed

- **Breaking:** `IPostgreSqlStoredTableDataSet` renamed to
  `IPostgreSqlStoredSchemaDataSet`.

## [0.1.0-preview.1.1.0] — 2025-09-29

### Added

- Boolean column type support.

## [0.1.0-preview.1.0.0] — 2025-09-29

### Added

- Primary key column creation as part of generated table DDL.
- `DeterminedHashColumnType` column type support.

### Changed

- Inserting rows now skips duplicates within the same batch.

### Fixed

- Corrected a wrong PostgreSQL type name in the column type mapping.

## [0.1.0-preview.0.4.0] — 2025-09-22

- Maintenance release: dependency and build updates.

## [0.1.0-preview.0.3.0] — 2025-09-11

### Added

- `PostgreSqlStoredSchemaDataSet` and `PostgreSqlCreatedSchema`,
  providing full relational schema creation (tables, foreign keys,
  indexes) with `IF NOT EXISTS` idempotence.
- `InsertStatement` and `PostgreSqlStoredTableDataSetWithInsertedRows`
  for inserting rows into a table.
- `SelectAllStatement` backing row retrieval.
- `IPostgreSqlStoredTableDataSet` interface exposing `SchemaName` and
  `Connection`.

### Fixed

- Rows were read using each column's raw name instead of its
  hash-derived physical name, causing wrong column mapping in both
  synchronous and asynchronous enumeration.
- `ArgumentException` thrown when accessing an inserted-rows dataset
  before insertion now carries a descriptive message.

## [0.1.0-preview.0.2.0] — 2025-08-21

- Maintenance release: dependency and build updates.

## [0.1.0-preview.0.1.0] — 2025-08-13

### Added

- Initial release: `PostgreSqlStoredTableDataSet`, an
  `IStoredTableDataSet` implementation backed by PostgreSQL (via
  `IDbConnection`), providing LINQ query support (`IQueryable<IRow>`)
  and asynchronous row enumeration (`IAsyncEnumerable<IRow>`).

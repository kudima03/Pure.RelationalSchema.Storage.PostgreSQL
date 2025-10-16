using Pure.Collections.Generic;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.Table;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Storage.Abstractions;
using Pure.RelationalSchema.Storage.PostgreSQL.Tests.Fakes;

namespace Pure.RelationalSchema.Storage.PostgreSQL.Tests;

public sealed record PostgreSqlStoredTableDataSetWithInsertedRowsTests
    : IAsyncLifetime,
        IDisposable
{
    private DatabaseFixture? _fixture;

    [Fact]
    public void InsertMultipleRows()
    {
        IPostgreSqlStoredSchemaDataSet schema = new PostgreSqlStoredSchemaDataSet(
            _fixture!.Schema,
            _fixture.Connection
        );

        IEnumerable<IGrouping<ITable, IRow>> rows = schema.Keys.Select(
            x => new Grouping(
                x,
                Enumerable
                    .Range(0, 100)
                    .Select(_ => new Row(
                        new Dictionary<IColumn, IColumn, ICell>(
                            x.Columns,
                            x => x,
                            x => new Cell(new RandomValueForColumnType(x.Type)),
                            x => new ColumnHash(x)
                        )
                    ))
            )
        );

        IStoredSchemaDataSet dataSetWithInsertedRows =
            new PostgreSqlStoredSchemaDataSetWithInsertedRows(
                _fixture!.Schema,
                schema,
                rows
            );

        Assert.Equal(
            200,
            dataSetWithInsertedRows.SelectMany(x => x.Value).Count()
        );
    }

    [Fact]
    public void InsertOnlyDistinctRows()
    {
        IPostgreSqlStoredSchemaDataSet schema = new PostgreSqlStoredSchemaDataSet(
            _fixture!.Schema,
            _fixture.Connection
        );

        IEnumerable<IGrouping<ITable, IRow>> rows = schema.Keys.Select(
            x => new Grouping(
                x,
                Enumerable
                    .Range(0, 100)
                    .Select(_ => new Row(
                        new Dictionary<IColumn, IColumn, ICell>(
                            x.Columns,
                            x => x,
                            x => new Cell(new DefaultValueForColumnType(x.Type)),
                            x => new ColumnHash(x)
                        )
                    ))
            )
        );

        IStoredSchemaDataSet dataSetWithInsertedRows =
            new PostgreSqlStoredSchemaDataSetWithInsertedRows(
                _fixture!.Schema,
                schema,
                rows
            );

        Assert.Equal(
            1,
            dataSetWithInsertedRows
                .Select(x => x.Value.Count())
                .Distinct()
                .Single()
        );
    }

    public void Dispose()
    {
        _fixture?.Dispose();
    }

    public Task DisposeAsync()
    {
        Dispose();
        return Task.CompletedTask;
    }

    public Task InitializeAsync()
    {
        _fixture = new DatabaseFixture();
        return Task.CompletedTask;
    }
}

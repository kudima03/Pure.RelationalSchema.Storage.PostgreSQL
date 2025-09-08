using Pure.Collections.Generic;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Storage.Abstractions;
using Pure.RelationalSchema.Storage.PostgreSQL.Tests.Fakes;

namespace Pure.RelationalSchema.Storage.PostgreSQL.Tests;

public sealed record PostgreSqlStoredTableDataSetWithInsertedRowsTests
    : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public PostgreSqlStoredTableDataSetWithInsertedRowsTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void InsertMultipleRows()
    {
        IPostgreSqlStoredTableDataSet schema = new PostgreSqlStoredTableDataSet(
            _fixture.Schema.Tables.First(),
            _fixture.Schema,
            _fixture.Connection
        );

        IEnumerable<IRow> rowsToInsert = Enumerable
            .Range(0, 100)
            .Select(_ => new Row(
                new Dictionary<IColumn, IColumn, ICell>(
                    schema.TableSchema.Columns,
                    x => x,
                    x => new Cell(new RandomValueForColumnType(x.Type)),
                    x => new ColumnHash(x)
                )
            ));

        IPostgreSqlStoredTableDataSet dataSetWithInsertedRows =
            new PostgreSqlStoredTableDataSetWithInsertedRows(schema, rowsToInsert);

        Assert.Equal(100, dataSetWithInsertedRows.Count());
    }
}

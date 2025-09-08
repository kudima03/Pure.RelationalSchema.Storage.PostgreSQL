using System.Collections;
using Pure.Collections.Generic;
using Pure.Primitives.Abstractions.Char;
using Pure.Primitives.Abstractions.Number;
using Pure.Primitives.Abstractions.String;
using Pure.Primitives.Random.Date;
using Pure.Primitives.Random.Number;
using Pure.Primitives.Switches.String;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.ColumnType;
using Pure.RelationalSchema.ColumnType;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Storage.Abstractions;
using Char = Pure.Primitives.Char.Char;
using String = Pure.Primitives.String.String;

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
    public void EnumeratesAsTyped()
    {
        IPostgreSqlStoredTableDataSet schema = new PostgreSqlStoredTableDataSet(
            _fixture.Schema.Tables.First(),
            _fixture.Schema,
            _fixture.Connection
        );

        IEnumerable<IRow> rowsToInsert =
        [
            new Row(
                new Dictionary<IColumn, IColumn, ICell>(
                    schema.TableSchema.Columns,
                    x => x,
                    x => new Cell(
                        
                    ),
                    x => new ColumnHash(x)
                )
            ),
        ];

        IPostgreSqlStoredTableDataSet dataSetWithInsertedRows =
            new PostgreSqlStoredTableDataSetWithInsertedRows(schema, rowsToInsert);

        Assert.Equal(1, dataSetWithInsertedRows.Count());
    }
}

internal sealed record RandomValueForColumnType : IString
{
    private readonly IColumnType _columnType;

    public RandomValueForColumnType(IColumnType columnType)
    {
        _columnType = columnType;
    }

    public string TextValue =>
        new StringSwitch<IColumnType>(
            _columnType,
            [
                new KeyValuePair<IColumnType, IString>(
                    new IntColumnType(),
                    ((INumber<int>)new RandomInt()).NumberValue.
                ),
                new KeyValuePair<IColumnType, IString>(
                    new LongColumnType(),
                    new String("bigint")
                ),
                new KeyValuePair<IColumnType, IString>(
                    new UShortColumnType(),
                    new String("integer")
                ),
                new KeyValuePair<IColumnType, IString>(
                    new UIntColumnType(),
                    new String("bigint")
                ),
                new KeyValuePair<IColumnType, IString>(
                    new ULongColumnType(),
                    new String("numeric")
                ),
                new KeyValuePair<IColumnType, IString>(
                    new StringColumnType(),
                    new String("text")
                ),
                new KeyValuePair<IColumnType, IString>(
                    new DateColumnType(),
                    new String("date")
                ),
                new KeyValuePair<IColumnType, IString>(
                    new DateTimeColumnType(),
                    new String("timestamp")
                ),
                new KeyValuePair<IColumnType, IString>(
                    new TimeColumnType(),
                    new String("time")
                ),
            ],
            x => new ColumnTypeHash(x)
        ).TextValue;

    public IEnumerator<IChar> GetEnumerator()
    {
        return TextValue.Select(x => new Char(x)).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

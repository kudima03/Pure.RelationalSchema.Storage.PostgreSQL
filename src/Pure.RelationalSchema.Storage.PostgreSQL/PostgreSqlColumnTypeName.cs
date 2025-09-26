using System.Collections;
using Pure.HashCodes;
using Pure.Primitives.Abstractions.Char;
using Pure.Primitives.Abstractions.String;
using Pure.Primitives.Switches.String;
using Pure.RelationalSchema.Abstractions.ColumnType;
using Pure.RelationalSchema.ColumnType;
using Pure.RelationalSchema.HashCodes;
using Char = Pure.Primitives.Char.Char;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.Storage.PostgreSQL;

internal sealed record PostgreSqlColumnTypeName : IString
{
    private readonly IColumnType _columnType;

    public PostgreSqlColumnTypeName(IColumnType columnType)
    {
        _columnType = columnType;
    }

    public string TextValue =>
        new StringSwitch<IColumnType>(
            _columnType,
            [
                new KeyValuePair<IColumnType, IString>(
                    new IntColumnType(),
                    new String("integer")
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
                new KeyValuePair<IColumnType, IString>(
                    new DeterminedHashColumnType(),
                    new String("bytea(32)")
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

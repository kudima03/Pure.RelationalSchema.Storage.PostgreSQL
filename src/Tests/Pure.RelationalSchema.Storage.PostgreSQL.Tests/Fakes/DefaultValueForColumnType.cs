using System.Collections;
using Pure.Primitives.Abstractions.Char;
using Pure.Primitives.Abstractions.String;
using Pure.Primitives.Date;
using Pure.Primitives.Switches.String;
using Pure.Primitives.Time;
using Pure.RelationalSchema.Abstractions.ColumnType;
using Pure.RelationalSchema.ColumnType;
using Pure.RelationalSchema.HashCodes;
using Char = Pure.Primitives.Char.Char;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.Storage.PostgreSQL.Tests.Fakes;

internal sealed record DefaultValueForColumnType : IString
{
    private readonly IColumnType _columnType;

    public DefaultValueForColumnType(IColumnType columnType)
    {
        _columnType = columnType;
    }

    public string TextValue =>
        new StringSwitch<IColumnType>(
            _columnType,
            [
                new KeyValuePair<IColumnType, IString>(
                    new IntColumnType(),
                    new String(int.MaxValue.ToString())
                ),
                new KeyValuePair<IColumnType, IString>(
                    new LongColumnType(),
                    new String(uint.MaxValue.ToString())
                ),
                new KeyValuePair<IColumnType, IString>(
                    new UShortColumnType(),
                    new String(ushort.MaxValue.ToString())
                ),
                new KeyValuePair<IColumnType, IString>(
                    new UIntColumnType(),
                    new String(uint.MaxValue.ToString())
                ),
                new KeyValuePair<IColumnType, IString>(
                    new ULongColumnType(),
                    new String(ulong.MaxValue.ToString())
                ),
                new KeyValuePair<IColumnType, IString>(
                    new StringColumnType(),
                    new String("Hello, world!")
                ),
                new KeyValuePair<IColumnType, IString>(
                    new DateColumnType(),
                    new String(new Date(new DateOnly(1970, 1, 1)))
                ),
                new KeyValuePair<IColumnType, IString>(
                    new DateTimeColumnType(),
                    new String(
                        new Primitives.DateTime.DateTime(
                            new Date(new DateOnly(1970, 1, 1)),
                            new Time(new TimeOnly(23, 59, 59))
                        )
                    )
                ),
                new KeyValuePair<IColumnType, IString>(
                    new TimeColumnType(),
                    new String(new Time(new TimeOnly(23, 59, 59)))
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

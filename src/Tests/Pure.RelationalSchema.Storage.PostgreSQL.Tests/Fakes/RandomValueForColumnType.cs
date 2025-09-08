using System.Collections;
using Pure.Primitives.Abstractions.Char;
using Pure.Primitives.Abstractions.String;
using Pure.Primitives.Random.Date;
using Pure.Primitives.Random.DateTime;
using Pure.Primitives.Random.Number;
using Pure.Primitives.Random.String;
using Pure.Primitives.Random.Time;
using Pure.Primitives.Switches.String;
using Pure.RelationalSchema.Abstractions.ColumnType;
using Pure.RelationalSchema.ColumnType;
using Pure.RelationalSchema.HashCodes;
using Char = Pure.Primitives.Char.Char;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.Storage.PostgreSQL.Tests.Fakes;

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
                    new String(new RandomInt())
                ),
                new KeyValuePair<IColumnType, IString>(
                    new LongColumnType(),
                    new String(new RandomUInt())
                ),
                new KeyValuePair<IColumnType, IString>(
                    new UShortColumnType(),
                    new String(new RandomUShort())
                ),
                new KeyValuePair<IColumnType, IString>(
                    new UIntColumnType(),
                    new String(new RandomUInt())
                ),
                new KeyValuePair<IColumnType, IString>(
                    new ULongColumnType(),
                    new String(new RandomUInt())
                ),
                new KeyValuePair<IColumnType, IString>(
                    new StringColumnType(),
                    new RandomString(new Char((char)65), new Char((char)91))
                ),
                new KeyValuePair<IColumnType, IString>(
                    new DateColumnType(),
                    new String(new RandomDate())
                ),
                new KeyValuePair<IColumnType, IString>(
                    new DateTimeColumnType(),
                    new String(new RandomDateTime())
                ),
                new KeyValuePair<IColumnType, IString>(
                    new TimeColumnType(),
                    new String(new RandomTime())
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

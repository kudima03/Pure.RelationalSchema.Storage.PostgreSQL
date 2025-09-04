using System.Collections;
using Pure.Primitives.Abstractions.Char;
using Pure.Primitives.Abstractions.String;
using Pure.Primitives.Choices.String;
using Pure.Primitives.String;
using Pure.Primitives.String.Operations;
using Pure.RelationalSchema.Abstractions.Index;
using Pure.RelationalSchema.Abstractions.Table;
using Pure.RelationalSchema.HashCodes;
using Char = Pure.Primitives.Char.Char;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.Storage.PostgreSQL;

internal sealed record IndexCreationStatement : IString
{
    private readonly IIndex _index;

    private readonly ITable _table;

    public IndexCreationStatement(IIndex index, ITable table)
    {
        _index = index;
        _table = table;
    }

    public string TextValue =>
        (
            (IString)
                new WhitespaceJoinedString(
                    new String("CREATE"),
                    new StringChoice(
                        _index.IsUnique,
                        new String("UNIQUE"),
                        new EmptyString()
                    ),
                    new String("INDEX"),
                    new ConcatenatedString(
                        new String("\""),
                        new HexString(new IndexHash(_index)),
                        new String("\"")
                    ),
                    new String("ON"),
                    new ConcatenatedString(
                        new String("\""),
                        new HexString(new TableHash(_table)),
                        new String("\"")
                    ),
                    new ConcatenatedString(
                        new String("("),
                        new JoinedString(
                            new String(", "),
                            _index.Columns.Select(x => new ConcatenatedString(
                                new String("\""),
                                new HexString(new ColumnHash(x)),
                                new String("\"")
                            ))
                        ),
                        new String(");")
                    )
                )
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

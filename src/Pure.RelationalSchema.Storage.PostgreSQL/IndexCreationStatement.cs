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

    private readonly IString _schemaName;

    public IndexCreationStatement(IIndex index, ITable table, IString schemaName)
    {
        _index = index;
        _table = table;
        _schemaName = schemaName;
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
                    new String("INDEX IF NOT EXISTS"),
                    new WrappedString(
                        new DoubleQuoteString(),
                        new TrimmedHash(new HexString(new IndexHash(_index)))
                    ),
                    new String("ON"),
                    new JoinedString(
                        new DotString(),
                        [
                            new WrappedString(new DoubleQuoteString(), _schemaName),
                            new WrappedString(
                                new DoubleQuoteString(),
                                new TrimmedHash(new HexString(new TableHash(_table)))
                            ),
                        ]
                    ),
                    new ConcatenatedString(
                        new LeftRoundBracketString(),
                        new JoinedString(
                            new CommaString(),
                            _index.Columns.Select(x => new WrappedString(
                                new DoubleQuoteString(),
                                new TrimmedHash(new HexString(new ColumnHash(x)))
                            ))
                        ),
                        new ConcatenatedString(
                            new RightRoundBracketString(),
                            new SemicolonString()
                        )
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

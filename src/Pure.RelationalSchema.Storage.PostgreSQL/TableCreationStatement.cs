using System.Collections;
using Pure.Primitives.Abstractions.Char;
using Pure.Primitives.Abstractions.String;
using Pure.Primitives.String;
using Pure.Primitives.String.Operations;
using Pure.RelationalSchema.Abstractions.Table;
using Pure.RelationalSchema.HashCodes;
using Char = Pure.Primitives.Char.Char;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.Storage.PostgreSQL;

internal sealed record TableCreationStatement : IString
{
    private readonly IString _schemaName;

    private readonly ITable _table;

    public TableCreationStatement(IString schemaName, ITable table)
    {
        _schemaName = schemaName;
        _table = table;
    }

    public string TextValue =>
        (
            (IString)
                new NewLineJoinedString(
                    new TableCreationHeaderStatement(
                        _schemaName,
                        new TrimmedHash(new HexString(new TableHash(_table)))
                    ),
                    new WrappedString(
                        new LeftRoundBracketString(),
                        new JoinedString(
                            new ConcatenatedString(
                                new CommaString(),
                                new NewLineString()
                            ),
                            _table.Columns.Select(x => new ColumnCreationStatement(x))
                        ),
                        new ConcatenatedString(
                            new RightRoundBracketString(),
                            new String(";")
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

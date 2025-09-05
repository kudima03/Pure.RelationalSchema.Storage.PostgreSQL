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

internal sealed record PostgreSqlTableCreationStatement : IString
{
    private readonly ITable _table;

    public PostgreSqlTableCreationStatement(ITable table)
    {
        _table = table;
    }

    public string TextValue =>
        (
            (IString)
                new NewLineJoinedString(
                    new TableCreationHeaderStatement(
                        new HexString(new TableHash(_table))
                    ),
                    new LeftRoundBracketString(),
                    new JoinedString(
                        new ConcatenatedString(new CommaString(), new NewLineString()),
                        _table.Columns.Select(x => new ColumnCreationStatement(x))
                    ),
                    new ConcatenatedString(new RightRoundBracketString(), new String(";"))
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

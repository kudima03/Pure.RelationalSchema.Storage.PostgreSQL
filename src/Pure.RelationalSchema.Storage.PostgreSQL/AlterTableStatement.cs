using System.Collections;
using Pure.Primitives.Abstractions.Char;
using Pure.Primitives.Abstractions.String;
using Pure.Primitives.String.Operations;
using Pure.RelationalSchema.Abstractions.Table;
using Pure.RelationalSchema.HashCodes;
using Char = Pure.Primitives.Char.Char;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.Storage.PostgreSQL;

internal sealed record AlterTableStatement : IString
{
    private readonly IString _tableName;

    public AlterTableStatement(ITable table)
        : this(new HexString(new TableHash(table))) { }

    public AlterTableStatement(IString tableName)
    {
        _tableName = tableName;
    }

    public string TextValue =>
        (
            (IString)
                new WhitespaceJoinedString(
                    new String("ALTER TABLE"),
                    new ConcatenatedString(new String("\""), _tableName, new String("\""))
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

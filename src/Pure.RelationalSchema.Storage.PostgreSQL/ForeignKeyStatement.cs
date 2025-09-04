using System.Collections;
using Pure.Primitives.Abstractions.Char;
using Pure.Primitives.Abstractions.String;
using Pure.Primitives.String.Operations;
using Pure.RelationalSchema.Abstractions.ForeignKey;
using Pure.RelationalSchema.HashCodes;
using Char = Pure.Primitives.Char.Char;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.Storage.PostgreSQL;

internal sealed record ForeignKeyStatement : IString
{
    private readonly IForeignKey _foreignKey;

    public ForeignKeyStatement(IForeignKey foreignKey)
    {
        _foreignKey = foreignKey;
    }

    public string TextValue =>
        (
            (IString)
                new WhitespaceJoinedString(
                    new String("FOREIGN KEY"),
                    new ConcatenatedString(
                        new String("(\""),
                        new HexString(new ColumnHash(_foreignKey.ReferencingColumn)),
                        new String("\")")
                    ),
                    new String("REFERENCES"),
                    new ConcatenatedString(
                        new String("\""),
                        new HexString(new TableHash(_foreignKey.ReferencedTable)),
                        new String("\"")
                    ),
                    new ConcatenatedString(
                        new String("(\""),
                        new HexString(new ColumnHash(_foreignKey.ReferencedColumn)),
                        new String("\");")
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

using System.Collections;
using Pure.Primitives.Abstractions.Char;
using Pure.Primitives.Abstractions.String;
using Pure.Primitives.String;
using Pure.Primitives.String.Operations;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Column;
using Pure.RelationalSchema.HashCodes;
using Char = Pure.Primitives.Char.Char;

namespace Pure.RelationalSchema.Storage.PostgreSQL;

internal sealed record PrimaryColumnCreationStatement : IString
{
    private readonly IColumn _primaryColumn = new RowDeterminedHashColumn();

    public string TextValue =>


                new WhitespaceJoinedString(
                    new WrappedString(
                        new DoubleQuoteString(),
                        new TrimmedHash(new HexString(new ColumnHash(_primaryColumn)))
                    ),
                    new PostgreSqlColumnTypeName(_primaryColumn.Type),
                    new PrimaryKeyStatement()
                )
        .TextValue;

    public IEnumerator<IChar> GetEnumerator()
    {
        return TextValue.Select(x => new Char(x)).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

using System.Collections;
using Pure.Primitives.Abstractions.Char;
using Pure.Primitives.Abstractions.String;
using Pure.Primitives.String;
using Pure.Primitives.String.Operations;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.HashCodes;
using Char = Pure.Primitives.Char.Char;

namespace Pure.RelationalSchema.Storage.PostgreSQL;

internal sealed record ColumnCreationStatement : IString
{
    private readonly IColumn _column;

    public ColumnCreationStatement(IColumn column)
    {
        _column = column;
    }

    public string TextValue =>
        (
            (IString)
                new ConcatenatedString(
                    new HexString(new ColumnHash(_column)),
                    new WhitespaceString(),
                    new PostgreSqlColumnTypeName(_column.Type),
                    new WhitespaceString(),
                    new NotNullStatement()
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

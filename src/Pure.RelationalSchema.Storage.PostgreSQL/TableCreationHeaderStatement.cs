using System.Collections;
using Pure.Primitives.Abstractions.Char;
using Pure.Primitives.Abstractions.String;
using Pure.Primitives.String;
using Pure.Primitives.String.Operations;
using Char = Pure.Primitives.Char.Char;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.Storage.PostgreSQL;

internal sealed record TableCreationHeaderStatement : IString
{
    private readonly IString _tableName;

    public TableCreationHeaderStatement(IString tableName)
    {
        _tableName = tableName;
    }

    public string TextValue =>
        (
            (IString)
                new ConcatenatedString(
                    new String("CREATE TABLE IF NOT EXISTS"),
                    new WhitespaceString(),
                    new String("\""),
                    _tableName,
                    new String("\"")
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

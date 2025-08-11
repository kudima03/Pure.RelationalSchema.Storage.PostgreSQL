using System.Collections;
using Pure.Primitives.Abstractions.Char;
using Pure.Primitives.Abstractions.String;
using Pure.Primitives.Materialized.String;
using Pure.Primitives.String.Operations;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.Storage.PostgreSQL;

public sealed record SelectAllClause : IString
{
    private readonly IString _selectAllClause;

    public SelectAllClause(IString tableName)
    {
        _selectAllClause = new WhitespaceJoinedString(
            new String("SELECT * FROM"),
            new SurroundedString(new String("\""), tableName)
        );
    }

    public string TextValue => new MaterializedString(_selectAllClause).Value;

    public IEnumerator<IChar> GetEnumerator()
    {
        return _selectAllClause.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

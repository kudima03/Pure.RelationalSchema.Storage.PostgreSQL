using System.Collections;
using Pure.Primitives.Abstractions.Char;
using Pure.Primitives.Abstractions.String;
using Pure.Primitives.Materialized.String;
using Pure.Primitives.String.Operations;
using Char = Pure.Primitives.Char.Char;

namespace Pure.RelationalSchema.Storage.PostgreSQL;

public sealed record SurroundedString : IString
{
    private readonly IString _surroundedString;

    public SurroundedString(IString surroundValue, IString value)
        : this(new ConcatenatedString(surroundValue, value, surroundValue)) { }

    private SurroundedString(IString surroundedString)
    {
        _surroundedString = surroundedString;
    }

    public string TextValue => new MaterializedString(_surroundedString).Value;

    public IEnumerator<IChar> GetEnumerator()
    {
        return _surroundedString.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

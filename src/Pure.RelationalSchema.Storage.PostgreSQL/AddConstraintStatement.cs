using System.Collections;
using Pure.Primitives.Abstractions.Char;
using Pure.Primitives.Abstractions.String;
using Pure.Primitives.String;
using Pure.Primitives.String.Operations;
using Char = Pure.Primitives.Char.Char;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.Storage.PostgreSQL;

internal sealed record AddConstraintStatement : IString
{
    private readonly IString _constraintName;

    public AddConstraintStatement(IString constraintName)
    {
        _constraintName = constraintName;
    }

    public string TextValue =>


                new WhitespaceJoinedString(
                    new String("ADD CONSTRAINT"),
                    new WrappedString(new DoubleQuoteString(), _constraintName)
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

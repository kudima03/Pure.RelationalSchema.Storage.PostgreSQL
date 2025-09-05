using System.Collections;
using Pure.Primitives.Abstractions.Char;
using Pure.Primitives.Abstractions.String;
using Pure.Primitives.String;
using Pure.Primitives.String.Operations;
using Pure.RelationalSchema.Abstractions.ForeignKey;
using Pure.RelationalSchema.HashCodes;
using Char = Pure.Primitives.Char.Char;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.Storage.PostgreSQL;

internal sealed record ForeignKeyStatement : IString
{
    private readonly IForeignKey _foreignKey;

    private readonly IString _schemaName;

    public ForeignKeyStatement(IForeignKey foreignKey, IString schemaName)
    {
        _foreignKey = foreignKey;
        _schemaName = schemaName;
    }

    public string TextValue =>
        (
            (IString)
                new WhitespaceJoinedString(
                    new String("FOREIGN KEY"),
                    new WrappedString(
                        new LeftRoundBracketString(),
                        new WrappedString(
                            new DoubleQuoteString(),
                            new TrimmedHash(
                                new HexString(
                                    new ColumnHash(_foreignKey.ReferencingColumn)
                                )
                            )
                        ),
                        new RightRoundBracketString()
                    ),
                    new String("REFERENCES"),
                    new JoinedString(
                        new DotString(),
                        [
                            new WrappedString(new DoubleQuoteString(), _schemaName),
                            new WrappedString(
                                new DoubleQuoteString(),
                                new TrimmedHash(
                                    new HexString(
                                        new TableHash(_foreignKey.ReferencedTable)
                                    )
                                )
                            ),
                        ]
                    ),
                    new ConcatenatedString(
                        new WrappedString(
                            new LeftRoundBracketString(),
                            new WrappedString(
                                new DoubleQuoteString(),
                                new TrimmedHash(
                                    new HexString(
                                        new ColumnHash(_foreignKey.ReferencedColumn)
                                    )
                                )
                            ),
                            new RightRoundBracketString()
                        ),
                        new String(";")
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

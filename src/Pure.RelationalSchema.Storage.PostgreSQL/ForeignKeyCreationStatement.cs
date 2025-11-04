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

internal sealed record ForeignKeyCreationStatement : IString
{
    private readonly IForeignKey _foreignKey;

    private readonly IString _schemaName;

    public ForeignKeyCreationStatement(IForeignKey foreignKey, IString schemaName)
    {
        _foreignKey = foreignKey;
        _schemaName = schemaName;
    }

    public string TextValue =>


                new NewLineJoinedString(
                    new String("DO $$"),
                    new String("BEGIN"),
                    new WhitespaceJoinedString(
                        new String("IF NOT EXISTS"),
                        new WrappedString(
                            new LeftRoundBracketString(),
                            new WhitespaceJoinedString(
                                new String("SELECT"),
                                new JoinedString(
                                    new ConcatenatedString(
                                        new CommaString(),
                                        new WhitespaceString()
                                    ),
                                    [
                                        new String("constraint_schema"),
                                        new String("constraint_name"),
                                    ]
                                ),
                                new String("FROM"),
                                new String("information_schema.table_constraints"),
                                new String("WHERE"),
                                new String("constraint_schema"),
                                new String("="),
                                new WrappedString(
                                    new SingleQuoteString(),
                                    new TrimmedHash(_schemaName)
                                ),
                                new String("AND"),
                                new String("constraint_name"),
                                new String("="),
                                new WrappedString(
                                    new SingleQuoteString(),
                                    new TrimmedHash(
                                        new HexString(new ForeignKeyHash(_foreignKey))
                                    )
                                )
                            ),
                            new RightRoundBracketString()
                        )
                    ),
                    new String("THEN"),
                    new NewLineJoinedString(
                        new AlterTableStatement(
                            _foreignKey.ReferencingTable,
                            _schemaName
                        ),
                        new AddConstraintStatement(
                            new TrimmedHash(
                                new HexString(new ForeignKeyHash(_foreignKey))
                            )
                        ),
                        new ForeignKeyStatement(_foreignKey, _schemaName)
                    ),
                    new ConcatenatedString(new String("END IF"), new SemicolonString()),
                    new ConcatenatedString(new String("END$$"), new SemicolonString())
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

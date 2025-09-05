using System.Collections;
using Pure.Primitives.Abstractions.Char;
using Pure.Primitives.Abstractions.String;
using Pure.Primitives.String.Operations;
using Pure.RelationalSchema.Abstractions.ForeignKey;
using Pure.RelationalSchema.HashCodes;
using Char = Pure.Primitives.Char.Char;

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
        (
            (IString)
                new NewLineJoinedString(
                    new AlterTableStatement(_foreignKey.ReferencingTable, _schemaName),
                    new AddConstraintStatement(
                        new TrimmedHash(new HexString(new ForeignKeyHash(_foreignKey)))
                    ),
                    new ForeignKeyStatement(_foreignKey, _schemaName)
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

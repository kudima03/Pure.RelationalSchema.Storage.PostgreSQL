using System.Collections;
using Pure.Primitives.Abstractions.Char;
using Pure.Primitives.Abstractions.String;
using Pure.Primitives.String;
using Pure.Primitives.String.Operations;
using Pure.RelationalSchema.Abstractions.Table;
using Pure.RelationalSchema.HashCodes;
using Char = Pure.Primitives.Char.Char;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.Storage.PostgreSQL;

internal sealed record AlterTableStatement : IString
{
    private readonly IString _tableName;

    private readonly IString _schemaName;

    public AlterTableStatement(ITable table, IString schemaName)
        : this(new TrimmedHash(new HexString(new TableHash(table))), schemaName) { }

    public AlterTableStatement(IString tableName, IString schemaName)
    {
        _tableName = tableName;
        _schemaName = schemaName;
    }

    public string TextValue =>


                new WhitespaceJoinedString(
                    new String("ALTER TABLE"),
                    new JoinedString(
                        new DotString(),
                        [
                            new WrappedString(new DoubleQuoteString(), _schemaName),
                            new WrappedString(new DoubleQuoteString(), _tableName),
                        ]
                    )
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

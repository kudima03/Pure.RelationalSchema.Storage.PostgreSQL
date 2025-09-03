using System.Collections;
using Pure.Primitives.Abstractions.Char;
using Pure.Primitives.Abstractions.String;
using Pure.Primitives.String;
using Pure.Primitives.String.Operations;
using Pure.RelationalSchema.Abstractions.Schema;
using Char = Pure.Primitives.Char.Char;

namespace Pure.RelationalSchema.Storage.PostgreSQL;

internal sealed record PostgreSqlSchemaCreationStatement : IString
{
    private readonly ISchema _schema;

    public PostgreSqlSchemaCreationStatement(ISchema schema)
    {
        _schema = schema;
    }

    public string TextValue =>
        (
            (IString)
                new JoinedString(
                    new ConcatenatedString(new NewLineString(), new NewLineString()),
                    _schema.Tables.Select(x => new PostgreSqlTableCreationStatement(x))
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

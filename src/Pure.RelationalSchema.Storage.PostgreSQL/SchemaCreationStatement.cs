using System.Collections;
using Pure.Primitives.Abstractions.Char;
using Pure.Primitives.Abstractions.String;
using Pure.Primitives.String;
using Pure.Primitives.String.Operations;
using Pure.RelationalSchema.Abstractions.Schema;
using Pure.RelationalSchema.HashCodes;
using Char = Pure.Primitives.Char.Char;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.Storage.PostgreSQL;

internal sealed record SchemaCreationStatement : IString
{
    private readonly ISchema _schema;

    public SchemaCreationStatement(ISchema schema)
    {
        _schema = schema;
    }

    public string TextValue =>


                new JoinedString(
                    new ConcatenatedString(new NewLineString(), new NewLineString()),
                    [
                        new ConcatenatedString(
                            new WhitespaceJoinedString(
                                new String("CREATE SCHEMA IF NOT EXISTS"),
                                new WrappedString(
                                    new DoubleQuoteString(),
                                    new TrimmedHash(
                                        new HexString(new SchemaHash(_schema))
                                    )
                                )
                            ),
                            new SemicolonString()
                        ),
                        new JoinedString(
                            new ConcatenatedString(
                                new NewLineString(),
                                new NewLineString()
                            ),
                            _schema.Tables.Select(x => new TableCreationStatement(
                                new TrimmedHash(new HexString(new SchemaHash(_schema))),
                                x
                            ))
                        ),
                        new JoinedString(
                            new ConcatenatedString(
                                new NewLineString(),
                                new NewLineString()
                            ),
                            _schema.Tables.SelectMany(x =>
                                x.Indexes.Select(c => new IndexCreationStatement(
                                    c,
                                    x,
                                    new TrimmedHash(
                                        new HexString(new SchemaHash(_schema))
                                    )
                                ))
                            )
                        ),
                        new JoinedString(
                            new ConcatenatedString(
                                new NewLineString(),
                                new NewLineString()
                            ),
                            _schema.ForeignKeys.Select(
                                x => new ForeignKeyCreationStatement(
                                    x,
                                    new TrimmedHash(
                                        new HexString(new SchemaHash(_schema))
                                    )
                                )
                            )
                        ),
                    ]
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

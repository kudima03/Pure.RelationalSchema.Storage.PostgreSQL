using Pure.Primitives.Bool;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.Schema;
using Pure.RelationalSchema.ColumnType;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.Storage.PostgreSQL.Tests;

using Column = Column.Column;
using Index = Index.Index;
using Schema = Schema.Schema;
using Table = Table.Table;

public sealed record PostgreSqlCreatedSchemaTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public PostgreSqlCreatedSchemaTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Works()
    {
        IReadOnlyCollection<IColumn> columns =
        [
            new Column(new String("Column1"), new DateColumnType()),
            new Column(new String("Column2"), new LongColumnType()),
            new Column(new String("Column3"), new StringColumnType()),
            new Column(new String("Column4"), new ULongColumnType()),
        ];

        ISchema schema = new Schema(
            new String("Test"),
            [
                new Table(
                    new String("Test"),
                    columns,
                    [
                        new Index(new True(), columns.Take(2)),
                        new Index(new False(), columns.Skip(2).Take(2)),
                    ]
                ),
            ],
            []
        );

        Assert.NotEmpty(new PostgreSqlCreatedSchema(schema, _fixture.Connection).Name);
    }
}

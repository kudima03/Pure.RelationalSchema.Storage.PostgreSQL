using Pure.Primitives.Bool;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.ForeignKey;
using Pure.RelationalSchema.Abstractions.Schema;
using Pure.RelationalSchema.Abstractions.Table;
using Pure.RelationalSchema.ColumnType;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.Storage.PostgreSQL.Tests;

using Column = Column.Column;
using ForeignKey = ForeignKey.ForeignKey;
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
    public void CreateSchema()
    {
        IReadOnlyCollection<IColumn> columns1 =
        [
            new Column(new String("Column1"), new DateColumnType()),
            new Column(new String("Column2"), new LongColumnType()),
            new Column(new String("Column3"), new StringColumnType()),
            new Column(new String("Column4"), new ULongColumnType()),
        ];

        IReadOnlyCollection<IColumn> columns2 =
        [
            new Column(new String("Column5"), new DateColumnType()),
            new Column(new String("Column6"), new LongColumnType()),
            new Column(new String("Column7"), new TimeColumnType()),
            new Column(new String("Column8"), new IntColumnType()),
        ];

        ITable table1 = new Table(
            new String("Test"),
            columns1,
            [
                new Index(new True(), columns1.Take(1)),
                new Index(new True(), columns1.Skip(1).Take(1)),
                new Index(new False(), columns1.Skip(2).Take(2)),
            ]
        );

        ITable table2 = new Table(
            new String("Test"),
            columns2,
            [
                new Index(new True(), columns2.Take(1)),
                new Index(new True(), columns2.Skip(1).Take(1)),
                new Index(new False(), columns2.Skip(2).Take(2)),
            ]
        );

        IForeignKey foreignKey1 = new ForeignKey(
            table1,
            table1.Columns.First(),
            table2,
            table2.Columns.First()
        );

        IForeignKey foreignKey2 = new ForeignKey(
            table1,
            table1.Columns.Skip(1).First(),
            table2,
            table2.Columns.Skip(1).First()
        );

        ISchema schema = new Schema(
            new String("Test1"),
            [table1, table2],
            [foreignKey1, foreignKey2]
        );

        Assert.NotEmpty(new PostgreSqlCreatedSchema(schema, _fixture.Connection).Name);
    }

    [Fact]
    public void CreateSchemaWithNoColumns()
    {
        ITable table1 = new Table(
            new String("Test"),
            [],
            []
        );

        ITable table2 = new Table(
            new String("Test1"),
            [],
            []
        );

        ISchema schema = new Schema(
            new String("Test2"),
            [table1, table2],
            []
        );

        Assert.NotEmpty(new PostgreSqlCreatedSchema(schema, _fixture.Connection).Name);
    }

    [Fact]
    public void CreateSchemaWithNoIndexes()
    {
        IReadOnlyCollection<IColumn> columns1 =
        [
            new Column(new String("Column1"), new DateColumnType()),
            new Column(new String("Column2"), new LongColumnType()),
            new Column(new String("Column3"), new StringColumnType()),
            new Column(new String("Column4"), new ULongColumnType()),
        ];

        IReadOnlyCollection<IColumn> columns2 =
        [
            new Column(new String("Column5"), new DateColumnType()),
            new Column(new String("Column6"), new LongColumnType()),
            new Column(new String("Column7"), new TimeColumnType()),
            new Column(new String("Column8"), new IntColumnType()),
        ];

        ITable table1 = new Table(
            new String("Test"),
            columns1,
            []
        );

        ITable table2 = new Table(
            new String("Test"),
            columns2,
            []
        );

        ISchema schema = new Schema(
            new String("Test1"),
            [table1, table2],
            []
        );

        Assert.NotEmpty(new PostgreSqlCreatedSchema(schema, _fixture.Connection).Name);
    }

    [Fact]
    public void CreateSchemaWithNoTables()
    {
        ISchema schema = new Schema(
            new String("Test1"),
            [],
            []
        );

        Assert.NotEmpty(new PostgreSqlCreatedSchema(schema, _fixture.Connection).Name);
    }

    [Fact]
    public void CreateSchemaWithNoForeignKeys()
    {
        IReadOnlyCollection<IColumn> columns1 =
        [
            new Column(new String("Column1"), new DateColumnType()),
            new Column(new String("Column2"), new LongColumnType()),
            new Column(new String("Column3"), new StringColumnType()),
            new Column(new String("Column4"), new ULongColumnType()),
        ];

        IReadOnlyCollection<IColumn> columns2 =
        [
            new Column(new String("Column5"), new DateColumnType()),
            new Column(new String("Column6"), new LongColumnType()),
            new Column(new String("Column7"), new TimeColumnType()),
            new Column(new String("Column8"), new IntColumnType()),
        ];

        ITable table1 = new Table(
            new String("Test"),
            columns1,
            [
                new Index(new True(), columns1.Take(1)),
                new Index(new True(), columns1.Skip(1).Take(1)),
                new Index(new False(), columns1.Skip(2).Take(2)),
            ]
        );

        ITable table2 = new Table(
            new String("Test"),
            columns2,
            [
                new Index(new True(), columns2.Take(1)),
                new Index(new True(), columns2.Skip(1).Take(1)),
                new Index(new False(), columns2.Skip(2).Take(2)),
            ]
        );

        ISchema schema = new Schema(
            new String("Test1"),
            [table1, table2],
            []
        );

        Assert.NotEmpty(new PostgreSqlCreatedSchema(schema, _fixture.Connection).Name);
    }

    [Fact]
    public void CreateSchemaOnExisting()
    {
        IReadOnlyCollection<IColumn> columns1 =
        [
            new Column(new String("Column1"), new DateColumnType()),
            new Column(new String("Column2"), new LongColumnType()),
            new Column(new String("Column3"), new StringColumnType()),
            new Column(new String("Column4"), new ULongColumnType()),
        ];

        IReadOnlyCollection<IColumn> columns2 =
        [
            new Column(new String("Column5"), new DateColumnType()),
            new Column(new String("Column6"), new LongColumnType()),
            new Column(new String("Column7"), new TimeColumnType()),
            new Column(new String("Column8"), new IntColumnType()),
        ];

        ITable table1 = new Table(
            new String("Test"),
            columns1,
            [
                new Index(new True(), columns1.Take(1)),
                new Index(new True(), columns1.Skip(1).Take(1)),
                new Index(new False(), columns1.Skip(2).Take(2)),
            ]
        );

        ITable table2 = new Table(
            new String("Test"),
            columns2,
            [
                new Index(new True(), columns2.Take(1)),
                new Index(new True(), columns2.Skip(1).Take(1)),
                new Index(new False(), columns2.Skip(2).Take(2)),
            ]
        );

        IForeignKey foreignKey1 = new ForeignKey(
            table1,
            table1.Columns.First(),
            table2,
            table2.Columns.First()
        );

        IForeignKey foreignKey2 = new ForeignKey(
            table1,
            table1.Columns.Skip(1).First(),
            table2,
            table2.Columns.Skip(1).First()
        );

        ISchema schema = new Schema(
            new String("Test1"),
            [table1, table2],
            [foreignKey1, foreignKey2]
        );

        //_ = new PostgreSqlSchemaCreationStatement(schema).TextValue;
        Assert.NotEmpty(
            new PostgreSqlCreatedSchema(
                new PostgreSqlCreatedSchema(schema, _fixture.Connection),
                _fixture.Connection
            ).Name
        );
    }
}

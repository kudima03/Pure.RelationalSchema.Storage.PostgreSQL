using System.Collections;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Storage.Abstractions;

namespace Pure.RelationalSchema.Storage.PostgreSQL.Tests;

public sealed record PostgreSqlStoredTableDataSetTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public PostgreSqlStoredTableDataSetTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void EnumeratesAsTyped()
    {
        Assert.NotEmpty(
            new PostgreSqlStoredTableDataSet(
                _fixture.Table,
                _fixture.Connection
            ).AsEnumerable()
        );
    }

    [Fact]
    public void EnumeratesAsUntyped()
    {
        ICollection<IRow> list = new List<IRow>();

        IEnumerable rows = new PostgreSqlStoredTableDataSet(
            _fixture.Table,
            _fixture.Connection
        );

        foreach (object obj in rows)
        {
            list.Add((IRow)obj);
        }

        Assert.NotEmpty(list);
    }

    [Fact]
    public async Task EnumeratesAsync()
    {
        Assert.NotEmpty(
            await new PostgreSqlStoredTableDataSet(
                _fixture.Table,
                _fixture.Connection
            ).ToArrayAsync()
        );
    }

    [Fact]
    public void TableSchemaInitializeCorrectly()
    {
        Assert.Equal(
            new TableHash(_fixture.Table),
            new TableHash(
                new PostgreSqlStoredTableDataSet(
                    _fixture.Table,
                    _fixture.Connection
                ).TableSchema
            )
        );
    }
}

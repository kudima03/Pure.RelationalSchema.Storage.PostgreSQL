using System.Data;
using Npgsql;
using Pure.RelationalSchema.Abstractions.Table;
using Pure.RelationalSchema.ColumnType;
using Testcontainers.PostgreSql;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.Storage.PostgreSQL.Tests;

public sealed record DatabaseFixture : IDisposable
{
    private readonly PostgreSqlContainer _postgres;

    public IDbConnection Connection { get; }

    public ITable Table { get; } =
        new Table.Table(
            new String("sample_data"),
            [
                new Column.Column(new String("id"), new IntColumnType()),
                new Column.Column(new String("big_number"), new LongColumnType()),
                new Column.Column(new String("name"), new StringColumnType()),
                new Column.Column(new String("created_date"), new DateColumnType()),
                new Column.Column(new String("created_at"), new TimeColumnType()),
            ],
            []
        );

    public DatabaseFixture()
    {
        _postgres = new PostgreSqlBuilder()
            .WithDatabase("testdb")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

        _postgres.StartAsync().GetAwaiter().GetResult();

        Connection = new NpgsqlConnection(_postgres.GetConnectionString());
        Connection.Open();


        IDbCommand tableCreationCommand = Connection.CreateCommand();
        tableCreationCommand.CommandText = """
            CREATE TABLE sample_data (
                id INT PRIMARY KEY,
                big_number BIGINT NOT NULL,
                name TEXT NOT NULL,
                created_date DATE NOT NULL,
                created_at TIMESTAMP NOT NULL
            );
            """;
        _ = tableCreationCommand.ExecuteScalar();

        IDbCommand tableSeedCommand = Connection.CreateCommand();
        tableSeedCommand.CommandText = """
            INSERT INTO sample_data (id, big_number, name, created_date, created_at) VALUES
            (1, 1234567890123, 'Alice',   '2025-08-01', '2025-08-01 10:15:00'),
            (2, 9876543210987, 'Bob',     '2025-08-02', '2025-08-02 14:30:00'),
            (3, 5555555555555, 'Carol',   '2025-08-03', '2025-08-03 18:45:00'),
            (4, 1111111111111, 'Dave',    '2025-08-04', '2025-08-04 08:00:00'),
            (5, 2222222222222, 'Eve',     '2025-08-05', '2025-08-05 09:15:00'),
            (6, 3333333333333, 'Frank',   '2025-08-06', '2025-08-06 11:30:00'),
            (7, 4444444444444, 'Grace',   '2025-08-07', '2025-08-07 13:45:00'),
            (8, 6666666666666, 'Heidi',   '2025-08-08', '2025-08-08 16:00:00'),
            (9, 7777777777777, 'Ivan',    '2025-08-09', '2025-08-09 18:15:00'),
            (10,8888888888888, 'Judy',    '2025-08-10', '2025-08-10 20:30:00');
            """;

        _ = tableSeedCommand.ExecuteScalar();
    }

    public void Dispose()
    {
        Connection.Dispose();
        _postgres.DisposeAsync().AsTask().GetAwaiter().GetResult();
    }
}

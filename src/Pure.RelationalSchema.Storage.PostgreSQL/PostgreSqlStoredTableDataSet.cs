using System.Collections;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using Pure.Collections.Generic;
using Pure.Primitives.Materialized.String;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.Table;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Storage.Abstractions;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.Storage.PostgreSQL;

public sealed record PostgreSqlStoredTableDataSet : IStoredTableDataSet
{
    private readonly IDbConnection _connection;

    public PostgreSqlStoredTableDataSet(ITable tableSchema, IDbConnection connection)
    {
        TableSchema = tableSchema;
        _connection = connection;
    }

    public ITable TableSchema { get; }

    public Type ElementType { get; }

    public Expression Expression { get; }

    public IQueryProvider Provider { get; }

    public IEnumerator<IRow> GetEnumerator()
    {
        using IDbCommand cmd = _connection.CreateCommand();
        cmd.CommandText =
            $"SELECT * FROM \"{new MaterializedString(TableSchema.Name).Value}\"";
        IDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            yield return MapDataRecordToRow(reader);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public async IAsyncEnumerator<IRow> GetAsyncEnumerator(
        CancellationToken cancellationToken = default
    )
    {
        await using DbConnection conn = (DbConnection)_connection;
        await using DbCommand cmd = conn.CreateCommand();
        cmd.CommandText =
            $"SELECT * FROM \"{new MaterializedString(TableSchema.Name).Value}\"";

        await using DbDataReader reader = await cmd.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            yield return MapDataRecordToRow(reader);
        }
    }

    private IRow MapDataRecordToRow(IDataRecord record)
    {
        IReadOnlyDictionary<string, string> rawCells = TableSchema
            .Columns.Select(x => new MaterializedString(x.Name).Value)
            .ToDictionary(x => x, x => record[x].ToString())!;

        IReadOnlyDictionary<IColumn, ICell> cells = new Dictionary<
            IColumn,
            IColumn,
            ICell
        >(
            TableSchema.Columns,
            x => x,
            x => new Cell(
                new String(rawCells[new MaterializedString(x.Name).Value].ToString())
            ),
            x => new ColumnHash(x)
        );

        return new Row(cells);
    }
}

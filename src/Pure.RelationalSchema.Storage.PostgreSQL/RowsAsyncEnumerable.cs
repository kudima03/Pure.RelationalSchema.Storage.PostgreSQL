using System.Data;
using System.Data.Common;
using Pure.Collections.Generic;
using Pure.Primitives.Materialized.String;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.Table;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Storage.Abstractions;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.Storage.PostgreSQL;

public sealed record RowsAsyncEnumerable : IAsyncEnumerable<IRow>
{
    private readonly IDbConnection _connection;

    private readonly ITable _table;

    public RowsAsyncEnumerable(IDbConnection connection, ITable table)
    {
        _connection = connection;
        _table = table;
    }

    public async IAsyncEnumerator<IRow> GetAsyncEnumerator(
        CancellationToken cancellationToken = default
    )
    {
        await using DbConnection conn = (DbConnection)_connection;
        await using DbCommand cmd = conn.CreateCommand();
        cmd.CommandText =
            $"SELECT * FROM \"{new MaterializedString(_table.Name).Value}\"";

        await using DbDataReader reader = await cmd.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            yield return MapDataRecordToRow(reader);
        }
    }

    private IRow MapDataRecordToRow(IDataRecord record)
    {
        IReadOnlyDictionary<string, string> rawCells = _table
            .Columns.Select(x => new MaterializedString(x.Name).Value)
            .ToDictionary(x => x, x => record[x].ToString())!;

        IReadOnlyDictionary<IColumn, ICell> cells = new Dictionary<
            IColumn,
            IColumn,
            ICell
        >(
            _table.Columns,
            x => x,
            x => new Cell(
                new String(rawCells[new MaterializedString(x.Name).Value].ToString())
            ),
            x => new ColumnHash(x)
        );

        return new Row(cells);
    }
}

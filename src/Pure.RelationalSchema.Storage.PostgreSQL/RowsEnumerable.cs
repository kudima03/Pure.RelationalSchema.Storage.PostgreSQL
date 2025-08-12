using System.Collections;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using Pure.Collections.Generic;
using Pure.Primitives.Abstractions.String;
using Pure.Primitives.Materialized.String;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.Table;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Storage.Abstractions;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.Storage.PostgreSQL;

public sealed record RowsEnumerable : IEnumerable<IRow>
{
    private readonly IDbConnection _connection;

    private readonly ITable _table;

    public RowsEnumerable(IDbConnection connection, ITable table)
    {
        _connection = connection;
        _table = table;
    }

    public IEnumerator<IRow> GetEnumerator()
    {
        using IDbCommand cmd = _connection.CreateCommand();
        cmd.CommandText =
            $"SELECT * FROM \"{new MaterializedString(_table.Name).Value}\"";
        using IDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            IReadOnlyDictionary<string, string> rawCells = _table
                .Columns.Select(x => new MaterializedString(x.Name).Value)
                .ToDictionary(x => x, x => reader[x].ToString())!;

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

            yield return new Row(cells);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

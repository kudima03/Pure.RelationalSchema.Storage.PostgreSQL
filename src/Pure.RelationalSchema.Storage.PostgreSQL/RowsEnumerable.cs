using System.Collections;
using System.Data;
using Pure.Collections.Generic;
using Pure.Primitives.Abstractions.String;
using Pure.Primitives.String.Operations;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.Table;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Storage.Abstractions;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.Storage.PostgreSQL;

internal sealed record RowsEnumerable : IEnumerable<IRow>
{
    private readonly IDbConnection _connection;

    private readonly ITable _table;

    private readonly IString _schemaName;

    public RowsEnumerable(IDbConnection connection, IString schemaName, ITable table)
    {
        _connection = connection;
        _table = table;
        _schemaName = schemaName;
    }

    public IEnumerator<IRow> GetEnumerator()
    {
        using IDbCommand cmd = _connection.CreateCommand();
        cmd.CommandText = new SelectAllStatement(_table, _schemaName).TextValue;
        using IDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            IReadOnlyDictionary<string, string> rawCells = _table
                .Columns.Select(x =>
                    new TrimmedHash(new HexString(new ColumnHash(x))).TextValue
                )
                .ToDictionary(x => x, x => reader[x].ToString())!;

            IReadOnlyDictionary<IColumn, ICell> cells = new Dictionary<
                IColumn,
                IColumn,
                ICell
            >(
                _table.Columns,
                x => x,
                x => new Cell(
                    new String(
                        rawCells[
                            new TrimmedHash(new HexString(new ColumnHash(x))).TextValue
                        ]
                            .ToString()
                    )
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

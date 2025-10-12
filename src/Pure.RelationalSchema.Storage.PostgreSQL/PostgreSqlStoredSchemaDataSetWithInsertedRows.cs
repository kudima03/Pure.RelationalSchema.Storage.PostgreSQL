using System.Data;
using Pure.Collections.Generic;
using Pure.Primitives.String.Operations;
using Pure.RelationalSchema.Abstractions.Schema;
using Pure.RelationalSchema.Abstractions.Table;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Storage.Abstractions;

namespace Pure.RelationalSchema.Storage.PostgreSQL;

public sealed record PostgreSqlStoredSchemaDataSetWithInsertedRows
    : IPostgreSqlStoredSchemaDataSet
{
    private readonly ISchema _schema;

    private readonly IPostgreSqlStoredSchemaDataSet _dataset;

    private readonly IEnumerable<IGrouping<ITable, IRow>> _rows;

    public PostgreSqlStoredSchemaDataSetWithInsertedRows(
        ISchema schema,
        IPostgreSqlStoredSchemaDataSet dataset,
        IEnumerable<IGrouping<ITable, IRow>> rows
    )
    {
        _schema = schema;
        _dataset = dataset;
        _rows = rows;
    }

    public IDbConnection Connection => _dataset.Connection;

    public IReadOnlyDictionary<ITable, IStoredTableDataSet> TablesDatasets =>
        new OrderedDictionary<
            KeyValuePair<ITable, IStoredTableDataSet>,
            ITable,
            IStoredTableDataSet
        >(
            _rows.Select(x => x.Key).Select(x => new KeyValuePair<ITable, IStoredTableDataSet>(x, _dataset.TablesDatasets[x])),
            x => x.Key,
            x => new PostgreSqlStoredTableDataSetWithInsertedRows(
                x.Value,
                Connection,
                new TrimmedHash(new HexString(new SchemaHash(_schema))),
                _rows.Single(c =>
                    new TableHash(c.Key).SequenceEqual(new TableHash(x.Key))
                )
            ),
            x => new TableHash(x)
        );
}

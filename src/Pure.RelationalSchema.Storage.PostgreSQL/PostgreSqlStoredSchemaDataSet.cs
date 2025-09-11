using System.Data;
using Pure.Collections.Generic;
using Pure.Primitives.String.Operations;
using Pure.RelationalSchema.Abstractions.Schema;
using Pure.RelationalSchema.Abstractions.Table;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Storage.Abstractions;

namespace Pure.RelationalSchema.Storage.PostgreSQL;

public sealed record PostgreSqlStoredSchemaDataSet : IStoredSchemaDataSet
{
    private readonly ISchema _schema;

    private readonly IDbConnection _connection;

    public PostgreSqlStoredSchemaDataSet(ISchema schema, IDbConnection connection)
    {
        _schema = schema;
        _connection = connection;
    }

    public IReadOnlyDictionary<ITable, IStoredTableDataSet> TablesDatasets =>
        new Dictionary<ITable, ITable, IStoredTableDataSet>(
            _schema.Tables,
            x => x,
            x => new PostgreSqlStoredTableDataSet(
                x,
                new TrimmedHash(new HexString(new SchemaHash(_schema))),
                _connection
            ),
            x => new TableHash(x)
        );
}

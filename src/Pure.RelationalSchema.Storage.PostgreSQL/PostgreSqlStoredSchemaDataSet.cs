using System.Data;
using Pure.Collections.Generic;
using Pure.Primitives.String.Operations;
using Pure.RelationalSchema.Abstractions.Schema;
using Pure.RelationalSchema.Abstractions.Table;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Storage.Abstractions;

namespace Pure.RelationalSchema.Storage.PostgreSQL;

public sealed record PostgreSqlStoredSchemaDataSet : IPostgreSqlStoredSchemaDataSet
{
    private readonly ISchema _schema;

    public PostgreSqlStoredSchemaDataSet(ISchema schema, IDbConnection connection)
    {
        _schema = schema;
        Connection = connection;
    }

    public IDbConnection Connection { get; }

    public IReadOnlyDictionary<ITable, IStoredTableDataSet> TablesDatasets =>
        new Dictionary<ITable, ITable, IStoredTableDataSet>(
            _schema.Tables,
            x => x,
            x => new PostgreSqlStoredTableDataSet(
                x,
                new TrimmedHash(new HexString(new SchemaHash(_schema))),
                Connection
            ),
            x => new TableHash(x)
        );
}

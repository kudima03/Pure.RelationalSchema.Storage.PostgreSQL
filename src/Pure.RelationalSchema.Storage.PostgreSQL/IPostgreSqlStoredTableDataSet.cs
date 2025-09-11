using System.Data;
using Pure.Primitives.Abstractions.String;
using Pure.RelationalSchema.Storage.Abstractions;

namespace Pure.RelationalSchema.Storage.PostgreSQL;

public interface IPostgreSqlStoredTableDataSet : IStoredTableDataSet
{
    public IString SchemaName { get; }

    public IDbConnection Connection { get; }
}

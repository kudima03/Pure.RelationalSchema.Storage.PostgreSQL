using System.Data;
using Pure.RelationalSchema.Storage.Abstractions;

namespace Pure.RelationalSchema.Storage.PostgreSQL;

public interface IPostgreSqlStoredSchemaDataSet : IStoredSchemaDataSet
{
    public IDbConnection Connection { get; }
}

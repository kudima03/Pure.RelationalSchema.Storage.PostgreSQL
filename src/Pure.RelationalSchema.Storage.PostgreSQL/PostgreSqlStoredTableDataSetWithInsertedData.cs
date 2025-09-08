using System.Collections;
using System.Data;
using System.Linq.Expressions;
using Pure.Primitives.Abstractions.Bool;
using Pure.Primitives.Abstractions.String;
using Pure.Primitives.Bool;
using Pure.Primitives.String.Operations;
using Pure.RelationalSchema.Abstractions.Table;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Storage.Abstractions;

namespace Pure.RelationalSchema.Storage.PostgreSQL;

public sealed record PostgreSqlStoredTableDataSetWithInsertedRows
    : IPostgreSqlStoredTableDataSet
{
    private readonly IPostgreSqlStoredTableDataSet _dataSet;

    private readonly Lazy<IBool> _rowsInserted;

    public PostgreSqlStoredTableDataSetWithInsertedRows(
        IPostgreSqlStoredTableDataSet dataSet,
        IEnumerable<IRow> rows
    )
    {
        _dataSet = dataSet;
        _rowsInserted = new Lazy<IBool>(() =>
        {
            IDbCommand command = dataSet.Connection.CreateCommand();
            command.CommandText = new InsertStatement(
                new TrimmedHash(new HexString(new TableHash(dataSet.TableSchema))),
                dataSet.SchemaName,
                rows
            ).TextValue;
            _ = command.ExecuteNonQuery();
            return new True();
        });
    }

    public IString SchemaName =>
        _rowsInserted.Value.BoolValue
            ? _dataSet.SchemaName
            : throw new ArgumentException();

    public IDbConnection Connection =>
        _rowsInserted.Value.BoolValue
            ? _dataSet.Connection
            : throw new ArgumentException();

    public ITable TableSchema =>
        _rowsInserted.Value.BoolValue
            ? _dataSet.TableSchema
            : throw new ArgumentException();

    public Type ElementType =>
        _rowsInserted.Value.BoolValue
            ? _dataSet.ElementType
            : throw new ArgumentException();

    public Expression Expression =>
        _rowsInserted.Value.BoolValue
            ? _dataSet.Expression
            : throw new ArgumentException();

    public IQueryProvider Provider =>
        _rowsInserted.Value.BoolValue ? _dataSet.Provider : throw new ArgumentException();

    public IEnumerator<IRow> GetEnumerator()
    {
        return _rowsInserted.Value.BoolValue
            ? _dataSet.GetEnumerator()
            : throw new ArgumentException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IAsyncEnumerator<IRow> GetAsyncEnumerator(
        CancellationToken cancellationToken = default
    )
    {
        return _rowsInserted.Value.BoolValue
            ? _dataSet.GetAsyncEnumerator(cancellationToken)
            : throw new ArgumentException();
    }
}

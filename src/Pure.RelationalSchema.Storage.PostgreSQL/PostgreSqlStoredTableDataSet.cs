using System.Collections;
using System.Data;
using System.Linq.Expressions;
using Pure.RelationalSchema.Abstractions.Table;
using Pure.RelationalSchema.Storage.Abstractions;

namespace Pure.RelationalSchema.Storage.PostgreSQL;

public sealed record PostgreSqlStoredTableDataSet : IStoredTableDataSet
{
    private readonly IQueryable<IRow> _rows;

    private readonly IAsyncEnumerable<IRow> _rowsAsync;

    public PostgreSqlStoredTableDataSet(ITable tableSchema, IDbConnection connection)
        : this(
            tableSchema,
            new RowsEnumerable(connection, tableSchema).AsQueryable(),
            new RowsAsyncEnumerable(connection, tableSchema)
        ) { }

    private PostgreSqlStoredTableDataSet(
        ITable tableSchema,
        IQueryable<IRow> rows,
        IAsyncEnumerable<IRow> rowsAsync
    )
    {
        _rows = rows;
        _rowsAsync = rowsAsync;
        TableSchema = tableSchema;
    }

    public ITable TableSchema { get; }

    public Type ElementType => _rows.ElementType;

    public Expression Expression => _rows.Expression;

    public IQueryProvider Provider => _rows.Provider;

    public IEnumerator<IRow> GetEnumerator()
    {
        return _rows.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IAsyncEnumerator<IRow> GetAsyncEnumerator(
        CancellationToken cancellationToken = default
    )
    {
        return _rowsAsync.GetAsyncEnumerator(cancellationToken);
    }
}

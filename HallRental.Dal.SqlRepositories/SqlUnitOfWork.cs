using HallRental.Bll.Common.Shared;
using Microsoft.Data.SqlClient;

namespace HallRental.Dal.SqlRepositories;

public class SqlUnitOfWork : IUnitOfWork, IAsyncDisposable
{
    private readonly string _connectionString;
    private SqlConnection? _connection;
    private SqlTransaction? _transaction;

    public SqlUnitOfWork(string connectionString)
    {
        _connectionString = connectionString;
    }

    internal Task<SqlCommand> CreateCommandAsync(string sql, CancellationToken cancellationToken) =>
        CreateCommandAsync(sql, beginTransaction: false, cancellationToken);

    internal Task<SqlCommand> CreateTransactionalCommandAsync(string sql, CancellationToken cancellationToken) =>
        CreateCommandAsync(sql, beginTransaction: true, cancellationToken);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(CancellationToken.None);
            _transaction = null;
        }

        return 0;
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            if (_transaction != null)
                await _transaction.RollbackAsync();
        }
        finally
        {
            if (_connection != null)
                await _connection.DisposeAsync();
        }
    }

    private async Task<SqlCommand> CreateCommandAsync(string sql, bool beginTransaction, CancellationToken cancellationToken)
    {
        if (_connection == null)
        {
            _connection = new SqlConnection(_connectionString);
            await _connection.OpenAsync(cancellationToken);
        }

        if (beginTransaction && _transaction == null)
            _transaction = (SqlTransaction)await _connection.BeginTransactionAsync(cancellationToken);

        return new SqlCommand(sql, _connection, _transaction);
    }
}

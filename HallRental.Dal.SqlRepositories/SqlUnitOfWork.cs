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

    internal async Task<SqlCommand> CreateCommandAsync(string sql, CancellationToken cancellationToken)
    {
        if (_connection == null)
        {
            _connection = new SqlConnection(_connectionString);
            await _connection.OpenAsync(cancellationToken);
            _transaction = (SqlTransaction)await _connection.BeginTransactionAsync(cancellationToken);
        }

        return new SqlCommand(sql, _connection, _transaction);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken);
            _transaction = null;
        }

        return 0;
    }

    public async ValueTask DisposeAsync()
    {
        if (_transaction != null)
            await _transaction.RollbackAsync();

        if (_connection != null)
            await _connection.DisposeAsync();
    }
}

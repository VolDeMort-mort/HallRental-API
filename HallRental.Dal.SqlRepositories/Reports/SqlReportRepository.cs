using System.Data;
using HallRental.Bll.Common.Reports;
using HallRental.Bll.Common.Reports.Models;
using Microsoft.Data.SqlClient;

namespace HallRental.Dal.SqlRepositories.Reports;

public class SqlReportRepository : IReportRepository
{
    private readonly SqlUnitOfWork _unitOfWork;

    public SqlReportRepository(SqlUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<HallReportRow>> GetHallRowsAsync(DateTime from, DateTime to, CancellationToken cancellationToken)
    {
        var sql = SqlScripts.Get<SqlReportRepository>("GetHallRows");

        await using var command = await _unitOfWork.CreateCommandAsync(sql, cancellationToken);
        AddPeriod(command, from, to);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var rows = new List<HallReportRow>();
        while (await reader.ReadAsync(cancellationToken))
            rows.Add(new HallReportRow(
                reader.GetGuid(0),
                reader.GetString(1),
                reader.GetBoolean(2),
                reader.GetInt32(3),
                reader.GetInt32(4),
                reader.GetDecimal(5),
                reader.GetDecimal(6)));

        return rows;
    }

    public async Task<IReadOnlyList<ServiceReport>> GetServiceRowsAsync(DateTime from, DateTime to, CancellationToken cancellationToken)
    {
        var sql = SqlScripts.Get<SqlReportRepository>("GetServiceRows");

        await using var command = await _unitOfWork.CreateCommandAsync(sql, cancellationToken);
        AddPeriod(command, from, to);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var rows = new List<ServiceReport>();
        while (await reader.ReadAsync(cancellationToken))
            rows.Add(new ServiceReport(reader.GetString(0), reader.GetInt32(1), reader.GetDecimal(2)));

        return rows;
    }

    private static void AddPeriod(SqlCommand command, DateTime from, DateTime to)
    {
        command.Parameters.Add("@From", SqlDbType.DateTime2).Value = from;
        command.Parameters.Add("@To", SqlDbType.DateTime2).Value = to;
    }
}

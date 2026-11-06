using System.Data;
using AutoMapper;
using HallRental.Bll.Common.Bookings;
using HallRental.Bll.Common.Bookings.Models;
using HallRental.Bll.Common.Shared.Constants;
using HallRental.Dal.SqlRepositories.Bookings.Entities;
using Microsoft.Data.SqlClient;

namespace HallRental.Dal.SqlRepositories.Bookings;

public class SqlBookingRepository : IBookingRepository
{
    private readonly SqlUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SqlBookingRepository(SqlUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task AddAsync(Booking booking, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<BookingEntity>(booking);
        var sql = SqlScripts.Get<SqlBookingRepository>("Insert");

        await using (var command = await _unitOfWork.CreateCommandAsync(sql, cancellationToken))
        {
            command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = entity.Id;
            command.Parameters.Add("@HallId", SqlDbType.UniqueIdentifier).Value = entity.HallId;
            command.Parameters.Add("@ClientId", SqlDbType.NVarChar, EntityLimits.ClientIdMaxLength).Value = entity.ClientId;
            AddPeriod(command, entity.Start, entity.End);
            AddMoney(command, "@RentPrice", entity.RentPrice);
            AddMoney(command, "@ServicesPrice", entity.ServicesPrice);
            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        var serviceSql = SqlScripts.Get<SqlBookingRepository>("InsertService");

        foreach (var service in entity.Services)
        {
            await using var command = await _unitOfWork.CreateCommandAsync(serviceSql, cancellationToken);
            command.Parameters.Add("@BookingId", SqlDbType.UniqueIdentifier).Value = entity.Id;
            command.Parameters.Add("@ServiceId", SqlDbType.UniqueIdentifier).Value = service.ServiceId;
            command.Parameters.Add("@Name", SqlDbType.NVarChar, EntityLimits.ServiceNameMaxLength).Value = service.Name;
            AddMoney(command, "@Price", service.Price);
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
    }

    public async Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var sql = SqlScripts.Get<SqlBookingRepository>("GetById");

        await using var command = await _unitOfWork.CreateCommandAsync(sql, cancellationToken);
        command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        if (!await reader.ReadAsync(cancellationToken))
            return null;

        var entity = BookingEntity.Read(reader);
        var hallName = reader.GetString(7);

        await reader.NextResultAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
            entity.Services.Add(BookedServiceEntity.Read(reader));

        var model = _mapper.Map<Booking>(entity);
        model.HallName = hallName;

        return model;
    }

    // Same formula as the [Start, End) overlap: 10:00–12:00 and 12:00–14:00 don't overlap
    public async Task<bool> HasOverlapAsync(Guid hallId, DateTime start, DateTime end, CancellationToken cancellationToken)
    {
        var sql = SqlScripts.Get<SqlBookingRepository>("HasOverlap");

        await using var command = await _unitOfWork.CreateCommandAsync(sql, cancellationToken);
        command.Parameters.Add("@HallId", SqlDbType.UniqueIdentifier).Value = hallId;
        AddPeriod(command, start, end);

        return (int)(await command.ExecuteScalarAsync(cancellationToken))! == 1;
    }

    private static void AddPeriod(SqlCommand command, DateTime start, DateTime end)
    {
        command.Parameters.Add("@Start", SqlDbType.DateTime2).Value = start;
        command.Parameters.Add("@End", SqlDbType.DateTime2).Value = end;
    }

    private static void AddMoney(SqlCommand command, string name, decimal value) =>
        command.Parameters.Add(new SqlParameter(name, SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = value });
}

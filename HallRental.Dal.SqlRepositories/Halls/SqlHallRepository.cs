using System.Data;
using AutoMapper;
using HallRental.Bll.Common.Halls;
using HallRental.Bll.Common.Halls.Models;
using HallRental.Bll.Common.Shared.Constants;
using HallRental.Dal.SqlRepositories.Halls.Entities;
using Microsoft.Data.SqlClient;

namespace HallRental.Dal.SqlRepositories.Halls;

public class SqlHallRepository : IHallRepository
{
    private readonly SqlUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SqlHallRepository(SqlUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task AddAsync(Hall hall, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<HallEntity>(hall);
        var sql = SqlScripts.Get<SqlHallRepository>("Insert");

        await using (var command = await _unitOfWork.CreateCommandAsync(sql, cancellationToken))
        {
            AddHallParameters(command, entity);
            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        foreach (var service in entity.Services)
            await InsertServiceAsync(service, cancellationToken);
    }

    public async Task<Hall?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var sql = SqlScripts.Get<SqlHallRepository>("GetById");

        await using var command = await _unitOfWork.CreateCommandAsync(sql, cancellationToken);
        command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        if (!await reader.ReadAsync(cancellationToken))
            return null;

        var hall = HallEntity.Read(reader);

        await reader.NextResultAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
            hall.Services.Add(ServiceEntity.Read(reader));

        return _mapper.Map<Hall>(hall);
    }

    public async Task<IReadOnlyList<Hall>> GetAvailableAsync(int minCapacity, DateTime start, DateTime end, CancellationToken cancellationToken)
    {
        var sql = SqlScripts.Get<SqlHallRepository>("GetAvailable");

        await using var command = await _unitOfWork.CreateCommandAsync(sql, cancellationToken);
        command.Parameters.Add("@MinCapacity", SqlDbType.Int).Value = minCapacity;
        command.Parameters.Add("@Start", SqlDbType.DateTime2).Value = start;
        command.Parameters.Add("@End", SqlDbType.DateTime2).Value = end;
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var halls = new List<HallEntity>();
        while (await reader.ReadAsync(cancellationToken))
            halls.Add(HallEntity.Read(reader));

        var hallsById = halls.ToDictionary(h => h.Id);

        await reader.NextResultAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var service = ServiceEntity.Read(reader);
            hallsById[service.HallId].Services.Add(service);
        }

        return _mapper.Map<List<Hall>>(halls);
    }

    public async Task UpdateAsync(Hall hall, CancellationToken cancellationToken)
    {
        var sql = SqlScripts.Get<SqlHallRepository>("Update");

        await using var command = await _unitOfWork.CreateCommandAsync(sql, cancellationToken);
        AddHallParameters(command, _mapper.Map<HallEntity>(hall));
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public Task AddServiceAsync(Service service, CancellationToken cancellationToken) =>
        InsertServiceAsync(_mapper.Map<ServiceEntity>(service), cancellationToken);

    // Includes bookings going on right now
    public async Task<bool> HasFutureBookingsAsync(Guid hallId, DateTime now, CancellationToken cancellationToken)
    {
        var sql = SqlScripts.Get<SqlHallRepository>("HasFutureBookings");

        await using var command = await _unitOfWork.CreateCommandAsync(sql, cancellationToken);
        command.Parameters.Add("@HallId", SqlDbType.UniqueIdentifier).Value = hallId;
        command.Parameters.Add("@Now", SqlDbType.DateTime2).Value = now;

        return (int)(await command.ExecuteScalarAsync(cancellationToken))! == 1;
    }

    private async Task InsertServiceAsync(ServiceEntity service, CancellationToken cancellationToken)
    {
        var sql = SqlScripts.Get<SqlHallRepository>("InsertService");

        await using var command = await _unitOfWork.CreateCommandAsync(sql, cancellationToken);
        command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = service.Id;
        command.Parameters.Add("@HallId", SqlDbType.UniqueIdentifier).Value = service.HallId;
        command.Parameters.Add("@Name", SqlDbType.NVarChar, EntityLimits.ServiceNameMaxLength).Value = service.Name;
        command.Parameters.Add(new SqlParameter("@Price", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = service.Price });
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static void AddHallParameters(SqlCommand command, HallEntity hall)
    {
        command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = hall.Id;
        command.Parameters.Add("@Name", SqlDbType.NVarChar, EntityLimits.HallNameMaxLength).Value = hall.Name;
        command.Parameters.Add("@Capacity", SqlDbType.Int).Value = hall.Capacity;
        command.Parameters.Add(new SqlParameter("@PricePerHour", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = hall.PricePerHour });
        command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = hall.IsActive;
    }
}

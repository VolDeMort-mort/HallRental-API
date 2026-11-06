using System.Data;
using HallRental.Bll.Common.Shared;
using HallRental.Bll.Common.Shared.Constants;
using HallRental.Dal.SqlRepositories.Halls;
using HallRental.Dal.SqlRepositories.Halls.Entities;
using HallRental.Dal.SqlRepositories.Pricing.Entities;
using Microsoft.Data.SqlClient;

namespace HallRental.Dal.SqlRepositories;

internal class DbInitializer : IDatabaseInitializer
{
    private readonly SqlUnitOfWork _unitOfWork;

    public DbInitializer(SqlUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        await using (var command = await _unitOfWork.CreateTransactionalCommandAsync(SqlScripts.Get<DbInitializer>("CreateSchema"), cancellationToken))
            await command.ExecuteNonQueryAsync(cancellationToken);

        if (!await ExistsAsync("HasHalls", cancellationToken))
        {
            foreach (var hall in CreateHalls())
                await InsertHallAsync(hall, cancellationToken);
        }

        if (!await ExistsAsync("HasHoursPricings", cancellationToken))
        {
            foreach (var zone in CreateHoursPricings())
                await InsertZoneAsync(zone, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<bool> ExistsAsync(string scriptName, CancellationToken cancellationToken)
    {
        await using var command = await _unitOfWork.CreateCommandAsync(SqlScripts.Get<DbInitializer>(scriptName), cancellationToken);
        return (int)(await command.ExecuteScalarAsync(cancellationToken))! == 1;
    }

    private async Task InsertHallAsync(HallEntity hall, CancellationToken cancellationToken)
    {
        await using (var command = await _unitOfWork.CreateTransactionalCommandAsync(SqlScripts.Get<SqlHallRepository>("Insert"), cancellationToken))
        {
            command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = hall.Id;
            command.Parameters.Add("@Name", SqlDbType.NVarChar, EntityLimits.HallNameMaxLength).Value = hall.Name;
            command.Parameters.Add("@Capacity", SqlDbType.Int).Value = hall.Capacity;
            command.Parameters.Add(new SqlParameter("@PricePerHour", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = hall.PricePerHour });
            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = hall.IsActive;
            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        foreach (var service in hall.Services)
        {
            await using var command = await _unitOfWork.CreateTransactionalCommandAsync(SqlScripts.Get<SqlHallRepository>("InsertService"), cancellationToken);
            command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = service.Id;
            command.Parameters.Add("@HallId", SqlDbType.UniqueIdentifier).Value = service.HallId;
            command.Parameters.Add("@Name", SqlDbType.NVarChar, EntityLimits.ServiceNameMaxLength).Value = service.Name;
            command.Parameters.Add(new SqlParameter("@Price", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = service.Price });
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
    }

    private async Task InsertZoneAsync(HoursPricingEntity zone, CancellationToken cancellationToken)
    {
        await using var command = await _unitOfWork.CreateTransactionalCommandAsync(SqlScripts.Get<DbInitializer>("InsertHoursPricing"), cancellationToken);
        command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = zone.Id;
        command.Parameters.Add("@Name", SqlDbType.NVarChar, EntityLimits.HoursPricingNameMaxLength).Value = zone.Name;
        command.Parameters.Add("@From", SqlDbType.Time).Value = zone.From.ToTimeSpan();
        command.Parameters.Add("@To", SqlDbType.Time).Value = zone.To.ToTimeSpan();
        command.Parameters.Add(new SqlParameter("@Multiplier", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = zone.Multiplier });
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static IEnumerable<HallEntity> CreateHalls()
    {
        var halls = new[]
        {
            CreateHall("Зал A", 50, 2000m),
            CreateHall("Зал B", 100, 3500m),
            CreateHall("Зал C", 30, 1500m)
        };

        foreach (var hall in halls)
        {
            hall.Services.Add(CreateService(hall.Id, "Проєктор", 500m));
            hall.Services.Add(CreateService(hall.Id, "Wi-Fi", 300m));
            hall.Services.Add(CreateService(hall.Id, "Звук", 700m));
        }

        return halls;
    }

    private static IEnumerable<HoursPricingEntity> CreateHoursPricings() =>
    [
        CreateZone("Morning", new TimeOnly(6, 0), new TimeOnly(9, 0), 0.90m),
        CreateZone("Standard", new TimeOnly(9, 0), new TimeOnly(12, 0), 1.00m),
        CreateZone("Peak", new TimeOnly(12, 0), new TimeOnly(14, 0), 1.15m),
        CreateZone("Standard", new TimeOnly(14, 0), new TimeOnly(18, 0), 1.00m),
        CreateZone("Evening", new TimeOnly(18, 0), new TimeOnly(23, 0), 0.80m)
    ];

    private static HallEntity CreateHall(string name, int capacity, decimal pricePerHour) => new()
    {
        Id = Guid.NewGuid(),
        Name = name,
        Capacity = capacity,
        PricePerHour = pricePerHour,
        IsActive = true
    };

    private static ServiceEntity CreateService(Guid hallId, string name, decimal price) => new()
    {
        Id = Guid.NewGuid(),
        HallId = hallId,
        Name = name,
        Price = price
    };

    private static HoursPricingEntity CreateZone(string name, TimeOnly from, TimeOnly to, decimal multiplier) => new()
    {
        Id = Guid.NewGuid(),
        Name = name,
        From = from,
        To = to,
        Multiplier = multiplier
    };
}

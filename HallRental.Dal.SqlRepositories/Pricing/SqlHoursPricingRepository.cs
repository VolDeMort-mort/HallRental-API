using AutoMapper;
using HallRental.Bll.Common.Pricing;
using HallRental.Bll.Common.Pricing.Models;
using HallRental.Dal.SqlRepositories.Pricing.Entities;

namespace HallRental.Dal.SqlRepositories.Pricing;

public class SqlHoursPricingRepository : IHoursPricingRepository
{
    private readonly SqlUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SqlHoursPricingRepository(SqlUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<HoursPricing>> GetAllAsync(CancellationToken cancellationToken)
    {
        var sql = SqlScripts.Get<SqlHoursPricingRepository>("GetAll");

        await using var command = await _unitOfWork.CreateCommandAsync(sql, cancellationToken);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var zones = new List<HoursPricingEntity>();
        while (await reader.ReadAsync(cancellationToken))
            zones.Add(HoursPricingEntity.Read(reader));

        return _mapper.Map<List<HoursPricing>>(zones);
    }
}

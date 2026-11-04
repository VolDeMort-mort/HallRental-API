using AutoMapper;
using HallRental.Bll.Common.Pricing;
using HallRental.Bll.Common.Pricing.Models;
using Microsoft.EntityFrameworkCore;

namespace HallRental.Dal.SqlRepositories.Pricing;

public class SqlHoursPricingRepository : IHoursPricingRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public SqlHoursPricingRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<HoursPricing>> GetAllAsync(CancellationToken cancellationToken)
    {
        var zones = await _context.HoursPricings.AsNoTracking().ToListAsync(cancellationToken);

        return _mapper.Map<List<HoursPricing>>(zones);
    }
}

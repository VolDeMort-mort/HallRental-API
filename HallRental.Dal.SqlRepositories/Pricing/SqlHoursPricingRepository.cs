using HallRental.Bll.Common.Pricing;
using HallRental.Bll.Common.Pricing.Models;
using HallRental.Dal.SqlRepositories.Mapping;
using Microsoft.EntityFrameworkCore;

namespace HallRental.Dal.SqlRepositories.Pricing;

public class SqlHoursPricingRepository : IHoursPricingRepository
{
    private readonly AppDbContext _context;

    public SqlHoursPricingRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<HoursPricing>> GetAllAsync(CancellationToken cancellationToken)
    {
        var zones = await _context.HoursPricings.AsNoTracking().ToListAsync(cancellationToken);

        return zones.Select(z => z.ToModel()).ToList();
    }
}

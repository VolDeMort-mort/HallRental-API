using HallRental.Application.Interfaces;
using HallRental.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HallRental.Infrastructure.Persistence.Repositories;

public class HoursPricingRepository: IHoursPricingRepository
{
    private readonly AppDbContext _context;
    public HoursPricingRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<HoursPricing>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.HoursPricings.AsNoTracking().ToListAsync(cancellationToken);
    }
}

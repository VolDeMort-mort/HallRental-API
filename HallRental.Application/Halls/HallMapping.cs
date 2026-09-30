using HallRental.Domain.Entities;

namespace HallRental.Application.Halls;

internal static class HallMapping
{
    public static HallDto ToDto(this Hall hall) => new(
        hall.Id,
        hall.Name,
        hall.Capacity,
        hall.PricePerHour,
        // Ordinal, so the order doesn't depend on the server's culture
        hall.Services
            .OrderBy(s => s.Name, StringComparer.Ordinal)
            .Select(s => new ServiceDto(s.Id, s.Name, s.Price))
            .ToList());
}

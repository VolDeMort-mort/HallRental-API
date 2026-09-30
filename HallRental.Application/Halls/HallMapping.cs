using HallRental.Domain.Entities;

namespace HallRental.Application.Halls;

/// <summary>One place that turns a hall into its DTO, shared by every query that returns halls.</summary>
internal static class HallMapping
{
    public static HallDto ToDto(this Hall hall) => new(
        hall.Id,
        hall.Name,
        hall.Capacity,
        hall.PricePerHour,
        // Ordinal order: the same response on every server, whatever its culture settings
        hall.Services
            .OrderBy(s => s.Name, StringComparer.Ordinal)
            .Select(s => new ServiceDto(s.Id, s.Name, s.Price))
            .ToList());
}

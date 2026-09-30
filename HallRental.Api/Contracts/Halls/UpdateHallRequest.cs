using HallRental.Application.Halls.UpdateHall;

namespace HallRental.Api.Contracts.Halls;

/// <summary>New data of a hall. Send every field; unchanged ones with their current values.</summary>
public sealed record UpdateHallRequest
{
    /// <summary>Hall name.</summary>
    /// <example>Зал A</example>
    public required string Name { get; init; }

    /// <summary>Maximum number of people.</summary>
    /// <example>50</example>
    public required int Capacity { get; init; }

    /// <summary>Base rent per hour in UAH. Existing bookings keep the price they were made with.</summary>
    /// <example>2500</example>
    public required decimal PricePerHour { get; init; }

    // The id comes from the route, so a body can't point the command at another hall
    public UpdateHallCommand ToCommand(Guid id) => new(id, Name, Capacity, PricePerHour);
}

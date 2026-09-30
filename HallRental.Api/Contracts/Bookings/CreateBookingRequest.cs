using HallRental.Application.Bookings.CreateBooking;

namespace HallRental.Api.Contracts.Bookings;

public sealed record CreateBookingRequest
{
    public required Guid HallId { get; init; }

    /// <summary>The hall's local time, without a UTC offset.</summary>
    /// <example>2030-09-01T10:00:00</example>
    public required DateTime Start { get; init; }

    /// <summary>hh:mm:ss; the whole rent has to fit into 06:00–23:00.</summary>
    /// <example>04:00:00</example>
    public required TimeSpan Duration { get; init; }

    public required IReadOnlyList<Guid> ServiceIds { get; init; }

    public CreateBookingCommand ToCommand() => new(HallId, Start, Duration, ServiceIds);
}

using HallRental.Application.Bookings.CreateBooking;

namespace HallRental.Api.Contracts.Bookings;

/// <summary>A booking of one hall for one period.</summary>
public sealed record CreateBookingRequest
{
    /// <summary>Id of the hall, taken from the search or the hall details.</summary>
    public required Guid HallId { get; init; }

    /// <summary>Start of the rent in the hall's local time, without a UTC offset.</summary>
    /// <example>2030-09-01T10:00:00</example>
    public required DateTime Start { get; init; }

    /// <summary>Length of the rent, hh:mm:ss. The whole rent has to fit into working hours (06:00–23:00).</summary>
    /// <example>04:00:00</example>
    public required TimeSpan Duration { get; init; }

    /// <summary>Ids of the hall's services to include; may be empty. Prices always come from the hall.</summary>
    public required IReadOnlyList<Guid> ServiceIds { get; init; }

    public CreateBookingCommand ToCommand() => new(HallId, Start, Duration, ServiceIds);
}

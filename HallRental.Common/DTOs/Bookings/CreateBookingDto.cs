namespace HallRental.Common.DTOs.Bookings;

/// <summary>Only the ids of services come from the client; their prices always come from the hall.</summary>
public record CreateBookingDto(
    Guid HallId,
    DateTime Start,
    TimeSpan Duration,
    IReadOnlyList<Guid> ServiceIds);

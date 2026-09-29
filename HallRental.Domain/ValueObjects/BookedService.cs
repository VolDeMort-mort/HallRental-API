namespace HallRental.Domain.ValueObjects;

/// <summary>
/// A copy of a hall service taken at the moment of booking.
/// If the projector becomes more expensive tomorrow, bookings made today keep today's price.
/// </summary>
public sealed record BookedService(Guid ServiceId, string Name, decimal Price);

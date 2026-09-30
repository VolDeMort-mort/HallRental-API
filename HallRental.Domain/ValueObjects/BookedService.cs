namespace HallRental.Domain.ValueObjects;

// A copy taken at the booking moment, so later price changes don't affect existing bookings
public sealed record BookedService(Guid ServiceId, string Name, decimal Price);

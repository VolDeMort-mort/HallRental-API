using HallRental.Domain.Entities;

namespace HallRental.Application.Bookings;

internal static class BookingMapping
{
    public static BookingDto ToDto(this Booking booking, string hallName) => new(
        booking.Id,
        booking.HallId,
        hallName,
        booking.ClientId,
        booking.Period.Start,
        booking.Period.End,
        booking.Services
            .OrderBy(s => s.Name, StringComparer.Ordinal)
            .Select(s => new BookedServiceDto(s.ServiceId, s.Name, s.Price))
            .ToList(),
        booking.RentPrice,
        booking.ServicesPrice,
        booking.TotalPrice);
}

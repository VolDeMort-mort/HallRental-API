using HallRental.Domain.Entities;

namespace HallRental.Application.Bookings;

/// <summary>One place that turns a booking into its DTO, shared by the command and the query.</summary>
internal static class BookingMapping
{
    /// <param name="booking">The booking to show.</param>
    /// <param name="hallName">Passed in separately: a booking stores only the id of its hall.</param>
    public static BookingDto ToDto(this Booking booking, string hallName) => new(
        booking.Id,
        booking.HallId,
        hallName,
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

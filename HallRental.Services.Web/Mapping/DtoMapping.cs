using HallRental.Bll.Common.Bookings.Models;
using HallRental.Bll.Common.Halls.Models;
using HallRental.Bll.Common.Reports.Models;
using HallRental.Services.Web.DTOs.Bookings;
using HallRental.Services.Web.DTOs.Halls;
using HallRental.Services.Web.DTOs.Reports;

namespace HallRental.Services.Web.Mapping;

internal static class DtoMapping
{
    public static Hall ToModel(this CreateHallDto dto) => new()
    {
        Name = dto.Name,
        Capacity = dto.Capacity,
        PricePerHour = dto.PricePerHour,
        Services = dto.Services.Select(s => s.ToModel()).ToList()
    };

    // The id comes from the route, so the body can't point the update at another hall
    public static Hall ToModel(this UpdateHallDto dto, Guid id) => new()
    {
        Id = id,
        Name = dto.Name,
        Capacity = dto.Capacity,
        PricePerHour = dto.PricePerHour
    };

    public static Service ToModel(this CreateServiceDto dto) => new()
    {
        Name = dto.Name,
        Price = dto.Price
    };

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

    public static NewBooking ToModel(this CreateBookingDto dto) => new()
    {
        HallId = dto.HallId,
        Start = dto.Start,
        Duration = dto.Duration,
        ServiceIds = dto.ServiceIds.ToList()
    };

    public static BookingDto ToDto(this Booking booking) => new(
        booking.Id,
        booking.HallId,
        booking.HallName,
        booking.ClientId,
        booking.Start,
        booking.End,
        booking.Services
            .OrderBy(s => s.Name, StringComparer.Ordinal)
            .Select(s => new BookedServiceDto(s.ServiceId, s.Name, s.Price))
            .ToList(),
        booking.RentPrice,
        booking.ServicesPrice,
        booking.TotalPrice);

    public static ReportPeriod ToModel(this ReportPeriodDto dto) => new(dto.From, dto.To);

    public static HallsReportDto ToDto(this HallsReport report) => new(
        report.From,
        report.To,
        report.WorkingHoursPerDay,
        report.Halls
            .Select(h => new HallReportDto(
                h.HallId,
                h.Name,
                h.IsActive,
                h.Bookings,
                h.BookedHours,
                h.OccupancyPercent,
                h.RentRevenue,
                h.ServicesRevenue,
                h.TotalRevenue))
            .ToList(),
        new HallsReportTotalDto(
            report.Total.Bookings,
            report.Total.BookedHours,
            report.Total.RentRevenue,
            report.Total.ServicesRevenue,
            report.Total.TotalRevenue));

    public static ServicesReportDto ToDto(this ServicesReport report) => new(
        report.From,
        report.To,
        report.Services.Select(s => new ServiceReportDto(s.Name, s.TimesBooked, s.Revenue)).ToList(),
        report.TotalRevenue);
}

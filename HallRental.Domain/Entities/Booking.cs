using HallRental.Domain.Exceptions;
using HallRental.Domain.Pricing;
using HallRental.Domain.ValueObjects;

namespace HallRental.Domain.Entities;

/// <summary>
/// A booking of one hall for one period with the services the client picked.
/// <para>
/// The check that the hall is free is NOT here: it needs the other bookings of the hall,
/// so the Application layer does it with a database query before creating a booking.
/// </para>
/// </summary>
public class Booking
{
    private readonly List<BookedService> _services = new();

    public Guid Id { get; private set; }

    public Guid HallId { get; private set; }

    /// <summary> Always set by Create; null! is only for EF Core, which uses the private constructor.</summary>
    public RentalPeriod Period { get; private set; } = null!;

    /// <summary> Hall rent with the tariff zones applied.</summary>
    public decimal RentPrice { get; private set; }

    /// <summary> Sum of the picked services at the prices of the booking moment.</summary>
    public decimal ServicesPrice { get; private set; }

    public decimal TotalPrice => RentPrice + ServicesPrice;

    public IReadOnlyCollection<BookedService> Services => _services.AsReadOnly();

    private Booking(){}

    /// <param name="now">Current time in the hall's local time. It comes from outside (TimeProvider in the handler),
    /// so the domain never calls DateTime.Now and stays testable.</param>
    public static Booking Create(
        Hall hall,
        RentalPeriod period,
        IReadOnlyCollection<Guid> serviceIds,
        PricingPolicy pricing,
        DateTime now)
    {
        EnsureNotInPast(period, now);

        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            HallId = hall.Id,
            Period = period,
            RentPrice = pricing.CalculateRent(hall.PricePerHour, period)
        };

        booking._services.AddRange(hall.SelectServices(serviceIds)
            .Select(s => new BookedService(s.Id, s.Name, s.Price)));
        booking.ServicesPrice = booking._services.Sum(s => s.Price);

        return booking;
    }

    /// <summary>
    /// Moves the booking to another period of the same hall. Services keep their booked prices,
    /// the rent is calculated again for the new time.
    /// The caller still has to check that the new period is free (excluding this booking).
    /// </summary>
    public void ChangeRentTime(RentalPeriod period, Hall hall, PricingPolicy pricing, DateTime now)
    {
        if (hall.Id != HallId)
            throw new ArgumentException("The hall doesn't match the booking", nameof(hall));

        EnsureNotInPast(period, now);

        Period = period;
        RentPrice = pricing.CalculateRent(hall.PricePerHour, period);
    }

    private static void EnsureNotInPast(RentalPeriod period, DateTime now)
    {
        if (period.Start < now)
            throw new DomainException("A booking can't start in the past");
    }
}
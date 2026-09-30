using HallRental.Domain.Exceptions;
using HallRental.Domain.Pricing;
using HallRental.Domain.ValueObjects;

namespace HallRental.Domain.Entities;

// "Is the hall free?" needs the other bookings of the hall, so the Application layer checks it before Create
public class Booking
{
    public const int MaxClientIdLength = 200;

    private readonly List<BookedService> _services = new();

    public Guid Id { get; private set; }

    public Guid HallId { get; private set; }

    public string ClientId { get; private set; } = string.Empty;

    // Always set by Create; null! is only for EF Core, which uses the private constructor
    public RentalPeriod Period { get; private set; } = null!;

    public decimal RentPrice { get; private set; }

    public decimal ServicesPrice { get; private set; }

    public decimal TotalPrice => RentPrice + ServicesPrice;

    public IReadOnlyCollection<BookedService> Services => _services.AsReadOnly();

    private Booking(){}

    // now comes from outside instead of DateTime.Now, so the domain stays testable
    public static Booking Create(
        string clientId,
        Hall hall,
        RentalPeriod period,
        IReadOnlyCollection<Guid> serviceIds,
        PricingPolicy pricing,
        DateTime now)
    {
        if (string.IsNullOrWhiteSpace(clientId) || clientId.Length > MaxClientIdLength)
            throw new DomainException("A booking needs a valid client");

        if (!hall.IsActive)
            throw new DomainException("A deleted hall can't be booked");

        EnsureNotInPast(period, now);

        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            HallId = hall.Id,
            ClientId = clientId,
            Period = period,
            RentPrice = pricing.CalculateRent(hall.PricePerHour, period)
        };

        booking._services.AddRange(hall.SelectServices(serviceIds)
            .Select(s => new BookedService(s.Id, s.Name, s.Price)));
        booking.ServicesPrice = booking._services.Sum(s => s.Price);

        return booking;
    }

    // The caller still has to check that the new period is free (excluding this booking)
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
namespace HallRental.Bll.Common.Bookings.Models;

public class Booking
{
    public Guid Id { get; set; }

    public Guid HallId { get; set; }

    public string ClientId { get; set; } = string.Empty;

    public DateTime Start { get; set; }

    public DateTime End { get; set; }

    public decimal RentPrice { get; set; }

    public decimal ServicesPrice { get; set; }

    public decimal TotalPrice => RentPrice + ServicesPrice;

    public List<BookedService> Services { get; set; } = new();
}

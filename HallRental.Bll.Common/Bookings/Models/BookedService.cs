namespace HallRental.Bll.Common.Bookings.Models;

public class BookedService
{
    public Guid ServiceId { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }
}

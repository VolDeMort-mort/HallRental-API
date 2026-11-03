namespace HallRental.Bll.Common.Bookings.Models;

public class NewBooking
{
    public Guid HallId { get; set; }

    public DateTime Start { get; set; }

    public TimeSpan Duration { get; set; }

    public List<Guid> ServiceIds { get; set; } = new();
}

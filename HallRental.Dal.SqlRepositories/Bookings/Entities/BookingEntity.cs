namespace HallRental.Dal.SqlRepositories.Bookings.Entities;

internal class BookingEntity
{
    public Guid Id { get; set; }

    public Guid HallId { get; set; }

    public string ClientId { get; set; } = string.Empty;

    public DateTime Start { get; set; }

    public DateTime End { get; set; }

    public decimal RentPrice { get; set; }

    public decimal ServicesPrice { get; set; }

    public List<BookedServiceEntity> Services { get; set; } = new();
}

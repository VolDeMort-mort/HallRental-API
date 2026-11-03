namespace HallRental.Dal.SqlRepositories.Bookings.Entities;

internal class BookedServiceEntity
{
    public Guid ServiceId { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }
}

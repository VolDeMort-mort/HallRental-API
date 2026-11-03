namespace HallRental.Common.Models;

public class BookedServiceModel
{
    public Guid ServiceId { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }
}
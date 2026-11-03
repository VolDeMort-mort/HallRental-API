namespace HallRental.Common.Models;

public class ServiceModel
{
    public Guid Id { get; set; }

    public Guid HallId { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }
}
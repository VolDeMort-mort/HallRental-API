namespace HallRental.Bll.Common.Halls.Models;

public class Service
{
    public Guid Id { get; set; }

    public Guid HallId { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }
}

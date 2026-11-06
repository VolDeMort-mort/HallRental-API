namespace HallRental.Bll.Common.Halls.Models;

public class Hall
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int Capacity { get; set; }

    public decimal PricePerHour { get; set; }

    public bool IsActive { get; set; }

    public List<Service> Services { get; set; } = new();
}

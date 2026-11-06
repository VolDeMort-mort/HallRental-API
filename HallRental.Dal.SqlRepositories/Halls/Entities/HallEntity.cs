namespace HallRental.Dal.SqlRepositories.Halls.Entities;

internal class HallEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int Capacity { get; set; }

    public decimal PricePerHour { get; set; }

    public bool IsActive { get; set; }

    public List<ServiceEntity> Services { get; set; } = new();
}

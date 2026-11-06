using Microsoft.Data.SqlClient;

namespace HallRental.Dal.SqlRepositories.Halls.Entities;

internal class HallEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int Capacity { get; set; }

    public decimal PricePerHour { get; set; }

    public bool IsActive { get; set; }

    public List<ServiceEntity> Services { get; set; } = new();

    public static HallEntity Read(SqlDataReader reader) => new()
    {
        Id = reader.GetGuid(0),
        Name = reader.GetString(1),
        Capacity = reader.GetInt32(2),
        PricePerHour = reader.GetDecimal(3),
        IsActive = reader.GetBoolean(4)
    };
}

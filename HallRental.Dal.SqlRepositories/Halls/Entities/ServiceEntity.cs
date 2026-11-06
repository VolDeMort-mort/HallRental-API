using Microsoft.Data.SqlClient;

namespace HallRental.Dal.SqlRepositories.Halls.Entities;

internal class ServiceEntity
{
    public Guid Id { get; set; }

    public Guid HallId { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public static ServiceEntity Read(SqlDataReader reader) => new()
    {
        Id = reader.GetGuid(0),
        HallId = reader.GetGuid(1),
        Name = reader.GetString(2),
        Price = reader.GetDecimal(3)
    };
}

using Microsoft.Data.SqlClient;

namespace HallRental.Dal.SqlRepositories.Bookings.Entities;

internal class BookedServiceEntity
{
    public Guid ServiceId { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public static BookedServiceEntity Read(SqlDataReader reader) => new()
    {
        ServiceId = reader.GetGuid(0),
        Name = reader.GetString(1),
        Price = reader.GetDecimal(2)
    };
}

using Microsoft.Data.SqlClient;

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

    public static BookingEntity Read(SqlDataReader reader) => new()
    {
        Id = reader.GetGuid(0),
        HallId = reader.GetGuid(1),
        ClientId = reader.GetString(2),
        Start = reader.GetDateTime(3),
        End = reader.GetDateTime(4),
        RentPrice = reader.GetDecimal(5),
        ServicesPrice = reader.GetDecimal(6)
    };
}

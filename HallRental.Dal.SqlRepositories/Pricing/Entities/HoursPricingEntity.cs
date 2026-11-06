using Microsoft.Data.SqlClient;

namespace HallRental.Dal.SqlRepositories.Pricing.Entities;

internal class HoursPricingEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public TimeOnly From { get; set; }

    public TimeOnly To { get; set; }

    public decimal Multiplier { get; set; }

    public static HoursPricingEntity Read(SqlDataReader reader) => new()
    {
        Id = reader.GetGuid(0),
        Name = reader.GetString(1),
        From = TimeOnly.FromTimeSpan(reader.GetTimeSpan(2)),
        To = TimeOnly.FromTimeSpan(reader.GetTimeSpan(3)),
        Multiplier = reader.GetDecimal(4)
    };
}

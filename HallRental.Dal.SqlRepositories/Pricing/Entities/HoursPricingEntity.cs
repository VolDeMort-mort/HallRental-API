namespace HallRental.Dal.SqlRepositories.Pricing.Entities;

internal class HoursPricingEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public TimeOnly From { get; set; }

    public TimeOnly To { get; set; }

    public decimal Multiplier { get; set; }
}

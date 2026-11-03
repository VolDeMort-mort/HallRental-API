namespace HallRental.Common.Models;

public class HoursPricingModel
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public TimeOnly From { get; set; }

    public TimeOnly To { get; set; }

    public decimal Multiplier { get; set; }
}
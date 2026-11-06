namespace HallRental.Bll.Common.Pricing.Models;

public class HoursPricing
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public TimeOnly From { get; set; }

    public TimeOnly To { get; set; }

    public decimal Multiplier { get; set; }
}

using HallRental.Domain.Exceptions;

namespace HallRental.Domain.Entities;

/// <summary>
/// A time-of-day tariff zone, e.g. "Peak 12:00–14:00 x1.15" or "Evening 18:00–23:00 x0.80".
/// The multiplier is applied to the hall's price per hour: below 1 is a discount, above 1 is a markup.
/// A zone lies within one day, so it can't cross midnight.
/// </summary>
public class HoursPricing
{
    public Guid Id{get; private set;}

    public string Name { get; private set; } = string.Empty;

    public TimeOnly From { get; private set; }

    public TimeOnly To { get; private set; }

    public decimal Multiplier { get; private set; }

    private HoursPricing(){}

    public static HoursPricing Create(string name, TimeOnly from, TimeOnly to, decimal multiplier)
    {
        var pricing = new HoursPricing{Id = Guid.NewGuid()};
        pricing.Rename(name);
        pricing.ChangeHours(from, to);
        pricing.ChangeMultiplier(multiplier);
        return pricing;
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Hours pricing name can't be empty");
        Name = name.Trim();
    }

    public void ChangeHours(TimeOnly from, TimeOnly to)
    {
        if (from >= to)
            throw new DomainException("Hours pricing time to has to be greater then time from");
        From = from;
        To = to;
    }

    public void ChangeMultiplier(decimal multiplier)
    {
        if (multiplier <= 0)
            throw new DomainException("Hours pricing multiplier has to be greater than 0");
        Multiplier = multiplier;
    }

    public bool Overlaps(HoursPricing other) => From < other.To && other.From < To;
}

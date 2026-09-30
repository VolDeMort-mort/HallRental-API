using HallRental.Domain.Exceptions;

namespace HallRental.Domain.Entities;

// A time-of-day tariff zone within one day; Multiplier below 1 is a discount, above 1 a markup
public class HoursPricing
{
    public const int MaxNameLength = 100;

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
        if (name.Trim().Length > MaxNameLength)
            throw new DomainException($"Hours pricing name can't be longer than {MaxNameLength} characters");
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

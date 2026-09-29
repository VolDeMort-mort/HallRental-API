using HallRental.Domain.Exceptions;

namespace HallRental.Domain.ValueObjects;

/// <summary>
/// Rental interval [Start, End) in the hall's local time.
/// The end is exclusive, so 10:00–12:00 and 12:00–14:00 do not overlap.
/// It is a value object: two periods with the same Start and End are equal.
/// </summary>
public sealed record RentalPeriod
{
    public DateTime Start { get; }

    public DateTime End { get; }

    public RentalPeriod(DateTime start, DateTime end)
    {
        if (start >= end)
            throw new DomainException("The end of the rental has to be greater than its start");
        Start = start;
        End = end;
    }

    /// <summary>The assignment describes a booking as "start + duration", so this is the natural entry point for the API.</summary>
    public static RentalPeriod FromDuration(DateTime start, TimeSpan duration) => new(start, start + duration);

    public TimeSpan Duration => End - Start;

    /// <summary>Two intervals overlap when each of them starts before the other one ends.</summary>
    public bool Overlaps(RentalPeriod otherPeriod) => Start < otherPeriod.End && otherPeriod.Start < End;
}

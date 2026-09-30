using HallRental.Domain.Exceptions;

namespace HallRental.Domain.ValueObjects;

// [Start, End) in the hall's local time: 10:00–12:00 and 12:00–14:00 don't overlap
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

    public static RentalPeriod FromDuration(DateTime start, TimeSpan duration) => new(start, start + duration);

    public TimeSpan Duration => End - Start;

    public bool Overlaps(RentalPeriod otherPeriod) => Start < otherPeriod.End && otherPeriod.Start < End;
}

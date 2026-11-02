namespace HallRental.Utils;

public static class TimeSpanUtils
{
    // Ticks keep the value exact, unlike TotalHours (double)
    public static decimal ToHours(this TimeSpan time) => (decimal)time.Ticks / TimeSpan.TicksPerHour;

}

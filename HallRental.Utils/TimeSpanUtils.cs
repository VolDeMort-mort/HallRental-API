namespace HallRental.Utils;

public static class TimeSpanUtils
{
    public static decimal ToHours(this TimeSpan time) => (decimal)time.Ticks / TimeSpan.TicksPerHour;
}

namespace HallRental.Utils;

public static class DateTimeUtils
{
    public static DateTime Later(this DateTime a, DateTime b) => a > b ? a : b;

    public static DateTime Earlier(this DateTime a, DateTime b) => a < b ? a : b;
}


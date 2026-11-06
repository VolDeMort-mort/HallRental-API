namespace HallRental.Bll.Common.Halls.Models;

public record AvailableHallsFilter(DateTime Start, DateTime End, int MinCapacity);

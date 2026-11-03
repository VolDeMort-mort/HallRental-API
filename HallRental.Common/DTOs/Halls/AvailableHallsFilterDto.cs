namespace HallRental.Common.DTOs.Halls;

public record AvailableHallsFilterDto(DateTime Start, DateTime End, int MinCapacity);

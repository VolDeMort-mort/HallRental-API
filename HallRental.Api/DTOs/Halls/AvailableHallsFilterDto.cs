namespace HallRental.Api.DTOs.Halls;

public record AvailableHallsFilterDto(DateTime Start, DateTime End, int MinCapacity);

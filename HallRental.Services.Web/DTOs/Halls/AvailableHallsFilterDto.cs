namespace HallRental.Services.Web.DTOs.Halls;

public record AvailableHallsFilterDto(DateTime Start, DateTime End, int MinCapacity);

namespace HallRental.Services.Web.DTOs.Halls;

public record UpdateHallDto(string Name, int Capacity, decimal PricePerHour);

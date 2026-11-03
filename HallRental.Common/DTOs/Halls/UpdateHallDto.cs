namespace HallRental.Common.DTOs.Halls;

// No Id: it comes from the route, so the body can't point the update at another hall
public record UpdateHallDto(string Name, int Capacity, decimal PricePerHour);

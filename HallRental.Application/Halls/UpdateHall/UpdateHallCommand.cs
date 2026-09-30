using MediatR;

namespace HallRental.Application.Halls.UpdateHall;

public record UpdateHallCommand(
    Guid Id,
    string Name,
    int Capacity,
    decimal PricePerHour): IRequest;

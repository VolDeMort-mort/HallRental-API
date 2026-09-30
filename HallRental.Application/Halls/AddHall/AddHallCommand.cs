using MediatR;

namespace HallRental.Application.Halls.AddHall;

public record ServiceInput(
    string Name,
    decimal Price
);

public record AddHallCommand(
    string Name,
    int Capacity,
    decimal PricePerHour,
    IReadOnlyList<ServiceInput> Services) : IRequest<Guid>;

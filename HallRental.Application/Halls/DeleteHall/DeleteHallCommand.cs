using MediatR;

namespace HallRental.Application.Halls.DeleteHall;

public record DeleteHallCommand(Guid Id): IRequest;

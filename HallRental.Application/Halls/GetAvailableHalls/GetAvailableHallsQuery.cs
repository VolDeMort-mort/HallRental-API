using MediatR;

namespace HallRental.Application.Halls.GetAvailableHalls;

public record GetAvailableHallsQuery(
    DateTime Start,
    DateTime End,
    int MinCapacity
): IRequest<IReadOnlyList<HallDto>>;

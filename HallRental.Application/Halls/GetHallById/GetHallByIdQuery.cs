using MediatR;

namespace HallRental.Application.Halls.GetHallById;

public record GetHallByIdQuery(Guid Id) : IRequest<HallDto>;

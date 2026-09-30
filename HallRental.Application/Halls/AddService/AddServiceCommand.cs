using MediatR;

namespace HallRental.Application.Halls.AddService;

public record AddServiceCommand(
    Guid HallId,
    string Name,
    decimal Price
): IRequest<Guid>;

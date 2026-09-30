using MediatR;
using HallRental.Application.Interfaces;
using HallRental.Application.Exceptions;
using HallRental.Domain.Entities;

namespace HallRental.Application.Halls.GetHallById;

public class GetHallByIdQueryHandler : IRequestHandler<GetHallByIdQuery, HallDto>
{
    private readonly IHallRepository _halls;

    public GetHallByIdQueryHandler(IHallRepository halls)
    {
        _halls = halls;
    }

    public async Task<HallDto> Handle(GetHallByIdQuery request, CancellationToken cancellationToken)
    {
        Hall? hall = await _halls.GetByIdAsync(request.Id, cancellationToken);

        if (hall == null)
            throw new NotFoundException("Hall was not found");

        return hall.ToDto();
    }
}

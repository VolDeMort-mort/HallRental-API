using MediatR;
using HallRental.Application.Interfaces;
using HallRental.Application.Exceptions;
using HallRental.Domain.Entities;

namespace HallRental.Application.Halls.UpdateHall;

public class UpdateHallCommandHandler: IRequestHandler<UpdateHallCommand>
{
    private readonly IHallRepository _halls;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateHallCommandHandler(IHallRepository halls, IUnitOfWork unitOfWork)
    {
        _halls = halls;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateHallCommand request, CancellationToken cancellationToken)
    {
        Hall? hall = await _halls.GetByIdAsync(request.Id, cancellationToken);

        if (hall == null)
            throw new NotFoundException("Hall was not found");

        hall.Rename(request.Name);
        hall.ChangeCapacity(request.Capacity);
        hall.ChangePrice(request.PricePerHour);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

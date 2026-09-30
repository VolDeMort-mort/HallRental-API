using MediatR;
using HallRental.Application.Interfaces;
using HallRental.Application.Exceptions;
using HallRental.Domain.Entities;

namespace HallRental.Application.Halls.AddService;

public class AddServiceCommandHandler : IRequestHandler<AddServiceCommand, Guid>
{
    private readonly IHallRepository _halls;
    private readonly IUnitOfWork _unitOfWork;

    public AddServiceCommandHandler(IHallRepository halls, IUnitOfWork unitOfWork)
    {
        _halls = halls;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(AddServiceCommand request, CancellationToken cancellationToken)
    {
        Hall? hall = await _halls.GetByIdAsync(request.HallId, cancellationToken);

        if (hall == null)
            throw new NotFoundException("Hall was not found");

        var service = Service.Create(request.Name, request.Price);
        hall.AddService(service);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return service.Id;
    }
}

using MediatR;
using HallRental.Application.Interfaces;
using HallRental.Domain.Entities;

namespace HallRental.Application.Halls.AddHall;

public class AddHallCommandHandler : IRequestHandler<AddHallCommand, Guid>
{
    private readonly IHallRepository _halls;
    private readonly IUnitOfWork _unitOfWork;

    public AddHallCommandHandler(IHallRepository halls, IUnitOfWork unitOfWork)
    {
        _halls = halls;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(AddHallCommand request, CancellationToken cancellationToken)
    {
        var hall = Hall.Create(request.Name, request.Capacity, request.PricePerHour);

        foreach (ServiceInput s in request.Services)
        {
            hall.AddService(Service.Create(s.Name, s.Price));
        }

        await _halls.AddAsync(hall, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return hall.Id;
    }
}

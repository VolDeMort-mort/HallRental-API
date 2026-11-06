using HallRental.Bll.Common.Halls.Models;

namespace HallRental.Bll.Common.Halls;

public interface IHallRepository
{
    Task AddAsync(Hall hall, CancellationToken cancellationToken);

    Task<Hall?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<Hall>> GetAvailableAsync(int minCapacity, DateTime start, DateTime end, CancellationToken cancellationToken);

    Task UpdateAsync(Hall hall, CancellationToken cancellationToken);

    Task AddServiceAsync(Service service, CancellationToken cancellationToken);

    Task<bool> HasFutureBookingsAsync(Guid hallId, DateTime now, CancellationToken cancellationToken);
}

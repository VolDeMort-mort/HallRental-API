using HallRental.Bll.Common.Halls.Models;

namespace HallRental.Bll.Common.Halls;

public interface IHallManager
{
    Task<Guid> CreateAsync(Hall hall, CancellationToken cancellationToken);

    Task<Hall> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task UpdateAsync(Hall hall, CancellationToken cancellationToken);

    Task<Guid> AddServiceAsync(Guid hallId, Service service, CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<Hall>> GetAvailableAsync(DateTime start, DateTime end, int minCapacity, CancellationToken cancellationToken);
}

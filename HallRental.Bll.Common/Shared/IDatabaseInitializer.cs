namespace HallRental.Bll.Common.Shared;

public interface IDatabaseInitializer
{
    Task InitializeAsync(CancellationToken cancellationToken);
}

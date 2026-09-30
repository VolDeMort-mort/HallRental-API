namespace HallRental.Application.Interfaces;

public interface ICurrentUser
{
    string Id { get; }

    bool IsAdmin { get; }
}

namespace HallRental.Application.Interfaces;

/// <summary>The person who sent the current request.</summary>
public interface ICurrentUser
{
    string Id { get; }

    bool IsAdmin { get; }
}

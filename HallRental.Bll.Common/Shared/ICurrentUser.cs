namespace HallRental.Bll.Common.Shared;

public interface ICurrentUser
{
    string Id { get; }

    bool IsAdmin { get; }
}

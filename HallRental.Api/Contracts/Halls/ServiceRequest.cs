using HallRental.Application.Halls.AddService;

namespace HallRental.Api.Contracts.Halls;

public sealed record ServiceRequest
{
    /// <example>Звук</example>
    public required string Name { get; init; }

    /// <summary>Price in UAH, charged once per booking.</summary>
    /// <example>700</example>
    public required decimal Price { get; init; }

    public AddServiceCommand ToCommand(Guid hallId) => new(hallId, Name, Price);
}

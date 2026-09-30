using HallRental.Domain.Exceptions;

namespace HallRental.Domain.Entities;

/// <summary>
/// An extra service offered by a hall (projector, Wi-Fi, sound).
/// The price is charged once per booking, not per hour.
/// </summary>
public class Service
{
    /// <summary>Shared by the domain check, the request validators and the database column.</summary>
    public const int MaxNameLength = 100;

    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public decimal Price { get; private set; }

    private Service(){}

    public static Service Create(string name, decimal price)
    {
        var service = new Service{Id = Guid.NewGuid() };
        service.ChangeName(name);
        service.ChangePrice(price);
        return service;
    }

    public void ChangeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Service name can't be empty");
        if (name.Trim().Length > MaxNameLength)
            throw new DomainException($"Service name can't be longer than {MaxNameLength} characters");
        Name = name.Trim();
    }

    public void ChangePrice(decimal price)
    {
        if (price <= 0)
            throw new DomainException("Service price has to be greater than 0");
        Price = price;
    }

    /// <summary>"Projector" and " projector " are the same service for a human.</summary>
    public bool HasName(string name) => string.Equals(Name, name.Trim(), StringComparison.OrdinalIgnoreCase);
}

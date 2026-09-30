using HallRental.Domain.Exceptions;

namespace HallRental.Domain.Entities;

public class Hall
{
    public const int MaxNameLength = 100;

    private readonly List<Service> _services = new();

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int Capacity { get; private set; }
    public decimal PricePerHour { get; private set; }
    public bool IsActive { get; private set; }
    public IReadOnlyCollection<Service> Services => _services.AsReadOnly();

    private Hall(){}

    public static Hall Create(string name, int capacity, decimal pricePerHour)
    {
        var hall = new Hall {Id = Guid.NewGuid(), IsActive = true };
        hall.Rename(name);
        hall.ChangeCapacity(capacity);
        hall.ChangePrice(pricePerHour);
        return hall;
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Hall name cant be empty");
        if (name.Trim().Length > MaxNameLength)
            throw new DomainException($"Hall name can't be longer than {MaxNameLength} characters");
        Name = name.Trim();
    }

    public void ChangeCapacity(int capacity)
    {
        if (capacity <= 0)
            throw new DomainException("Hall capacity has to be greater than 0");
        Capacity = capacity;
    }

    public void ChangePrice(decimal price)
    {
        if (price <= 0)
            throw new DomainException("Hall price per hour has to be greater than 0");
        PricePerHour = price;
    }

    // Soft delete: past bookings stay for history and reports
    public void Deactivate()
    {
        IsActive = false;
    }

    public void AddService(Service service)
    {
        if (_services.Any(s => s.HasName(service.Name)))
            throw new DomainException($"Service {service.Name} already in the service list");

        _services.Add(service);
    }

    public IReadOnlyList<Service> SelectServices(IReadOnlyCollection<Guid> serviceIds)
    {
        var requestedIds = serviceIds.Distinct().ToList();
        var selected = _services.Where(s => requestedIds.Contains(s.Id)).ToList();

        if (selected.Count != requestedIds.Count)
            throw new DomainException("Some of the selected services are not available in this hall");

        return selected;
    }
}

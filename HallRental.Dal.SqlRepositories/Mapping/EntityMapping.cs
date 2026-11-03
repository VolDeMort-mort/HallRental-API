using HallRental.Bll.Common.Bookings.Models;
using HallRental.Bll.Common.Halls.Models;
using HallRental.Bll.Common.Pricing.Models;
using HallRental.Dal.SqlRepositories.Bookings.Entities;
using HallRental.Dal.SqlRepositories.Halls.Entities;
using HallRental.Dal.SqlRepositories.Pricing.Entities;

namespace HallRental.Dal.SqlRepositories.Mapping;

internal static class EntityMapping
{
    public static Hall ToModel(this HallEntity entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Capacity = entity.Capacity,
        PricePerHour = entity.PricePerHour,
        IsActive = entity.IsActive,
        Services = entity.Services.Select(s => s.ToModel()).ToList()
    };

    public static HallEntity ToEntity(this Hall model) => new()
    {
        Id = model.Id,
        Name = model.Name,
        Capacity = model.Capacity,
        PricePerHour = model.PricePerHour,
        IsActive = model.IsActive,
        Services = model.Services.Select(s => s.ToEntity()).ToList()
    };

    public static Service ToModel(this ServiceEntity entity) => new()
    {
        Id = entity.Id,
        HallId = entity.HallId,
        Name = entity.Name,
        Price = entity.Price
    };

    public static ServiceEntity ToEntity(this Service model) => new()
    {
        Id = model.Id,
        HallId = model.HallId,
        Name = model.Name,
        Price = model.Price
    };

    public static Booking ToModel(this BookingEntity entity, string hallName) => new()
    {
        Id = entity.Id,
        HallId = entity.HallId,
        HallName = hallName,
        ClientId = entity.ClientId,
        Start = entity.Start,
        End = entity.End,
        RentPrice = entity.RentPrice,
        ServicesPrice = entity.ServicesPrice,
        Services = entity.Services
            .Select(s => new BookedService { ServiceId = s.ServiceId, Name = s.Name, Price = s.Price })
            .ToList()
    };

    public static BookingEntity ToEntity(this Booking model) => new()
    {
        Id = model.Id,
        HallId = model.HallId,
        ClientId = model.ClientId,
        Start = model.Start,
        End = model.End,
        RentPrice = model.RentPrice,
        ServicesPrice = model.ServicesPrice,
        Services = model.Services
            .Select(s => new BookedServiceEntity { ServiceId = s.ServiceId, Name = s.Name, Price = s.Price })
            .ToList()
    };

    public static HoursPricing ToModel(this HoursPricingEntity entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        From = entity.From,
        To = entity.To,
        Multiplier = entity.Multiplier
    };
}

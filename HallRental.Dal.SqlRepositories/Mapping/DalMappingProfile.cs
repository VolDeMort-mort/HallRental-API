using AutoMapper;
using HallRental.Bll.Common.Bookings.Models;
using HallRental.Bll.Common.Halls.Models;
using HallRental.Bll.Common.Pricing.Models;
using HallRental.Dal.SqlRepositories.Bookings.Entities;
using HallRental.Dal.SqlRepositories.Halls.Entities;
using HallRental.Dal.SqlRepositories.Pricing.Entities;

namespace HallRental.Dal.SqlRepositories.Mapping;

internal class DalMappingProfile : Profile
{
    public DalMappingProfile()
    {
        CreateMap<HallEntity, Hall>().ReverseMap();
        CreateMap<ServiceEntity, Service>().ReverseMap();

        CreateMap<BookingEntity, Booking>()
            .ForMember(d => d.HallName, o => o.Ignore());
        CreateMap<Booking, BookingEntity>();
        CreateMap<BookedServiceEntity, BookedService>().ReverseMap();

        CreateMap<HoursPricingEntity, HoursPricing>();
    }
}

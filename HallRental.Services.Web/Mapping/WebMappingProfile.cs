using AutoMapper;
using HallRental.Bll.Common.Bookings.Models;
using HallRental.Bll.Common.Halls.Models;
using HallRental.Bll.Common.Reports.Models;
using HallRental.Services.Web.DTOs.Bookings;
using HallRental.Services.Web.DTOs.Halls;
using HallRental.Services.Web.DTOs.Reports;

namespace HallRental.Services.Web.Mapping;

internal class WebMappingProfile : Profile
{
    public WebMappingProfile()
    {
        CreateMap<CreateHallDto, Hall>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.IsActive, o => o.Ignore());

        CreateMap<UpdateHallDto, Hall>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.IsActive, o => o.Ignore())
            .ForMember(d => d.Services, o => o.Ignore());

        CreateMap<CreateServiceDto, Service>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.HallId, o => o.Ignore());

        CreateMap<Hall, HallDto>()
            .ForCtorParam(nameof(HallDto.Services), o => o.MapFrom(s => s.Services.OrderBy(x => x.Name, StringComparer.Ordinal)));
        CreateMap<Service, ServiceDto>();

        CreateMap<CreateBookingDto, NewBooking>();

        CreateMap<Booking, BookingDto>()
            .ForCtorParam(nameof(BookingDto.Services), o => o.MapFrom(s => s.Services.OrderBy(x => x.Name, StringComparer.Ordinal)));
        CreateMap<BookedService, BookedServiceDto>();

        CreateMap<ReportPeriodDto, ReportPeriod>();
        CreateMap<HallsReport, HallsReportDto>();
        CreateMap<HallReport, HallReportDto>();
        CreateMap<HallsReportTotal, HallsReportTotalDto>();
        CreateMap<ServicesReport, ServicesReportDto>();
        CreateMap<ServiceReport, ServiceReportDto>();
    }
}

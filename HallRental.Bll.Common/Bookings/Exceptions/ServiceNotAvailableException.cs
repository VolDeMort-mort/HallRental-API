using HallRental.Bll.Common.Shared.Exceptions;

namespace HallRental.Bll.Common.Bookings.Exceptions;

public sealed class ServiceNotAvailableException()
    : BusinessRuleException("Some of the selected services are not available in this hall");

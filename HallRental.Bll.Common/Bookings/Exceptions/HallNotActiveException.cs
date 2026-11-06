using HallRental.Bll.Common.Shared.Exceptions;

namespace HallRental.Bll.Common.Bookings.Exceptions;

public sealed class HallNotActiveException()
    : BusinessRuleException("A deleted hall can't be booked");

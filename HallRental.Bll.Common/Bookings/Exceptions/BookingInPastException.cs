using HallRental.Bll.Common.Shared.Exceptions;

namespace HallRental.Bll.Common.Bookings.Exceptions;

public sealed class BookingInPastException()
    : BusinessRuleException("A booking can't start in the past");

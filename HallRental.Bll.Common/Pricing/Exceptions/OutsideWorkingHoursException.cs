using HallRental.Bll.Common.Shared.Exceptions;

namespace HallRental.Bll.Common.Pricing.Exceptions;

public sealed class OutsideWorkingHoursException()
    : BusinessRuleException("The hall can be booked only during working hours");

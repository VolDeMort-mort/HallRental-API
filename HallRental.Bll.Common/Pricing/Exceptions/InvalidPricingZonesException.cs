using HallRental.Bll.Common.Shared.Exceptions;

namespace HallRental.Bll.Common.Pricing.Exceptions;

public sealed class InvalidPricingZonesException(string message)
    : BusinessRuleException(message);

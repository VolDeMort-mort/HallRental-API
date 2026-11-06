using HallRental.Bll.Common.Shared.Exceptions;

namespace HallRental.Bll.Common.Bookings.Exceptions;

public sealed class InvalidClientException()
    : BusinessRuleException("A booking needs a valid client");

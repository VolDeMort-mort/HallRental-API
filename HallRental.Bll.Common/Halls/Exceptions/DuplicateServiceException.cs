using HallRental.Bll.Common.Shared.Exceptions;

namespace HallRental.Bll.Common.Halls.Exceptions;

public sealed class DuplicateServiceException(string serviceName)
    : BusinessRuleException($"Service {serviceName} already in the service list");

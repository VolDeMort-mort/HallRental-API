namespace HallRental.Bll.Common.Shared.Exceptions;

public sealed class NotFoundException(string message) : Exception(message);

using System.Security.Claims;
using HallRental.Application.Interfaces;

namespace HallRental.Api.Security;

public sealed class HttpCurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpCurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal User => _httpContextAccessor.HttpContext?.User ?? new ClaimsPrincipal();

    public string Id => User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new InvalidOperationException("The request has no authenticated user");

    public bool IsAdmin => User.IsInRole(Roles.Admin);
}

using HallRental.Api.Security;
using HallRental.Application.Reports.HallsReport;
using HallRental.Application.Reports.ServicesReport;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HallRental.Api.Controllers;

/// <summary>Business reports (admin only); the period [from, to) excludes "to", e.g. from=2030-09-01&amp;to=2030-10-01.</summary>
[ApiController]
[Route("api/reports")]
[Produces("application/json")]
[Authorize(Roles = Roles.Admin)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
public class ReportsController : ControllerBase
{
    private readonly ISender _sender;

    public ReportsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>Bookings, occupancy and revenue of every hall.</summary>
    [HttpGet("halls")]
    [ProducesResponseType(typeof(HallsReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<HallsReportDto>> Halls([FromQuery] DateOnly from, [FromQuery] DateOnly to, CancellationToken cancellationToken)
    {
        return Ok(await _sender.Send(new GetHallsReportQuery(from, to), cancellationToken));
    }

    /// <summary>How often each service was ordered and how much it earned.</summary>
    [HttpGet("services")]
    [ProducesResponseType(typeof(ServicesReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ServicesReportDto>> Services([FromQuery] DateOnly from, [FromQuery] DateOnly to, CancellationToken cancellationToken)
    {
        return Ok(await _sender.Send(new GetServicesReportQuery(from, to), cancellationToken));
    }
}

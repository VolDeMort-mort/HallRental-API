using HallRental.Api.Security;
using HallRental.Application.Reports.HallsReport;
using HallRental.Application.Reports.ServicesReport;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HallRental.Api.Controllers;

/// <summary>Business reports for a period of days (admin only).</summary>
/// <remarks>
/// The period is [from, to): "from" is included, "to" is not, so a month is from=2030-09-01&amp;to=2030-10-01.
/// A booking belongs to the period by its start. The period can't be longer than 366 days.
/// </remarks>
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
    /// <remarks>
    /// Shows which halls are idle and which earn the most. Occupancy is booked hours out of the working hours
    /// of the whole period (working hours come from the tariff zones). Deleted halls are listed with
    /// isActive = false if they had bookings in the period.
    /// </remarks>
    /// <param name="from" example="2030-09-01">First day of the period (included).</param>
    /// <param name="to" example="2030-10-01">Day after the period (not included).</param>
    /// <param name="cancellationToken">Cancelled when the client disconnects.</param>
    /// <response code="200">The report.</response>
    /// <response code="400">The period is invalid or too long.</response>
    [HttpGet("halls")]
    [ProducesResponseType(typeof(HallsReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<HallsReportDto>> Halls([FromQuery] DateOnly from, [FromQuery] DateOnly to, CancellationToken cancellationToken)
    {
        return Ok(await _sender.Send(new GetHallsReportQuery(from, to), cancellationToken));
    }

    /// <summary>How often each service was ordered and how much it earned.</summary>
    /// <remarks>
    /// Counted from the copies stored in the bookings, at the prices of the booking moment.
    /// The same service in different halls is counted together.
    /// </remarks>
    /// <param name="from" example="2030-09-01">First day of the period (included).</param>
    /// <param name="to" example="2030-10-01">Day after the period (not included).</param>
    /// <param name="cancellationToken">Cancelled when the client disconnects.</param>
    /// <response code="200">The report.</response>
    /// <response code="400">The period is invalid or too long.</response>
    [HttpGet("services")]
    [ProducesResponseType(typeof(ServicesReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ServicesReportDto>> Services([FromQuery] DateOnly from, [FromQuery] DateOnly to, CancellationToken cancellationToken)
    {
        return Ok(await _sender.Send(new GetServicesReportQuery(from, to), cancellationToken));
    }
}

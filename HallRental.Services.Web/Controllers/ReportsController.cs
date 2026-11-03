using HallRental.Bll.Common.Reports;
using HallRental.Bll.Common.Shared.Constants;
using HallRental.Services.Web.DTOs.Reports;
using HallRental.Services.Web.Mapping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HallRental.Services.Web.Controllers;

/// <summary>Business reports (admin only); the period [from, to) excludes "to", e.g. from=2030-09-01&amp;to=2030-10-01.</summary>
[ApiController]
[Route("api/reports")]
[Produces("application/json")]
[Authorize(Roles = Roles.Admin)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
public class ReportsController : ControllerBase
{
    private readonly IReportManager _reportManager;

    public ReportsController(IReportManager reportManager)
    {
        _reportManager = reportManager;
    }

    /// <summary>Bookings, occupancy and revenue of every hall.</summary>
    [HttpGet("halls")]
    [ProducesResponseType(typeof(HallsReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<HallsReportDto>> Halls([FromQuery] ReportPeriodDto period, CancellationToken cancellationToken)
    {
        var report = await _reportManager.GetHallsReportAsync(period.ToModel(), cancellationToken);
        return Ok(report.ToDto());
    }

    /// <summary>How often each service was ordered and how much it earned.</summary>
    [HttpGet("services")]
    [ProducesResponseType(typeof(ServicesReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ServicesReportDto>> Services([FromQuery] ReportPeriodDto period, CancellationToken cancellationToken)
    {
        var report = await _reportManager.GetServicesReportAsync(period.ToModel(), cancellationToken);
        return Ok(report.ToDto());
    }
}

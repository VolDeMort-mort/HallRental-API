using HallRental.Api.Contracts;
using HallRental.Api.Contracts.Halls;
using HallRental.Api.Security;
using HallRental.Application.Halls;
using HallRental.Application.Halls.DeleteHall;
using HallRental.Application.Halls.GetAvailableHalls;
using HallRental.Application.Halls.GetHallById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HallRental.Api.Controllers;

/// <summary>Conference halls: management (admin only) and a public availability search.</summary>
[ApiController]
[Route("api/halls")]
[Produces("application/json")]
[Authorize(Roles = Roles.Admin)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
public class HallsController : ControllerBase
{
    private readonly ISender _sender;

    public HallsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>Adds a conference hall with its services.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreatedResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreatedResponse>> Create(CreateHallRequest request, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(request.ToCommand(), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, new CreatedResponse(id));
    }

    /// <summary>Returns a hall with its services and their ids.</summary>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(HallDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HallDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        return Ok(await _sender.Send(new GetHallByIdQuery(id), cancellationToken));
    }

    /// <summary>Changes the name, capacity and price per hour; existing bookings keep their price.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, UpdateHallRequest request, CancellationToken cancellationToken)
    {
        await _sender.Send(request.ToCommand(id), cancellationToken);
        return NoContent();
    }

    /// <summary>Adds a service to a hall.</summary>
    [HttpPost("{hallId:guid}/services")]
    [ProducesResponseType(typeof(CreatedResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CreatedResponse>> AddService(Guid hallId, ServiceRequest request, CancellationToken cancellationToken)
    {
        var serviceId = await _sender.Send(request.ToCommand(hallId), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = hallId }, new CreatedResponse(serviceId));
    }

    /// <summary>Soft-deletes a hall; a hall with upcoming bookings can't be deleted (409).</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _sender.Send(new DeleteHallCommand(id), cancellationToken);
        return NoContent();
    }

    /// <summary>Finds free halls for the people count; times are the hall's local time, e.g. 2030-09-01T10:00:00.</summary>
    [HttpGet("available")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyList<HallDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<HallDto>>> SearchAvailable(
        [FromQuery] DateTime start,
        [FromQuery] DateTime end,
        [FromQuery] int minCapacity,
        CancellationToken cancellationToken)
    {
        return Ok(await _sender.Send(new GetAvailableHallsQuery(start, end, minCapacity), cancellationToken));
    }
}

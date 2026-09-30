using HallRental.Api.Contracts;
using HallRental.Api.Contracts.Halls;
using HallRental.Application.Halls;
using HallRental.Application.Halls.DeleteHall;
using HallRental.Application.Halls.GetAvailableHalls;
using HallRental.Application.Halls.GetHallById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HallRental.Api.Controllers;

/// <summary>Conference halls: management and availability search.</summary>
/// <remarks>
/// The controller only translates HTTP into commands and back. Business rules live in the Domain,
/// errors are turned into 400 / 404 / 409 by the global exception handler.
/// </remarks>
[ApiController]
[Route("api/halls")]
[Produces("application/json")]
public class HallsController : ControllerBase
{
    private readonly ISender _sender;

    public HallsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>Adds a conference hall with its services.</summary>
    /// <response code="201">The hall is created; the Location header points to it.</response>
    /// <response code="400">The request is invalid.</response>
    [HttpPost]
    [ProducesResponseType(typeof(CreatedResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreatedResponse>> Create(CreateHallRequest request, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(request.ToCommand(), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, new CreatedResponse(id));
    }

    /// <summary>Returns a hall with its services and their ids.</summary>
    /// <param name="id">Hall id.</param>
    /// <param name="cancellationToken">Cancelled when the client disconnects.</param>
    /// <response code="200">The hall.</response>
    /// <response code="404">There is no such hall, or it was deleted.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(HallDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HallDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        return Ok(await _sender.Send(new GetHallByIdQuery(id), cancellationToken));
    }

    /// <summary>Changes the name, capacity and price per hour of a hall.</summary>
    /// <remarks>Existing bookings keep the price they were made with.</remarks>
    /// <param name="id">Hall id.</param>
    /// <param name="request">New data of the hall.</param>
    /// <param name="cancellationToken">Cancelled when the client disconnects.</param>
    /// <response code="204">The hall is updated.</response>
    /// <response code="400">The request is invalid.</response>
    /// <response code="404">There is no such hall, or it was deleted.</response>
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
    /// <param name="hallId">Hall id.</param>
    /// <param name="request">The new service.</param>
    /// <param name="cancellationToken">Cancelled when the client disconnects.</param>
    /// <response code="201">The service is added; the Location header points to the hall, which lists its services.</response>
    /// <response code="400">The request is invalid or the hall already has a service with this name.</response>
    /// <response code="404">There is no such hall, or it was deleted.</response>
    [HttpPost("{hallId:guid}/services")]
    [ProducesResponseType(typeof(CreatedResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CreatedResponse>> AddService(Guid hallId, ServiceRequest request, CancellationToken cancellationToken)
    {
        var serviceId = await _sender.Send(request.ToCommand(hallId), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = hallId }, new CreatedResponse(serviceId));
    }

    /// <summary>Deletes a hall.</summary>
    /// <remarks>
    /// Soft delete: the hall disappears from the search and can't be booked, but its past bookings stay for history
    /// and reports. A hall with upcoming bookings can't be deleted.
    /// </remarks>
    /// <param name="id">Hall id.</param>
    /// <param name="cancellationToken">Cancelled when the client disconnects.</param>
    /// <response code="204">The hall is deleted.</response>
    /// <response code="404">There is no such hall, or it was deleted already.</response>
    /// <response code="409">The hall has upcoming bookings.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _sender.Send(new DeleteHallCommand(id), cancellationToken);
        return NoContent();
    }

    /// <summary>Finds halls that are free for the whole period and fit the number of people.</summary>
    /// <param name="start" example="2030-09-01T10:00:00">Start of the period, hall's local time.</param>
    /// <param name="end" example="2030-09-01T14:00:00">End of the period, hall's local time.</param>
    /// <param name="minCapacity" example="50">Number of people the hall has to fit.</param>
    /// <param name="cancellationToken">Cancelled when the client disconnects.</param>
    /// <response code="200">Free halls ordered by name; an empty list if there are none.</response>
    /// <response code="400">The period or the number of people is invalid.</response>
    [HttpGet("available")]
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

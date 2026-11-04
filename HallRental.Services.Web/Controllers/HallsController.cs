using AutoMapper;
using HallRental.Bll.Common.Halls;
using HallRental.Bll.Common.Halls.Models;
using HallRental.Bll.Common.Shared.Constants;
using HallRental.Services.Web.DTOs;
using HallRental.Services.Web.DTOs.Halls;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HallRental.Services.Web.Controllers;

/// <summary>Conference halls: management (admin only) and a public availability search.</summary>
[ApiController]
[Route("api/halls")]
[Produces("application/json")]
[Authorize(Roles = Roles.Admin)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
public class HallsController : ControllerBase
{
    private readonly IHallManager _hallManager;
    private readonly IMapper _mapper;

    public HallsController(IHallManager hallManager, IMapper mapper)
    {
        _hallManager = hallManager;
        _mapper = mapper;
    }

    /// <summary>Adds a conference hall with its services.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreatedResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreatedResponse>> Create(CreateHallDto request, CancellationToken cancellationToken)
    {
        var id = await _hallManager.CreateAsync(_mapper.Map<Hall>(request), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, new CreatedResponse(id));
    }

    /// <summary>Returns a hall with its services and their ids.</summary>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(HallDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HallDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var hall = await _hallManager.GetByIdAsync(id, cancellationToken);
        return Ok(_mapper.Map<HallDto>(hall));
    }

    /// <summary>Changes the name, capacity and price per hour; existing bookings keep their price.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, UpdateHallDto request, CancellationToken cancellationToken)
    {
        var hall = _mapper.Map<Hall>(request);
        hall.Id = id;

        await _hallManager.UpdateAsync(hall, cancellationToken);
        return NoContent();
    }

    /// <summary>Adds a service to a hall.</summary>
    [HttpPost("{hallId:guid}/services")]
    [ProducesResponseType(typeof(CreatedResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CreatedResponse>> AddService(Guid hallId, CreateServiceDto request, CancellationToken cancellationToken)
    {
        var serviceId = await _hallManager.AddServiceAsync(hallId, _mapper.Map<Service>(request), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = hallId }, new CreatedResponse(serviceId));
    }

    /// <summary>Soft-deletes a hall; a hall with upcoming bookings can't be deleted (409).</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _hallManager.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    /// <summary>Finds free halls for the people count; times are the hall's local time, e.g. 2030-09-01T10:00:00.</summary>
    [HttpGet("available")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyList<HallDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<HallDto>>> SearchAvailable(
        [FromQuery] AvailableHallsFilterDto filter,
        CancellationToken cancellationToken)
    {
        var halls = await _hallManager.GetAvailableAsync(filter.Start, filter.End, filter.MinCapacity, cancellationToken);
        return Ok(_mapper.Map<List<HallDto>>(halls));
    }
}

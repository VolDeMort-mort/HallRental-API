using HallRental.Bll.Common.Bookings;
using HallRental.Services.Web.DTOs.Bookings;
using HallRental.Services.Web.Mapping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HallRental.Services.Web.Controllers;

/// <summary>Hall bookings; a client sees only their own bookings, an admin sees all.</summary>
[ApiController]
[Route("api/bookings")]
[Produces("application/json")]
[Authorize]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class BookingsController : ControllerBase
{
    private readonly IBookingManager _bookingManager;

    public BookingsController(IBookingManager bookingManager)
    {
        _bookingManager = bookingManager;
    }

    /// <summary>Books a hall and returns the confirmation with the total cost.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(BookingDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<BookingDto>> Create(CreateBookingDto request, CancellationToken cancellationToken)
    {
        var booking = await _bookingManager.CreateAsync(request.ToModel(), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = booking.Id }, booking.ToDto());
    }

    /// <summary>Returns a booking with its cost breakdown.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BookingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookingDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var booking = await _bookingManager.GetByIdAsync(id, cancellationToken);
        return Ok(booking.ToDto());
    }
}

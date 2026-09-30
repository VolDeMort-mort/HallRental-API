using HallRental.Api.Contracts.Bookings;
using HallRental.Application.Bookings;
using HallRental.Application.Bookings.GetBookingById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HallRental.Api.Controllers;

/// <summary>Bookings of conference halls with the rent calculated by time of day.</summary>
/// <remarks>
/// Rent per hour: 06–09 −10%, 09–12 base price, 12–14 +15%, 14–18 base price, 18–23 −20%.
/// Services are charged once per booking.
/// </remarks>
[ApiController]
[Route("api/bookings")]
[Produces("application/json")]
public class BookingsController : ControllerBase
{
    private readonly ISender _sender;

    public BookingsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>Books a hall and returns the confirmation with the total cost.</summary>
    /// <param name="request">The hall, the period and the services.</param>
    /// <param name="cancellationToken">Cancelled when the client disconnects.</param>
    /// <response code="201">The booking is created; the body holds the cost breakdown.</response>
    /// <response code="400">The request is invalid, the period is in the past or outside working hours, or a service isn't offered by the hall.</response>
    /// <response code="404">There is no such hall, or it was deleted.</response>
    /// <response code="409">The hall is already booked for part of this period.</response>
    [HttpPost]
    [ProducesResponseType(typeof(BookingDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<BookingDto>> Create(CreateBookingRequest request, CancellationToken cancellationToken)
    {
        var booking = await _sender.Send(request.ToCommand(), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = booking.Id }, booking);
    }

    /// <summary>Returns a booking with its cost breakdown.</summary>
    /// <param name="id">Booking id.</param>
    /// <param name="cancellationToken">Cancelled when the client disconnects.</param>
    /// <response code="200">The booking.</response>
    /// <response code="404">There is no such booking.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BookingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookingDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        return Ok(await _sender.Send(new GetBookingByIdQuery(id), cancellationToken));
    }
}

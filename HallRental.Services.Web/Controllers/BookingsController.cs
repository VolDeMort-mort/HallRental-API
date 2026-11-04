using AutoMapper;
using HallRental.Bll.Common.Bookings;
using HallRental.Bll.Common.Bookings.Models;
using HallRental.Services.Web.DTOs.Bookings;
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
    private readonly IMapper _mapper;

    public BookingsController(IBookingManager bookingManager, IMapper mapper)
    {
        _bookingManager = bookingManager;
        _mapper = mapper;
    }

    /// <summary>Books a hall and returns the confirmation with the total cost.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(BookingDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<BookingDto>> Create(CreateBookingDto request, CancellationToken cancellationToken)
    {
        var booking = await _bookingManager.CreateAsync(_mapper.Map<NewBooking>(request), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = booking.Id }, _mapper.Map<BookingDto>(booking));
    }

    /// <summary>Returns a booking with its cost breakdown.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BookingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookingDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var booking = await _bookingManager.GetByIdAsync(id, cancellationToken);
        return Ok(_mapper.Map<BookingDto>(booking));
    }
}

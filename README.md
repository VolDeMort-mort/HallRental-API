# HallRental — Conference Hall Booking API

REST API for renting conference halls: hall management, free-hall search, bookings with time-of-day pricing
and business reports.

Business tasks and technical decisions: [DOCUMENTATION.md](DOCUMENTATION.md).

## Quick start

Requirements: .NET SDK 8+, SQL Server LocalDB (or change `ConnectionStrings:Default` in `appsettings.Development.json`).

```powershell
dotnet run --project HallRental.Api --launch-profile http
dotnet user-jwts create --project HallRental.Api --name admin --role Admin --output token
dotnet user-jwts create --project HallRental.Api --name client-1 --output token
```

On the first start the database is created with the initial data from the assignment.
Swagger: <http://localhost:5112/swagger> (use *Authorize* with a token).
`HallRental.Api.http` runs the whole scenario: put the tokens into `http-client.env.json.user` and select the `dev` environment.

## Business rules

| Hours | 06–09 | 09–12 | 12–14 | 14–18 | 18–23 |
|---|---|---|---|---|---|
| Rent per hour | −10% | base | +15% | base | −20% |

Example: Hall A (2000/h), 10:00–14:00 with a projector = 2 × 2000 + 2 × 2300 + 500 = **9100 UAH**.

Decisions where the assignment is silent:
- Services are charged once per booking.
- Peak hours override standard ones; 23:00–06:00 can't be booked.
- Times are the hall's local time without a UTC offset; bookings in the past are rejected.
- A booking keeps the prices of the booking moment.
- Deleting a hall is soft; a hall with upcoming bookings can't be deleted.

## API

| Endpoint | Access |
|---|---|
| `GET /api/halls/available`, `GET /api/halls/{id}` | public |
| `POST /api/bookings`, `GET /api/bookings/{id}` | client (own bookings only) |
| `POST/PUT/DELETE /api/halls`, `POST /api/halls/{id}/services` | admin |
| `GET /api/reports/halls`, `GET /api/reports/services` | admin |

Reports take `[from, to)`: revenue and occupancy per hall, popularity of services.

## Errors

All errors use `application/problem+json`.

| Code | When |
|---|---|
| 400 | Invalid request (every invalid field listed at once) or a broken business rule: past time, outside working hours, service not offered by the hall |
| 401 | No or invalid token |
| 403 | The action needs the `Admin` role |
| 404 | Hall or booking not found, the hall was deleted, or the booking belongs to someone else |
| 409 | The hall is already booked for this time, or a hall with upcoming bookings is being deleted |
| 429 | More than 100 requests a minute |
| 500 | Unexpected error; the response contains only a `traceId` to find it in the logs |

## Architecture

- **Clean Architecture:** Domain ← Application ← Infrastructure / Api.
- **Domain:** rich entities, value objects, `PricingPolicy` splits a booking by tariff zones.
- **Application:** CQRS with MediatR, FluentValidation in a pipeline behavior.
- **Infrastructure:** EF Core 8 + SQL Server; reports are aggregated by the database.
- **Api:** thin controllers, JWT with an `Admin` role, rate limiting, global exception handler.

## Known limitations

- Two simultaneous bookings of the same slot can both succeed (no serializable transaction yet).
- "Now" is the server's local time; a UTC server needs the halls' time zone configured.

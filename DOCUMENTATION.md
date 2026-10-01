# HallRental — Business Tasks and Technical Decisions

How to run the project: see [README.md](README.md).

## 1. Business tasks

A company rents out conference halls to businesses. The API serves two kinds of users:

- **Admin** — manages halls, their services and prices, and reads the reports.
- **Client** — a person who searches for free halls and books them.

| Task from the assignment | What the API does |
|---|---|
| Add a hall | Creates a hall with capacity, price per hour and its services |
| Edit a hall | Changes name, capacity and price; adds services |
| Delete a hall | Removes it from search and booking, keeps its history |
| Find free halls | Returns halls that fit the number of people and are free for the whole period |
| Book a hall | Books it and returns a confirmation with the rent, the services and the total |

### Pricing

| Hours | 06–09 | 09–12 | 12–14 | 14–18 | 18–23 |
|---|---|---|---|---|---|
| Rent per hour | −10% | base | +15% | base | −20% |

A booking is split by these zones and every part is priced with its own multiplier. Services are added once.
Example: Hall A (2000/h), 10:00–14:00 with a projector = 2 × 2000 + 2 × 2300 + 500 = **9100 UAH**.

### Reports

| Report | Shows | Decision it supports |
|---|---|---|
| Halls | bookings, booked hours, occupancy %, rent and services revenue per hall | which halls are idle (change the price, promote them) and which earn the most |
| Services | how often each service was ordered and how much it earned | which services to develop and which nobody needs |

Occupancy = booked hours ÷ working hours of the period. Deleted halls stay in the report while they have bookings
in the period, because their revenue is real.

### Gaps in the assignment

| Question | Decision | Why |
|---|---|---|
| Is a service charged per hour? | No, once per booking | The assignment gives a service a price without "per hour" |
| Peak hours (12–14) lie inside the standard ones (9–18) | The peak wins; the standard zone is split around it | A markup only makes sense if it replaces the base price; non-overlapping zones make every price unambiguous |
| 23:00–06:00 | Can't be booked | No price is defined for it, so it's treated as closed instead of guessing |
| Time zone | The hall's local time, without a UTC offset | The tariffs are defined by the hall's wall clock |
| Booking in the past | Rejected | It can't be fulfilled. The example date in the assignment (01.09.2024) is already in the past |
| A price changes after booking | The booking keeps its prices | A confirmed price must not change, and reports must show what was really earned |
| Deleting a hall with bookings | Soft delete; forbidden while upcoming bookings exist | Keeps the history for reports and protects clients who already booked |
| Who is a client | A person identified by the access token | Clients see only their own bookings |
| Rounding | To 2 decimals, midpoint away from zero | Commercial rounding to kopecks |
| Which hall offers which initial service | Every hall offers all three | The assignment lists services separately from halls |

## 2. Technical decisions

| Decision | Why | Alternative |
|---|---|---|
| Clean Architecture: Domain, Application, Infrastructure, Api | The assignment asks for a scalable solution: business rules don't depend on the database or the web framework, and each layer can be replaced or tested alone | One project: faster to write, but rules mix with EF and HTTP |
| CQRS with MediatR, one folder per use case | A new feature is a new folder; validation runs once in the pipeline for every request | Service classes with many methods |
| MediatR 12.5 | The last version under Apache 2.0; version 13+ needs a commercial license | Source-generated mediator or a hand-written dispatcher |
| Rich domain model with value objects | Rules live in one place, and a handler can't put an entity into an invalid state | Plain data classes checked in services |
| Pricing as a domain service, tariff zones stored as data | Testable without a database; the business can change tariffs without a release | Hard-coded `if` by hour |
| FluentValidation plus domain checks | The client gets every input error at once with field names; the domain keeps its rules whatever calls it; inputs that would crash (e.g. a `DateTime` overflow) never reach it | Domain exceptions only |
| SQL Server with EF Core 8 | Sums of money in SQL for reports, native `TimeOnly`, LocalDB needs no setup | SQLite can't sum `decimal` in SQL; PostgreSQL needs installing |
| Copies of service prices inside a booking | Confirmed bookings and reports stay correct after a price change | Joining current prices |
| Soft delete through a global query filter | History survives; one filter instead of a `Where` in every query | Hard delete breaks reports |
| Reports as a separate read model | The database groups and sums, only ready rows come back | Loading bookings into memory |
| One global exception handler, `problem+json` | One error format; a 500 hides internal details and returns a `traceId` for the logs | `try/catch` in every controller |
| JWT with an `Admin` role; someone else's booking returns 404 | Clients can't manage halls or see revenue; 404 doesn't reveal that the booking exists | API keys |
| Rate limiting (100 requests/min per user or IP) and caps on input (booking ≤ 1 day, report ≤ 366 days) | One client can't overload the API or the database | — |
| Retries of transient SQL errors | A dropped connection doesn't fail the request at once | — |

Development tokens come from `dotnet user-jwts`; in production an identity provider (Entra ID, Auth0, Keycloak)
would issue them without any change to the API.

## 3. Limitations and next steps

| Limitation | Next step |
|---|---|
| Two simultaneous bookings of the same slot can both succeed (10 of 20 in a test) | Run "is it free?" and the insert in one serializable transaction inside the retry strategy, or add a concurrency token on the hall |
| "Now" is the server's local time | Store the halls' time zone and convert explicitly |
| No automated tests | Unit tests for pricing and the domain, handler tests, API tests with a real database |
| LocalDB is Windows only, no CI | `docker-compose` with SQL Server, a build-and-test pipeline |
| Missing features | Cancelling a booking, "my bookings", a report by tariff zones to check whether the discounts work |

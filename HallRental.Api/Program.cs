using HallRental.Api.ErrorHandling;
using HallRental.Application;
using HallRental.Infrastructure;
using HallRental.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
// traceId lets a client report a 500 that can then be found in the logs
builder.Services.AddProblemDetails(options =>
    options.CustomizeProblemDetails = context =>
        context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// First in the pipeline, so it catches exceptions from everything registered after it
app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Creates or updates the local database and adds the initial data from the assignment.
    // In production migrations are applied as a separate deployment step, not on every start.
    await app.Services.InitializeDatabaseAsync();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

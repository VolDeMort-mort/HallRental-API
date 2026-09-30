using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.RateLimiting;
using HallRental.Api.ErrorHandling;
using HallRental.Api.Security;
using HallRental.Application;
using HallRental.Application.Interfaces;
using HallRental.Infrastructure;
using HallRental.Infrastructure.Persistence;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers()
    // Hall and service names are Cyrillic: keep them readable instead of "Зал"
    .AddJsonOptions(options => options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All));

builder.Services.AddAuthentication().AddJwtBearer();
builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, HttpCurrentUser>();

// 100 requests a minute per signed-in person, or per IP address for anonymous requests
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions { PermitLimit = 100, Window = TimeSpan.FromMinutes(1) }));
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
// traceId lets a client report a 500 that can then be found in the logs
builder.Services.AddProblemDetails(options =>
    options.CustomizeProblemDetails = context =>
        context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Controllers and request models are documented in the Api, response models in the Application
    var documentedAssemblies = new[] { typeof(Program).Assembly, typeof(HallRental.Application.DependencyInjection).Assembly };
    foreach (var assembly in documentedAssemblies)
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{assembly.GetName().Name}.xml"), includeControllerXmlComments: true);

    var bearer = new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
    };
    options.AddSecurityDefinition("Bearer", bearer);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement { [bearer] = Array.Empty<string>() });
});

var app = builder.Build();

// First in the pipeline, so it catches exceptions from everything registered after it
app.UseExceptionHandler();
app.UseStatusCodePages();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Creates or updates the local database and adds the initial data from the assignment.
    // In production migrations are applied as a separate deployment step, not on every start.
    await app.Services.InitializeDatabaseAsync();

    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseRateLimiter();
app.UseAuthorization();

app.MapControllers();

app.Run();

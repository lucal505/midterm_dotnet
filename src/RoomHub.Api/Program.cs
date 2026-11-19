using Microsoft.EntityFrameworkCore;
using RoomHub.Api.Common;
using RoomHub.Api.Domain;
using RoomHub.Api.Features.CancelBooking;
using RoomHub.Api.Features.CreateBooking;
using RoomHub.Api.Features.CreateRoom;
using RoomHub.Api.Features.GetBooking;
using RoomHub.Api.Features.GetRoom;
using RoomHub.Api.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Errors produced by the framework (bad JSON, unknown route) are also ProblemDetails.
builder.Services.AddProblemDetails();

// "Now" for the business rules (R5) always comes from TimeProvider.
builder.Services.AddSingleton(TimeProvider.System);

// Persistence: EF Core + SQLite. The database file lives next to the project,
// so data survives a restart. Can be overridden with ConnectionStrings:RoomHub.
var connectionString = builder.Configuration.GetConnectionString("RoomHub")
    ?? $"Data Source={Path.Combine(builder.Environment.ContentRootPath, "roomhub.db")}";
builder.Services.AddDbContext<RoomHubDbContext>(options => options.UseSqlite(connectionString));

builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Commands (write side).
builder.Services.AddScoped<ICommandHandler<CreateRoomCommand, CreateRoomResponse>, CreateRoomHandler>();
builder.Services.AddScoped<ICommandHandler<CreateBookingCommand, CreateBookingResponse>, CreateBookingHandler>();
builder.Services.AddScoped<ICommandHandler<CancelBookingCommand, CancelBookingResponse>, CancelBookingHandler>();

// Queries (read side).
builder.Services.AddScoped<IQueryHandler<GetRoomQuery, GetRoomResponse>, GetRoomHandler>();
builder.Services.AddScoped<IQueryHandler<GetBookingQuery, GetBookingResponse>, GetBookingHandler>();

var app = builder.Build();

// Create the SQLite database and tables on first run (existing data is kept).
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RoomHubDbContext>();
    db.Database.EnsureCreated();
}

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseMiddleware<ExceptionMiddleware>();

app.MapGet("/", () => "RoomHub API is running.");

app.MapCreateRoom();
app.MapGetRoom();
app.MapCreateBooking();
app.MapGetBooking();
app.MapCancelBooking();

app.Run();

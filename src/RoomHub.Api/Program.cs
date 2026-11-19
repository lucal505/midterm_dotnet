using RoomHub.Api.Common;
using RoomHub.Api.Features.CancelBooking;
using RoomHub.Api.Features.CreateBooking;
using RoomHub.Api.Features.CreateRoom;
using RoomHub.Api.Features.GetBooking;
using RoomHub.Api.Features.GetRoom;
using RoomHub.Api.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Errors produced by the framework (bad JSON, unknown route) are also ProblemDetails.
builder.Services.AddProblemDetails();

// Persistence (in-memory for now).
builder.Services.AddSingleton<RoomRepository>();
builder.Services.AddSingleton<BookingRepository>();

// Feature handlers (one per slice).
builder.Services.AddScoped<CreateRoomHandler>();
builder.Services.AddScoped<GetRoomHandler>();
builder.Services.AddScoped<CreateBookingHandler>();
builder.Services.AddScoped<GetBookingHandler>();
builder.Services.AddScoped<CancelBookingHandler>();

var app = builder.Build();

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

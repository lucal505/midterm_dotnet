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

// "Now" for the business rules (R5). Never DateTime.UtcNow.
builder.Services.AddSingleton(TimeProvider.System);

// Persistence (in-memory for now).
builder.Services.AddSingleton<RoomRepository>();
builder.Services.AddSingleton<BookingRepository>();

// Commands (write side).
builder.Services.AddScoped<ICommandHandler<CreateRoomCommand, CreateRoomResponse>, CreateRoomHandler>();
builder.Services.AddScoped<ICommandHandler<CreateBookingCommand, CreateBookingResponse>, CreateBookingHandler>();
builder.Services.AddScoped<ICommandHandler<CancelBookingCommand, CancelBookingResponse>, CancelBookingHandler>();

// Queries (read side).
builder.Services.AddScoped<IQueryHandler<GetRoomQuery, GetRoomResponse>, GetRoomHandler>();
builder.Services.AddScoped<IQueryHandler<GetBookingQuery, GetBookingResponse>, GetBookingHandler>();

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

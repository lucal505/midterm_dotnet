using RoomHub.Api.Common;

namespace RoomHub.Api.Features.CreateRoom;

public static class CreateRoomEndpoint
{
    public static void MapCreateRoom(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/rooms", async (
            CreateRoomCommand command,
            ICommandHandler<CreateRoomCommand, CreateRoomResponse> handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(command, ct);
            return result.IsSuccess
                ? Results.Created($"/api/rooms/{result.Value.Id}", result.Value)
                : result.Error!.ToProblem();
        }).WithTags("Rooms");
    }
}

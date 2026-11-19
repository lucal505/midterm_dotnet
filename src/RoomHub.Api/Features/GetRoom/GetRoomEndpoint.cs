using RoomHub.Api.Common;

namespace RoomHub.Api.Features.GetRoom;

public static class GetRoomEndpoint
{
    public static void MapGetRoom(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/rooms/{id:guid}", async (
            Guid id,
            IQueryHandler<GetRoomQuery, GetRoomResponse> handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetRoomQuery(id), ct);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : result.Error!.ToProblem();
        }).WithTags("Rooms");
    }
}

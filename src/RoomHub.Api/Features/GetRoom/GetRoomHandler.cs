using RoomHub.Api.Common;
using RoomHub.Api.Persistence;

namespace RoomHub.Api.Features.GetRoom;

public sealed class GetRoomHandler(RoomRepository rooms)
    : IQueryHandler<GetRoomQuery, GetRoomResponse>
{
    public Task<Result<GetRoomResponse>> Handle(GetRoomQuery query, CancellationToken cancellationToken)
    {
        var room = rooms.Get(query.Id);
        Result<GetRoomResponse> result = room is null
            ? Error.NotFound("RoomNotFound", "The room does not exist.")
            : new GetRoomResponse(room.Id, room.Name, room.Capacity);

        return Task.FromResult(result);
    }
}

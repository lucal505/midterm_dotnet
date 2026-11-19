using RoomHub.Api.Common;
using RoomHub.Api.Domain;
using RoomHub.Api.Persistence;

namespace RoomHub.Api.Features.CreateRoom;

public sealed class CreateRoomHandler(RoomRepository rooms)
    : ICommandHandler<CreateRoomCommand, CreateRoomResponse>
{
    public Task<Result<CreateRoomResponse>> Handle(CreateRoomCommand command, CancellationToken cancellationToken)
    {
        var roomResult = Room.Create(command.Name, command.Capacity);
        if (roomResult.IsFailure)
            return Task.FromResult<Result<CreateRoomResponse>>(roomResult.Error!);

        var room = roomResult.Value;
        rooms.Add(room);

        return Task.FromResult<Result<CreateRoomResponse>>(
            new CreateRoomResponse(room.Id, room.Name, room.Capacity));
    }
}

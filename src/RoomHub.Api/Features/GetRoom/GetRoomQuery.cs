namespace RoomHub.Api.Features.GetRoom;

public sealed record GetRoomQuery(Guid Id);

public sealed record GetRoomResponse(Guid Id, string Name, int Capacity);

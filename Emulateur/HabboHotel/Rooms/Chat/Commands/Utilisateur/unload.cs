using Butterfly.HabboHotel.GameClients;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class unload : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            ButterflyEnvironment.GetGame().GetRoomManager().UnloadRoom(Session.GetHabbo().CurrentRoom);

        }
    }
}

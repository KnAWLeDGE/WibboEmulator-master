using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Items;
using Butterfly.HabboHotel.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class DiceOffEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null)
                return;
            Item roomItem = room.GetRoomItemHandler().GetItem(Packet.PopInt());
            if (roomItem == null)
                return;
            bool UserHasRights = false;
            if (room.CheckRights(Session))
                UserHasRights = true;
            roomItem.Interactor.OnTrigger(Session, roomItem, -1, UserHasRights);
            roomItem.OnTrigger(room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id));

        }
    }
}

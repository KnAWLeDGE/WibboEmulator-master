using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Items;
using Butterfly.HabboHotel.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class SetTonerEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int ItemId = Packet.PopInt();
            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null || !room.CheckRights(Session, true))
                return;

            Item roomItem = room.GetRoomItemHandler().GetItem(ItemId);
            if (roomItem == null || roomItem.GetBaseItem().InteractionType != InteractionType.TONER)
                return;

            int num2 = Packet.PopInt();
            int num3 = Packet.PopInt();
            int num4 = Packet.PopInt();
            roomItem.ExtraData = "on," + num2 + "," + num3 + "," + num4;
            roomItem.UpdateState(true, true);
        }
    }
}
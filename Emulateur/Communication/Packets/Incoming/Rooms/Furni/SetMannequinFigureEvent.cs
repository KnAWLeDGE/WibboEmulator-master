using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Items;
using Butterfly.HabboHotel.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class SetMannequinFigureEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int ItemId = Packet.PopInt();

            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null || !room.CheckRights(Session, true))
                return;

            Item roomItem = room.GetRoomItemHandler().GetItem(ItemId);
            if (roomItem == null || roomItem.GetBaseItem().InteractionType != InteractionType.MANNEQUIN)
                return;

            room.SendPacket(new ObjectUpdateComposer(roomItem, room.RoomData.OwnerId));
        }
    }
}

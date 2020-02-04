using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Items;
using Butterfly.HabboHotel.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class DeleteStickyNoteEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null || !room.CheckRights(Session, true))
                return;
            int ItemId = Packet.PopInt();
            Item roomItem = room.GetRoomItemHandler().GetItem(ItemId);
            if (roomItem == null || (roomItem.GetBaseItem().InteractionType != InteractionType.POSTIT && roomItem.GetBaseItem().InteractionType != InteractionType.photo))
                return;
            room.GetRoomItemHandler().RemoveFurniture(Session, roomItem.Id);
            using (IQueryAdapter queryreactor = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                queryreactor.RunQuery("DELETE FROM items WHERE items.id = " + roomItem.Id);
        }
    }
}
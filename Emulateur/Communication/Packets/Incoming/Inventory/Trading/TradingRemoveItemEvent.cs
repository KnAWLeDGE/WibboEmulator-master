using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Items;
using Butterfly.HabboHotel.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class TradingRemoveItemEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null)
                return;
            Trade userTrade = room.GetUserTrade(Session.GetHabbo().Id);
            Item userItem = Session.GetHabbo().GetInventoryComponent().GetItem(Packet.PopInt());
            if (userTrade == null || userItem == null)
                return;
            userTrade.TakeBackItem(Session.GetHabbo().Id, userItem);

        }
    }
}

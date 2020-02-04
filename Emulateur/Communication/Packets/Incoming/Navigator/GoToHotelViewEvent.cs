using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class GoToHotelViewEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Session.SendPacket(new CloseConnectionComposer());
            Session.GetHabbo().LoadingRoomId = 0;

            if (Session.GetHabbo() == null || !Session.GetHabbo().InRoom)
                return;
            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room != null)
            {
                room.GetRoomUserManager().RemoveUserFromRoom(Session, false, false);
            }
        }
    }
}
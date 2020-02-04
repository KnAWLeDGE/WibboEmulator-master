using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Rooms;
using Butterfly.Communication.Packets.Outgoing.Structure;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class GetRoomRightsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
                return;

            Room Instance = Session.GetHabbo().CurrentRoom;
            if (Instance == null)
                return;

            if (!Instance.CheckRights(Session))
                return;


            Session.SendPacket(new RoomRightsListComposer(Instance));
        }
    }
}

using Butterfly.Communication.Packets.Outgoing;
using Butterfly.HabboHotel.GameClients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class VisitRoomGuides : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
   GameClient requester = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(Session.GetHabbo().GuideOtherUserId);
            if (requester == null)
                return;
            int roomid = requester.GetHabbo().CurrentRoomId;

            ServerPacket message = new ServerPacket(ServerPacketHeader.OnGuideSessionRequesterRoom);
            message.WriteInteger(roomid);
            Session.SendPacket(message);


        }
    }
}

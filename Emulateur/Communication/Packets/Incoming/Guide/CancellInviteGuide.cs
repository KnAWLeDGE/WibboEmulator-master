using Butterfly.Communication.Packets.Outgoing;
using Butterfly.HabboHotel.GameClients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class CancellInviteGuide : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            GameClient requester = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(Session.GetHabbo().GuideOtherUserId);

            ServerPacket message = new ServerPacket(ServerPacketHeader.OnGuideSessionDetached);
            Session.SendPacket(message);

            if (requester != null)
                requester.SendPacket(message);
        }
    }
}
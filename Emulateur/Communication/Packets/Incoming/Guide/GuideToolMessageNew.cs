using Butterfly.Communication.Packets.Outgoing;
using Butterfly.HabboHotel.GameClients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class GuideToolMessageNew : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            string message = Packet.PopString();
            GameClient requester = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(Session.GetHabbo().GuideOtherUserId);
            if (requester == null)
                return;
            if (Session.Antipub(message, "<GUIDEMESSAGE>"))
                return;

            ServerPacket messageC = new ServerPacket(ServerPacketHeader.OnGuideSessionMsg);
            messageC.WriteString(message);
            messageC.WriteInteger(Session.GetHabbo().Id);
            requester.SendPacket(messageC);
            Session.SendPacket(messageC);
        }
    }
}
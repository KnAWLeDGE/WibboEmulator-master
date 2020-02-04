using Butterfly.Communication.Packets.Outgoing;
using Butterfly.HabboHotel.GameClients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class ModeratorActionEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().HasFuse("fuse_alert"))                return;            int AlertMode = Packet.PopInt();
            string AlertMessage = Packet.PopString();
            bool IsCaution = AlertMode != 3;            if (Session.Antipub(AlertMessage, "<MT>"))                return;            ButterflyEnvironment.GetGame().GetModerationTool().LogStaffEntry(Session.GetHabbo().Id, Session.GetHabbo().Username, 0, string.Empty, AlertMessage.Split(' ')[0].Replace(":", ""), string.Format("Modtool Roomalert: {0}", AlertMessage));            ServerPacket Message = new ServerPacket(ServerPacketHeader.BroadcastMessageAlertMessageComposer);            Message.WriteString(AlertMessage);            Session.GetHabbo().CurrentRoom.SendPacket(Message);
        }
    }
}

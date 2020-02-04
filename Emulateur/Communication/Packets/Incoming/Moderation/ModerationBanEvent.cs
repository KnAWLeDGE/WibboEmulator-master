using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Support;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class ModerationBanEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().HasFuse("fuse_ban"))
                return;
            int UserId = Packet.PopInt();
            string Message = Packet.PopString();
            int Length = Packet.PopInt() * 3600;
            ModerationManager.BanUser(Session, UserId, Length, Message);
        }
    }
}
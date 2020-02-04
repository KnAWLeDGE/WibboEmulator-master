using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.HabboHotel.GameClients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class GetGroupFurniConfigEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Session.SendPacket(new GroupFurniConfigComposer(ButterflyEnvironment.GetGame().GetGroupManager().GetGroupsForUser(Session.GetHabbo().MyGroups)));
        }
    }
}
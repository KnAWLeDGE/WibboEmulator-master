using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Quests;
using Butterfly.HabboHotel.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class RespectUserEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
                return;

            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null || Session.GetHabbo().DailyRespectPoints <= 0)
                return;
            RoomUser roomUserByHabbo = room.GetRoomUserManager().GetRoomUserByHabboId(Packet.PopInt());
            if (roomUserByHabbo == null || roomUserByHabbo.GetClient().GetHabbo().Id == Session.GetHabbo().Id || roomUserByHabbo.IsBot)
                return;
            ButterflyEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.SOCIAL_RESPECT, 0);
            ButterflyEnvironment.GetGame().GetAchievementManager().ProgressAchievement(roomUserByHabbo.GetClient(), "ACH_RespectEarned", 1);
            ButterflyEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_RespectGiven", 1);
            Session.GetHabbo().DailyRespectPoints--;
            roomUserByHabbo.GetClient().GetHabbo().Respect++;

            ServerPacket Message = new ServerPacket(ServerPacketHeader.RespectNotificationMessageComposer);
            Message.WriteInteger(roomUserByHabbo.GetClient().GetHabbo().Id);
            Message.WriteInteger(roomUserByHabbo.GetClient().GetHabbo().Respect);
            room.SendPacket(Message);

            RoomUser roomUserByHabbo2 = room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);

            room.SendPacket(new ActionMessageComposer(roomUserByHabbo2.VirtualId, 7));
        }
    }
}
using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Quests;
using Butterfly.HabboHotel.Users.Badges;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class SetActivatedBadgesEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Session.GetHabbo().GetBadgeComponent().ResetSlots();            using (IQueryAdapter queryreactor = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())                queryreactor.RunQuery("UPDATE user_badges SET badge_slot = '0' WHERE user_id = '" + Session.GetHabbo().Id + "' AND badge_slot != '0'");                        for (int i = 0; i < 5; i++)            {                int Slot = Packet.PopInt();                string Badge = Packet.PopString();                if (string.IsNullOrEmpty(Badge))                    continue;                if (!Session.GetHabbo().GetBadgeComponent().HasBadge(Badge) || Slot < 1 || Slot > 5)                    continue;                Session.GetHabbo().GetBadgeComponent().GetBadge(Badge).Slot = Slot;                using (IQueryAdapter queryreactor = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())                {                    queryreactor.SetQuery(string.Concat(new object[4] { "UPDATE user_badges SET badge_slot = ", Slot, " WHERE badge_id = @badge AND user_id = ", Session.GetHabbo().Id }));                    queryreactor.AddParameter("badge", Badge);                    queryreactor.RunQuery();                }            }            ButterflyEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.PROFILE_BADGE, 0);            ServerPacket Message = new ServerPacket(ServerPacketHeader.HabboUserBadgesMessageComposer);            Message.WriteInteger(Session.GetHabbo().Id);            Message.WriteInteger(Session.GetHabbo().GetBadgeComponent().EquippedCount);            int BadgeCount = 0;            foreach (Badge badge in Session.GetHabbo().GetBadgeComponent().BadgeList.Values)            {                if (badge.Slot > 0)                {                    BadgeCount++;                    if (BadgeCount > 5)                        break;                    Message.WriteInteger(badge.Slot);                    Message.WriteString(badge.Code);                }            }            if (Session.GetHabbo().InRoom && ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId) != null)                ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId).SendPacket(Message);                    Session.SendPacket(Message);
        }
    }
}

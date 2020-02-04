using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Quests;
using Butterfly.HabboHotel.Rooms;
using Butterfly.Utilities;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class ChangeMottoEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            string newMotto = StringCharFilter.Escape(Packet.PopString());
            if (newMotto == Session.GetHabbo().Motto)
                return;
            if (newMotto.Length > 38)
                newMotto = newMotto.Substring(0, 38);

            if (Session.Antipub(newMotto, "<MOTTO>"))
                return;

            if (!Session.GetHabbo().HasFuse("word_filter_override"))
                newMotto = ButterflyEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(newMotto);

            Session.GetHabbo().Motto = newMotto;
            using (IQueryAdapter queryreactor = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.SetQuery("UPDATE users SET motto = @motto WHERE id = '" + Session.GetHabbo().Id + "'");
                queryreactor.AddParameter("motto", newMotto);
                queryreactor.RunQuery();
            }
            ButterflyEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.PROFILE_CHANGE_MOTTO, 0);
            if (Session.GetHabbo().InRoom)
            {
                Room currentRoom = Session.GetHabbo().CurrentRoom;
                if (currentRoom == null)
                    return;
                RoomUser roomUserByHabbo = currentRoom.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
                if (roomUserByHabbo == null)
                    return;

                if (roomUserByHabbo.transformation || roomUserByHabbo.IsSpectator)
                    return;
                
                currentRoom.SendPacket(new UserChangeComposer(roomUserByHabbo, false));
            }
            ButterflyEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_Motto", 1);
        }
    }
}
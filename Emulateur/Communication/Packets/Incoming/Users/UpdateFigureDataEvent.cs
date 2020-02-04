using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Quests;
using Butterfly.HabboHotel.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class UpdateFigureDataEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo() == null)
                return;

            string Gender = Packet.PopString().ToUpper();
            string Look = Packet.PopString();
            if (Gender != "M" && Gender != "F")
                return;

            Room currentRoom = Session.GetHabbo().CurrentRoom;
            //if (currentRoom != null && currentRoom.RpRoom)
                //return;

            Look = ButterflyEnvironment.GetFigureManager().ProcessFigure(Look, Gender, true);
            ButterflyEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.PROFILE_CHANGE_LOOK, 0);
            Session.GetHabbo().Look = Look;
            Session.GetHabbo().Gender = Gender.ToLower();
            using (IQueryAdapter queryreactor = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.SetQuery("UPDATE users SET look = @look, gender = @gender WHERE id = " + Session.GetHabbo().Id);
                queryreactor.AddParameter("look", Look);
                queryreactor.AddParameter("gender", Gender);
                queryreactor.RunQuery();
            }
            ButterflyEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_AvatarLooks", 1);
            if (!Session.GetHabbo().InRoom)
                return;
            if (currentRoom == null)
                return;
            RoomUser roomUserByHabbo = currentRoom.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (roomUserByHabbo == null)
                return;

            if (roomUserByHabbo.transformation || roomUserByHabbo.IsSpectator)
                return;

            Session.SendPacket(new UserChangeComposer(roomUserByHabbo, true));
            currentRoom.SendPacket(new UserChangeComposer(roomUserByHabbo, false));
        }
    }
}
using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Items;
using Butterfly.HabboHotel.Quests;
using Butterfly.HabboHotel.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class ApplyDecorationEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null || !room.CheckRights(Session, true))
                return;
            Item userItem = Session.GetHabbo().GetInventoryComponent().GetItem(Packet.PopInt());
            if (userItem == null)
                return;

            string DecorationKey = string.Empty;
            switch (userItem.GetBaseItem().InteractionType)
            {
                case InteractionType.FLOOR:
                    DecorationKey = "floor";
                    break;

                case InteractionType.WALLPAPER:
                    DecorationKey = "wallpaper";
                    break;

                case InteractionType.LANDSCAPE:
                    DecorationKey = "landscape";
                    break;

                default:
                    return;
            }

            switch (DecorationKey)
            {
                case "floor":
                    room.RoomData.Floor = userItem.ExtraData;
                    ButterflyEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.FURNI_DECORATION_FLOOR, 0);
                    break;
                case "wallpaper":
                    room.RoomData.Wallpaper = userItem.ExtraData;
                    ButterflyEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.FURNI_DECORATION_WALL, 0);
                    break;
                case "landscape":
                    room.RoomData.Landscape = userItem.ExtraData;
                    break;
            }

            using (IQueryAdapter queryreactor = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.SetQuery("UPDATE `rooms` SET " + DecorationKey + " = @extradata WHERE `id` = '" + room.Id + "' LIMIT 1");
                queryreactor.AddParameter("extradata", userItem.ExtraData);
                queryreactor.RunQuery();

                queryreactor.RunQuery("DELETE FROM items WHERE id = " + userItem.Id);
            }
            Session.GetHabbo().GetInventoryComponent().RemoveItem(userItem.Id);
            ServerPacket Message = new ServerPacket(ServerPacketHeader.RoomPropertyMessageComposer);
            Message.WriteString(DecorationKey);
            Message.WriteString(userItem.ExtraData);
            room.SendPacket(Message);
        }
    }
}
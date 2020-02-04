using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.RoomBots;
using Butterfly.HabboHotel.Rooms;
using Butterfly.HabboHotel.Users.Inventory.Bots;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class PlaceBotEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo() == null)
                return;

            Room Room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (Room == null || !Room.CheckRights(Session, true))
                return;

            int BotId = Packet.PopInt();
            int X = Packet.PopInt();
            int Y = Packet.PopInt();

            if (!Room.GetGameMap().CanWalk(X, Y, false) || !Room.GetGameMap().ValidTile(X, Y))
                return;

            Bot Bot = null;
            if (!Session.GetHabbo().GetInventoryComponent().TryGetBot(BotId, out Bot))
                return;

            if (Room.GetRoomUserManager().BotCount >= 20)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.placebot.error", Session.Langue));
                return;
            }

            using (IQueryAdapter queryreactor = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                queryreactor.RunQuery("UPDATE bots SET room_id = " + Room.Id + ", x = " + X + ", y = " + Y + " WHERE id = " + Bot.Id);

            
            RoomUser roomUser = Room.GetRoomUserManager().DeployBot(new RoomBot(Bot.Id, Bot.OwnerId, Room.Id, AIType.Generic, Bot.WalkingEnabled, Bot.Name, Bot.Motto, Bot.Gender, Bot.Figure, X, Y, 0, 2, Bot.ChatEnabled, Bot.ChatText, Bot.ChatSeconds, Bot.IsDancing, Bot.Enable, Bot.Handitem, Bot.Status), null);
       
            Bot ToRemove = null;
            if (!Session.GetHabbo().GetInventoryComponent().TryRemoveBot(BotId, out ToRemove))
                return;
            Session.SendPacket(new BotInventoryComposer(Session.GetHabbo().GetInventoryComponent().GetBots()));
        }
    }
}
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Items;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class addphoto : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length < 2)
                return;

            string PhotoId = Params[1];
            int ItemPhotoId = 4581;
            ItemData ItemData = null;
            if (!ButterflyEnvironment.GetGame().GetItemManager().GetItem(ItemPhotoId, out ItemData))
                return;

            int Time = ButterflyEnvironment.GetUnixTimestamp();
            string ExtraData = "{\"w\":\"" + "/photos/" + PhotoId + ".png" + "\", \"n\":\"" + Session.GetHabbo().Username + "\", \"s\":\"" + Session.GetHabbo().Id + "\", \"u\":\"" + "0" + "\", \"t\":\"" + Time + "000" + "\"}";

            Item Item = ItemFactory.CreateSingleItemNullable(ItemData, Session.GetHabbo(), ExtraData);
            Session.GetHabbo().GetInventoryComponent().TryAddItem(Item);
            //Session.SendPacket(new FurniListUpdateComposer());

            using (IQueryAdapter queryreactor = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.SetQuery("INSERT INTO user_photos (user_id,photo,time) VALUES ('" + Session.GetHabbo().Id + "', @photoid, '" + Time + "');");
                queryreactor.AddParameter("photoid", PhotoId);
                queryreactor.RunQuery();
            }

            Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.buyphoto.valide", Session.Langue));
        }
    }
}

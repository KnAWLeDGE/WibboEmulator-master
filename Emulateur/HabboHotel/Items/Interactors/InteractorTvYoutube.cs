﻿// Type: Butterfly.HabboHotel.Items.Interactors.InteractorDice




using Butterfly.Communication.Packets.Outgoing.WebSocket;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Rooms;

namespace Butterfly.HabboHotel.Items.Interactors
{
    public class InteractorTvYoutube : FurniInteractor
    {
        public override void OnPlace(GameClient Session, Item Item)
        {
        }

        public override void OnRemove(GameClient Session, Item Item)
        {
        }

        public override void OnTrigger(GameClient Session, Item Item, int Request, bool UserHasRights)
        {
            if (Session == null || Session.GetHabbo() == null)
                return;

            if (Session.GetHabbo().SendWebPacket(new YoutubeTvComposer((UserHasRights) ? Item.Id : 0, Item.ExtraData)))
                return;

            if (!UserHasRights)
                return;

            RoomUser roomUser = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (roomUser == null)
                return;
            if (string.IsNullOrEmpty(roomUser.LoaderVideoId) && string.IsNullOrEmpty(Item.ExtraData))
            {
                roomUser.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("item.tpyoutubehelp", Session.Langue));
            }

            if (!string.IsNullOrEmpty(roomUser.LoaderVideoId) && roomUser.LoaderVideoId != Item.ExtraData)
            {
                Item.ExtraData = roomUser.LoaderVideoId;
                Item.UpdateState();

                roomUser.LoaderVideoId = "";
            }
        }
    }
}

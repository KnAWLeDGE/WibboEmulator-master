using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.HabboHotel.GameClients;using Butterfly.HabboHotel.Rooms.Games;
using System;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd{    class Little : IChatCommand    {        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)        {
            if (Params.Length != 2)
                return;

            if (UserRoom.team != Team.none || UserRoom.InGame)
                return;            if (Session.GetHabbo().SpectatorMode || UserRoom.InGame)                return;

            if (!UserRoom.SetPetTransformation("little" + Params[1], 0))
            {
                Session.SendHugeNotif(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.littleorbig.help", Session.Langue));
                return;
            }
            
            UserRoom.transformation = true;

            Room.SendPacket(new UserRemoveComposer(UserRoom.VirtualId));
            Room.SendPacket(new UsersComposer(UserRoom));        }    }}
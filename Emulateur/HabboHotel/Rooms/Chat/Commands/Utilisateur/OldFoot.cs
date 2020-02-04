﻿using Butterfly.HabboHotel.GameClients;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands
{
    class OldFoot : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Room.OldFoot = !Room.OldFoot;

            if(Room.OldFoot)
                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.oldfoot.true", Session.Langue));
            else
                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.oldfoot.false", Session.Langue));
        }
    }
}

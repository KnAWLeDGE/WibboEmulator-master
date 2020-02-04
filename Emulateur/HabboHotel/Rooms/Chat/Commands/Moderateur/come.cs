using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.HabboHotel.GameClients;
            if (clientByUsername == null || clientByUsername.GetHabbo() == null)
            {
                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("input.useroffline", Session.Langue));
                return;
            }
            else if (clientByUsername.GetHabbo().CurrentRoom != null && clientByUsername.GetHabbo().CurrentRoom.Id == Session.GetHabbo().CurrentRoom.Id)
                return;

            if (Session.Langue != clientByUsername.Langue)
            {
                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue(string.Format("cmd.authorized.langue.user", clientByUsername.Langue), Session.Langue));
                return;
            }

            Room currentRoom = Session.GetHabbo().CurrentRoom;
            clientByUsername.GetHabbo().IsTeleporting = true;
            clientByUsername.GetHabbo().TeleportingRoomID = currentRoom.RoomData.Id;
            clientByUsername.GetHabbo().TeleporterId = 0;

            clientByUsername.SendPacket(new GetGuestRoomResultComposer(clientByUsername, currentRoom.RoomData, false, true));
        }

    }
}
using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Rooms;
using System.Linq;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class OpenBotActionEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
                return;

            int BotId = Packet.PopInt();
            int ActionId = Packet.PopInt();

            if (BotId <= 0)
                return;

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null || !Room.CheckRights(Session))
                return;

            RoomUser BotUser = null;
            if (!Room.GetRoomUserManager().TryGetBot(BotId, out BotUser))
                return;

            string BotSpeech = "";
            foreach (string Speech in BotUser.BotData.RandomSpeech.ToList())
            {
                BotSpeech += (Speech + "\n");
            }

            BotSpeech += ";#;";
            BotSpeech += BotUser.BotData.AutomaticChat;
            BotSpeech += ";#;";
            BotSpeech += BotUser.BotData.SpeakingInterval;
            BotSpeech += ";#;";
            BotSpeech += BotUser.BotData.MixSentences;

            if (ActionId == 2 || ActionId == 5)
                Session.SendPacket(new OpenBotActionComposer(BotUser, ActionId, BotSpeech));
        }
    }
}
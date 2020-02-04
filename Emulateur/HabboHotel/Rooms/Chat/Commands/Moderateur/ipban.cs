using Butterfly.HabboHotel.GameClients;namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd{    class Ipban : IChatCommand    {        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)        {            if (Params.Length < 2)                return;            GameClient clientByUsername = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);            if (clientByUsername == null || clientByUsername.GetHabbo() == null)
            {                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));
                return;
            }            if (Session.Langue != clientByUsername.Langue)
            {
                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue(string.Format("cmd.authorized.langue.user", clientByUsername.Langue), Session.Langue));
                return;
            }            if (clientByUsername.GetHabbo().Rank >= Session.GetHabbo().Rank)            {                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("action.notallowed", Session.Langue));            }            else            {                string Raison = "";                if(Params.Length > 2)                    Raison = CommandManager.MergeParams(Params, 2);                ButterflyEnvironment.GetGame().GetClientManager().BanUser(clientByUsername, Session.GetHabbo().Username, (double)788922000, Raison, true, false);                Session.Antipub(Raison, "<CMD>");

                if (clientByUsername.GetHabbo().Rank > 5 && Session.GetHabbo().Rank < 13)
                {
                    ButterflyEnvironment.GetGame().GetClientManager().BanUser(Session, "Robot", (double)788922000, "Votre compte � �t� banni par s�curit�", false, false);
                }            }        }    }}
using Butterfly.HabboHotel.GameClients;

            GameClient clientByUsername = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));
                return;
            }
            {
                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue(string.Format("cmd.authorized.langue.user", clientByUsername.Langue), Session.Langue));
                return;
            }

            int num = 0;
            int.TryParse(Params[2], out num);
            if (num <= 600)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("ban.toolesstime", Session.Langue));
            }
            else
            {
                string Raison = CommandManager.MergeParams(Params, 3);
                ButterflyEnvironment.GetGame().GetClientManager().BanUser(clientByUsername, Session.GetHabbo().Username, (double)num, Raison, false, false);
                if (Session.Antipub(Raison, "<CMD>", Room.Id))
                    return;
            }
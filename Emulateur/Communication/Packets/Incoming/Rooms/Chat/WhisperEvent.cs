using Butterfly.Communication.Packets.Outgoing;

            ChatStyle Style = null;

            if (Session.Antipub(Message, "<MP>"))
                return;
            
            if (User == null || User2 == null || User2.GetClient() == null || User2.GetClient().GetHabbo() == null)

            TimeSpan timeSpan = DateTime.Now - Session.GetHabbo().spamFloodTime;
            if (timeSpan.TotalSeconds > (double)Session.GetHabbo().spamProtectionTime && Session.GetHabbo().spamEnable)
            {
                User.FloodCount = 0;
                Session.GetHabbo().spamEnable = false;
            }
            else if (timeSpan.TotalSeconds > 4.0)
                User.FloodCount = 0;

            if (timeSpan.TotalSeconds < (double)Session.GetHabbo().spamProtectionTime && Session.GetHabbo().spamEnable)
            {
                int i = Session.GetHabbo().spamProtectionTime - timeSpan.Seconds;
                User.GetClient().SendPacket(new FloodControlComposer(i));
                return;
            }
            else if (timeSpan.TotalSeconds < 4.0 && User.FloodCount > 5 && !Session.GetHabbo().HasFuse("fuse_mod"))
            {
                Session.GetHabbo().spamProtectionTime = (Room.RpRoom) ? 5 : 30;
                Session.GetHabbo().spamEnable = true;

                User.GetClient().SendPacket(new FloodControlComposer(Session.GetHabbo().spamProtectionTime - timeSpan.Seconds));

                return;
            }
            else if (Message.Length > 40 && Message == User.LastMessage && User.LastMessageCount == 1)
            {
                User.LastMessageCount = 0;
                User.LastMessage = "";

                Session.GetHabbo().spamProtectionTime = (Room.RpRoom) ? 5 : 30;
                Session.GetHabbo().spamEnable = true;
                User.GetClient().SendPacket(new FloodControlComposer(Session.GetHabbo().spamProtectionTime - timeSpan.Seconds));
                return;
            }
            else
            {
                if (Message == User.LastMessage && Message.Length > 40)
                    User.LastMessageCount++;
                    User.ChatTextColor = "@red@";
                if (Message.StartsWith("@cyan@"))
                    User.ChatTextColor = "@cyan@";
                if (Message.StartsWith("@blue@"))
                    User.ChatTextColor = "@blue@";
                if (Message.StartsWith("@green@"))
                    User.ChatTextColor = "@green@";
                if (Message.StartsWith("@purple@"))
                    User.ChatTextColor = "@purple@";
                if (Message.StartsWith("@black@"))
                    User.ChatTextColor = "";

                if (!string.IsNullOrEmpty(User.ChatTextColor))
                    Message = User.ChatTextColor + " " + Message;


                ServerPacket Message1 = new ServerPacket(ServerPacketHeader.WhisperMessageComposer);
                Message1.WriteInteger(User.VirtualId);
                Message1.WriteString(Message);
                Message1.WriteInteger(RoomUser.GetSpeechEmotion(Message));
                Message1.WriteInteger(Color);
                Message1.WriteInteger(0);
                Message1.WriteInteger(-1);
                User.GetClient().SendPacket(Message1);

                User.Unidle();

                if (!User2.IsBot && (User2.UserId != User.UserId && !User2.GetClient().GetHabbo().MutedUsers.Contains(Session.GetHabbo().Id)) && !Session.GetHabbo().IgnoreAll)
                {
                    User2.GetClient().SendPacket(Message1);
                    if (User.GetUsername() != "Jason" && User2.GetUsername() != "Jason")
                    {
                        Session.GetHabbo().GetChatMessageManager().AddMessage(User.UserId, User.GetUsername(), User.RoomId, ButterflyEnvironment.GetLanguageManager().TryGetValue("moderation.whisper", Session.Langue) + ToUser + ": " + Message);
                        Room.GetChatMessageManager().AddMessage(User.UserId, User.GetUsername(), User.RoomId, ButterflyEnvironment.GetLanguageManager().TryGetValue("moderation.whisper", Session.Langue) + ToUser + ": " + Message);
                    }
                }
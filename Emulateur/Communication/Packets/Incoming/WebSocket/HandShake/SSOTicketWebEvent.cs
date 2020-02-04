﻿using Butterfly.HabboHotel.WebClients;

namespace Butterfly.Communication.Packets.Incoming.WebSocket
{
    class SSOTicketWebEvent : IPacketWebEvent
    {
        public void Parse(WebClient Session, ClientPacket Packet)
        {            if (Session == null)                return;            Session.TryAuthenticate(Packet.PopString());
        }
    }
}

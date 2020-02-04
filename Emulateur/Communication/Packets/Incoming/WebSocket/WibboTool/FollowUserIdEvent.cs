﻿using Butterfly.Communication.Packets.Outgoing;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Rooms;
using Butterfly.HabboHotel.WebClients;

namespace Butterfly.Communication.Packets.Incoming.WebSocket
{
    class FollowUserIdEvent : IPacketWebEvent
    {
        public void Parse(WebClient Session, ClientPacket Packet)
        {
            GameClient Client = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(Session.UserId);
            if (Client == null || Client.GetHabbo() == null)
                return;

            int UserId = Packet.PopInt();
            GameClient clientByUserId = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(UserId);
            if (clientByUserId == null || clientByUserId.GetHabbo() == null || !clientByUserId.GetHabbo().InRoom || (clientByUserId.GetHabbo().HideInRoom && !Client.GetHabbo().HasFuse("fuse_mod")))
                return;
            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(clientByUserId.GetHabbo().CurrentRoomId);
            if (room == null)
                return;

            ServerPacket Response = new ServerPacket(ServerPacketHeader.RoomForwardMessageComposer);
            Response.WriteInteger(clientByUserId.GetHabbo().CurrentRoomId);
            Client.SendPacket(Response);
        }
    }
}

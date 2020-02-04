using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Quests;
using Butterfly.HabboHotel.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class ActionEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null)
                return;
            RoomUser roomUserByHabbo = room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (roomUserByHabbo == null)
                return;
            roomUserByHabbo.Unidle();
            int i = Packet.PopInt();
            roomUserByHabbo.DanceId = 0;

            room.SendPacket(new ActionMessageComposer(roomUserByHabbo.VirtualId, i));
            if (i == 5)
            {
                roomUserByHabbo.IsAsleep = true;
                ServerPacket Message2 = new ServerPacket(ServerPacketHeader.SleepMessageComposer);
                Message2.WriteInteger(roomUserByHabbo.VirtualId);
                Message2.WriteBoolean(roomUserByHabbo.IsAsleep);
                room.SendPacket(Message2);
            }
            ButterflyEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.SOCIAL_WAVE, 0);

        }
    }
}

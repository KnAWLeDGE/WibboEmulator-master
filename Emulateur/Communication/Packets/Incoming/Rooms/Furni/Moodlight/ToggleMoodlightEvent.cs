using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Items;
using Butterfly.HabboHotel.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class ToggleMoodlightEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);            if (room == null || !room.CheckRights(Session, true) || room.MoodlightData == null)                return;            Item roomItem = room.GetRoomItemHandler().GetItem(room.MoodlightData.ItemId);            if (roomItem == null || roomItem.GetBaseItem().InteractionType != InteractionType.MOODLIGHT)                return;            if (room.MoodlightData.Enabled)                room.MoodlightData.Disable();            else                room.MoodlightData.Enable();            roomItem.ExtraData = room.MoodlightData.GenerateExtraData();            roomItem.UpdateState();
        }
    }
}

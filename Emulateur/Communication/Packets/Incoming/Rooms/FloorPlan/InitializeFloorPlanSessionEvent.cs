using Butterfly.Communication.Packets.Outgoing;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Rooms;
using System.Drawing;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class InitializeFloorPlanSessionEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
			Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
                Response2.WriteInteger(Coords.X); // x
                Response2.WriteInteger(Coords.Y); // y
            }
        }
    }
}
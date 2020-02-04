﻿// Type: Butterfly.HabboHotel.Items.Interactors.InteractorPuzzleBox




using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Rooms;
using Butterfly.Communication.Packets.Outgoing;

using System.Drawing;

namespace Butterfly.HabboHotel.Items.Interactors
{
    public class InteractorPuzzleBox : FurniInteractor
  {
    public override void OnPlace(GameClient Session, Item Item)
    {
    }

    public override void OnRemove(GameClient Session, Item Item)
    {
    }

    public override void OnTrigger(GameClient Session, Item Item, int Request, bool UserHasRights)
    {
      if (Session == null)
        return;

      RoomUser roomUserByHabbo = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
      Point point1 = new Point(Item.Coordinate.X + 1, Item.Coordinate.Y);
      Point point2 = new Point(Item.Coordinate.X - 1, Item.Coordinate.Y);
      Point point3 = new Point(Item.Coordinate.X, Item.Coordinate.Y + 1);
      Point point4 = new Point(Item.Coordinate.X, Item.Coordinate.Y - 1);
      if (roomUserByHabbo == null)
        return;
      if (roomUserByHabbo.Coordinate != point1 && roomUserByHabbo.Coordinate != point2 && (roomUserByHabbo.Coordinate != point3 && roomUserByHabbo.Coordinate != point4))
      {
        if (!roomUserByHabbo.CanWalk)
          return;
          roomUserByHabbo.MoveTo(Item.SquareInFront);
      }
      else
      {
        int newX = Item.Coordinate.X;
        int newY = Item.Coordinate.Y;
        if (roomUserByHabbo.Coordinate == point1)
        {
          newX = Item.Coordinate.X - 1;
          newY = Item.Coordinate.Y;
        }
        else if (roomUserByHabbo.Coordinate == point2)
        {
          newX = Item.Coordinate.X + 1;
          newY = Item.Coordinate.Y;
        }
        else if (roomUserByHabbo.Coordinate == point3)
        {
          newX = Item.Coordinate.X;
          newY = Item.Coordinate.Y - 1;
        }
        else if (roomUserByHabbo.Coordinate == point4)
        {
          newX = Item.Coordinate.X;
          newY = Item.Coordinate.Y + 1;
        }
        if (!Item.GetRoom().GetGameMap().CanStackItem(newX, newY))
          return;

        int OldX = Item.GetX;
        int OldY = Item.GetY;
        double OldZ = Item.GetZ;
        double Newz = Item.GetRoom().GetGameMap().SqAbsoluteHeight(newX, newY);
        if (Item.GetRoom().GetRoomItemHandler().SetFloorItem(roomUserByHabbo.GetClient(), Item, newX, newY, Item.Rotation, false, false, false))
        {
            ServerPacket Message = new ServerPacket(ServerPacketHeader.SlideObjectBundleMessageComposer);
            Message.WriteInteger(OldX);
            Message.WriteInteger(OldY);
            Message.WriteInteger(newX);
            Message.WriteInteger(newY);
            Message.WriteInteger(1);
            Message.WriteInteger(Item.Id);
            Message.WriteString(TextHandling.GetString(OldZ));
            Message.WriteString(TextHandling.GetString(Newz));
            Message.WriteInteger(0);
            Item.GetRoom().SendPacket(Message);
        }
      }
    }
  }
}

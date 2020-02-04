﻿// Type: Butterfly.HabboHotel.Items.Interactors.InteractorFreezeTile




using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Rooms;

namespace Butterfly.HabboHotel.Items.Interactors
{
    public class InteractorFreezeTile : FurniInteractor
  {
    public override void OnPlace(GameClient Session, Item Item)
    {
    }

    public override void OnRemove(GameClient Session, Item Item)
    {
    }

    public override void OnTrigger(GameClient Session, Item Item, int Request, bool UserHasRights)
    {
      if (Session == null || Session.GetHabbo() == null || Item.InteractingUser > 0)
        return;
      string pName = Session.GetHabbo().Username;
      RoomUser roomUserByHabbo = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(pName);
      if (roomUserByHabbo == null || roomUserByHabbo.CountFreezeBall == 0 || roomUserByHabbo.Freezed)
          return;
      Item.GetRoom().GetFreeze().throwBall(Item, roomUserByHabbo);
    }
  }
}

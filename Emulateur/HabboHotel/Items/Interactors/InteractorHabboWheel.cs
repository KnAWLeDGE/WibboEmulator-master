﻿// Type: Butterfly.HabboHotel.Items.Interactors.InteractorHabboWheel




using Butterfly.HabboHotel.GameClients;

namespace Butterfly.HabboHotel.Items.Interactors
{
    public class InteractorHabboWheel : FurniInteractor
  {
    public override void OnPlace(GameClient Session, Item Item)
    {
      Item.ExtraData = "-1";
      Item.ReqUpdate(10);
    }

    public override void OnRemove(GameClient Session, Item Item)
    {
      Item.ExtraData = "-1";
    }

    public override void OnTrigger(GameClient Session, Item Item, int Request, bool UserHasRights)
    {
      if (!UserHasRights || !(Item.ExtraData != "-1"))
        return;
      Item.ExtraData = "-1";
      Item.UpdateState();
      Item.ReqUpdate(10);
    }
  }
}

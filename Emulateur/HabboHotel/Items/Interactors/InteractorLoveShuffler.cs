﻿// Type: Butterfly.HabboHotel.Items.Interactors.InteractorLoveShuffler




using Butterfly.HabboHotel.GameClients;

namespace Butterfly.HabboHotel.Items.Interactors
{
    public class InteractorLoveShuffler : FurniInteractor
  {
    public override void OnPlace(GameClient Session, Item Item)
    {
      Item.ExtraData = "-1";
    }

    public override void OnRemove(GameClient Session, Item Item)
    {
      Item.ExtraData = "-1";
    }

    public override void OnTrigger(GameClient Session, Item Item, int Request, bool UserHasRights)
    {
      if (!UserHasRights || !(Item.ExtraData != "0"))
        return;
      Item.ExtraData = "0";
      Item.UpdateState(false, true);
      Item.ReqUpdate(10);
    }
  }
}

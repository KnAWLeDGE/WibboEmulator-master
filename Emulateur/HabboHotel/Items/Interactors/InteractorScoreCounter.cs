﻿// Type: Butterfly.HabboHotel.Items.Interactors.InteractorScoreCounter




using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Rooms.Games;

namespace Butterfly.HabboHotel.Items.Interactors
{
    public class InteractorScoreCounter : FurniInteractor
  {
    public override void OnPlace(GameClient Session, Item Item)
    {
      if (Item.team == Team.none)
        return;
      Item.ExtraData = Item.GetRoom().GetGameManager().Points[(int)Item.team].ToString();
      Item.UpdateState(false, true);
    }

    public override void OnRemove(GameClient Session, Item Item)
    {
    }

    public override void OnTrigger(GameClient Session, Item Item, int Request, bool UserHasRights)
    {
      if (!UserHasRights)
        return;

      int num = 0;
      if (!string.IsNullOrEmpty(Item.ExtraData))
      {
        try
        {
          num = int.Parse(Item.ExtraData);
        }
        catch
        {
        }
      }
      if (Request == 1)
        num++;
      else if (Request == 2)
        num--;
      else if (Request == 3)
        num = 0;
      Item.ExtraData = num.ToString();
      Item.UpdateState(false, true);
    }
  }
}

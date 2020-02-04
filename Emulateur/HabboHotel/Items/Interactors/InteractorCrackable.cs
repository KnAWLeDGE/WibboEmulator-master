﻿// Type: Butterfly.HabboHotel.Items.Interactors.InteractorGenericSwitch




using Butterfly.HabboHotel.GameClients;

namespace Butterfly.HabboHotel.Items.Interactors
{
    public class InteractorCrackable : FurniInteractor
  {
    private readonly int Modes;

    public InteractorCrackable(int Modes)
    {
      this.Modes = Modes - 1;
      if (this.Modes >= 0)
        return;
      this.Modes = 0;
    }

    public override void OnPlace(GameClient Session, Item Item)
    {
    }

    public override void OnRemove(GameClient Session, Item Item)
    {
    }

    public override void OnTrigger(GameClient Session, Item Item, int Request, bool UserHasRights)
    {
      if (!UserHasRights || this.Modes == 0)
        return;
      int NumMode = 0;

      int.TryParse(Item.ExtraData, out NumMode);

      NumMode++;

      if (NumMode > this.Modes)
          NumMode = 0;

      
      Item.ExtraData = NumMode.ToString();
      Item.UpdateState();
    }
  }
}

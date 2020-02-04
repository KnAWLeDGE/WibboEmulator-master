﻿using Butterfly.HabboHotel.Items;
using Butterfly.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;
using Butterfly.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using Butterfly.Communication.Packets.Outgoing;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Pathfinding;

namespace Butterfly.HabboHotel.Rooms.Wired.WiredHandlers.Conditions
{
    public class HasFurniOnFurni : IWiredCondition, IWired
  {
    private Item item;
    private List<Item> items;
    private bool isDisposed;

    public HasFurniOnFurni(Item item, List<Item> items)
    {
      this.item = item;
      this.items = items;
      this.isDisposed = false;
    }

    public bool AllowsExecution(RoomUser user, Item TriggerItem)
    {
         Room theRoom = item.GetRoom();
         Gamemap map = theRoom.GetGameMap();
      foreach (Item roomItem in this.items)
      {
          foreach (ThreeDCoord coor in roomItem.GetAffectedTiles.Values)
          {
              if (!map.ValidTile(coor.X, coor.Y))
                  return false;

              if (map.ItemHeightMap[coor.X, coor.Y] > roomItem.TotalHeight)
                  return true;
          }
      }
      return false;
    }

    public void SaveToDatabase(IQueryAdapter dbClient)
    {
        WiredUtillity.SaveTriggerItem(dbClient, this.item.Id, string.Empty, string.Empty, false, this.items);
    }

    public void LoadFromDatabase(IQueryAdapter dbClient, Room insideRoom)
    {
        dbClient.SetQuery("SELECT triggers_item FROM wired_items WHERE trigger_id = " + this.item.Id);
        DataRow row = dbClient.GetRow();

        if (row == null)
            return;

        string itemlist = row["triggers_item"].ToString();

        if (itemlist == "")
            return;

        foreach (string item in itemlist.Split(';'))
      {
        Item roomItem = insideRoom.GetRoomItemHandler().GetItem(Convert.ToInt32(item));
        if (roomItem != null && !this.items.Contains(roomItem) && roomItem.Id != this.item.Id)
          this.items.Add(roomItem);
      }
    }

    public void OnTrigger(GameClient Session, int SpriteId)
    {
        ServerPacket Message = new ServerPacket(ServerPacketHeader.WiredConditionConfigMessageComposer);
        Message.WriteBoolean(false);
        Message.WriteInteger(10);
        Message.WriteInteger(this.items.Count);
        foreach (Item roomItem in this.items)
            Message.WriteInteger(roomItem.Id);
        Message.WriteInteger(SpriteId);
        Message.WriteInteger(this.item.Id);
        Message.WriteInteger(0);
        Message.WriteInteger(0);
        Message.WriteInteger(0);
        Message.WriteBoolean(false);
        Message.WriteBoolean(true);

        Session.SendPacket(Message);
    }

    public void DeleteFromDatabase(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("DELETE FROM wired_items WHERE trigger_id = '" + this.item.Id + "'");
    }

    public void Dispose()
    {
      this.isDisposed = true;
      this.item = (Item) null;
      if (this.items != null)
        this.items.Clear();
      this.items = (List<Item>) null;
    }

    public bool Disposed()
    {
      return this.isDisposed;
    }
  }
}

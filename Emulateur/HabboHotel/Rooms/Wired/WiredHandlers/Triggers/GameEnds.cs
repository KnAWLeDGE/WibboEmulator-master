﻿// Type: Butterfly.HabboHotel.Rooms.Wired.WiredHandlers.Triggers.GameEnds




using Butterfly.HabboHotel.Items;
using Butterfly.HabboHotel.Rooms.Games;
using Butterfly.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;
using Butterfly.Database.Interfaces;
using System;

using Butterfly.Communication.Packets.Outgoing;
using Butterfly.HabboHotel.GameClients;

namespace Butterfly.HabboHotel.Rooms.Wired.WiredHandlers.Triggers
{
    public class GameEnds : IWired
  {
    private Item item;
    private WiredHandler handler;
    private readonly RoomEventDelegate gameEndsDeletgate;

    public GameEnds(Item item, WiredHandler handler, GameManager gameManager)
    {
      this.item = item;
      this.handler = handler;
      this.gameEndsDeletgate = new RoomEventDelegate(this.gameManager_OnGameEnd);
      gameManager.OnGameEnd += this.gameEndsDeletgate;
    }

    private void gameManager_OnGameEnd(object sender, EventArgs e)
    {
      this.handler.ExecutePile(this.item.Coordinate, (RoomUser) null, null);
    }

    public void Dispose()
    {
      this.handler.GetRoom().GetGameManager().OnGameEnd -= this.gameEndsDeletgate;
      this.item = (Item) null;
      this.handler = (WiredHandler) null;
    }

    public void SaveToDatabase(IQueryAdapter dbClient)
    {
    }

    public void LoadFromDatabase(IQueryAdapter dbClient, Room insideRoom)
    {
    }

    public void DeleteFromDatabase(IQueryAdapter dbClient)
    {
    }
    public void OnTrigger(GameClient Session, int SpriteId)
    {
        ServerPacket Message3 = new ServerPacket(ServerPacketHeader.WiredTriggerConfigMessageComposer);
        Message3.WriteBoolean(false);
        Message3.WriteInteger(0);
        Message3.WriteInteger(0);
        Message3.WriteInteger(SpriteId);
        Message3.WriteInteger(this.item.Id);
        Message3.WriteString("");
        Message3.WriteInteger(0);
        Message3.WriteInteger(0);
        Message3.WriteInteger(8);
        Message3.WriteInteger(0);
        Message3.WriteInteger(0);
        Message3.WriteInteger(0);
        Session.SendPacket(Message3);
    }
  }
}

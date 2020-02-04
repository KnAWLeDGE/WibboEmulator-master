using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.Core;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.Catalog;
using Butterfly.HabboHotel.Catalog.Utilities;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Groups;
using Butterfly.HabboHotel.Items;
using Butterfly.HabboHotel.Pets;
using Butterfly.HabboHotel.Users.Inventory.Bots;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class PurchaseFromCatalogEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int PageId = Packet.PopInt();
            int ItemId = Packet.PopInt();
            string ExtraData = Packet.PopString();
            int Amount = Packet.PopInt();
            
            CatalogPage Page = null;
            if (!ButterflyEnvironment.GetGame().GetCatalog().TryGetPage(PageId, out Page))
                return;

            if (!Page.Enabled || Page.MinimumRank > Session.GetHabbo().Rank)
                return;

            CatalogItem Item = null;
            if (!Page.Items.TryGetValue(ItemId, out Item))
            {
                if (Page.ItemOffers.ContainsKey(ItemId))
                {
                    Item = (CatalogItem)Page.ItemOffers[ItemId];
                    if (Item == null)
                        return;
                }
                else
                    return;
            }

            if (Amount < 1 || Amount > 100 || !Item.HaveOffer)
                Amount = 1;

            int AmountPurchase = Item.Amount > 1 ? Item.Amount : Amount;

            int TotalCreditsCost = Amount > 1 ? ((Item.CostCredits * Amount) - ((int)Math.Floor((double)Amount / 6) * Item.CostCredits)) : Item.CostCredits;
            int TotalPixelCost = Amount > 1 ? ((Item.CostDuckets * Amount) - ((int)Math.Floor((double)Amount / 6) * Item.CostDuckets)) : Item.CostDuckets;
            int TotalDiamondCost = Amount > 1 ? ((Item.CostDiamonds * Amount) - ((int)Math.Floor((double)Amount / 6) * Item.CostDiamonds)) : Item.CostDiamonds;

            if (Session.GetHabbo().Credits < TotalCreditsCost || Session.GetHabbo().Duckets < TotalPixelCost || Session.GetHabbo().WibboPoints < TotalDiamondCost)
                return;

            int LimitedEditionSells = 0;
            int LimitedEditionStack = 0;

            #region Create the extradata
            switch (Item.Data.InteractionType)
            {
                case InteractionType.none:
                    ExtraData = "";
                    break;

                case InteractionType.GUILD_ITEM:
                case InteractionType.GUILD_GATE:
                    int GroupId = 0;
                    if (!int.TryParse(ExtraData, out GroupId))
                        return;
                    if (GroupId == 0)
                        return;
                    Group Group = null;
                    if (!ButterflyEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group))
                        return;
                    ExtraData = "0;" + Group.Id;
                    break;

                #region Pet handling

                case InteractionType.pet:
                    try
                    {
                        string[] Bits = ExtraData.Split('\n');
                        string PetName = Bits[0];
                        string Race = Bits[1];
                        string Color = Bits[2];

                        int.Parse(Race); // to trigger any possible errors

                        if (!PetUtility.CheckPetName(PetName))
                            return;

                        if (Race.Length > 2)
                            return;

                        if (Color.Length != 6)
                            return;

                        ButterflyEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_PetLover", 1);
                    }
                    catch (Exception e)
                    {
                        Logging.LogException((e).ToString());
                        return;
                    }

                    break;

                #endregion

                case InteractionType.FLOOR:
                case InteractionType.WALLPAPER:
                case InteractionType.LANDSCAPE:

                    Double Number = 0;

                    try
                    {
                        if (string.IsNullOrEmpty(ExtraData))
                            Number = 0;
                        else
                            Number = Double.Parse(ExtraData);
                    }
                    catch (Exception e)
                    {
                        Logging.LogException((e).ToString());
                    }

                    ExtraData = Number.ToString().Replace(',', '.');
                    break; // maintain extra data // todo: validate

                case InteractionType.POSTIT:
                    ExtraData = "FFFF33";
                    break;

                case InteractionType.MOODLIGHT:
                    ExtraData = "1,1,1,#000000,255";
                    break;

                case InteractionType.TROPHY:
                    ExtraData = Session.GetHabbo().Username + Convert.ToChar(9) + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + Convert.ToChar(9) + ExtraData;
                    break;

                case InteractionType.MANNEQUIN:
                    ExtraData = "m" + Convert.ToChar(5) + "ch-210-1321.lg-285-92" + Convert.ToChar(5) + "Default Mannequin";
                    break;

                case InteractionType.BADGE_TROC:
                    {
                        string[] BadgeNotAllowed = { "ADM", "GPHWIB", "wibbo.helpeur", "WIBARC", "CRPOFFI", "ZEERSWS", "PRWRD1", "WBI1", "WBI2", "WBI3", "WBI4", "WBI5", "WBI6", "WBI7", "CASINOB" };
                        if (BadgeNotAllowed.Any(x => x == ExtraData))
                        {
                            ButterflyEnvironment.GetGame().GetModerationTool().LogStaffEntry(Session.GetHabbo().Id, Session.GetHabbo().Username, 0, string.Empty, "notallowed", "Tentative de cr�ation Badge Troc: " + ExtraData);
                            Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.buybadgedisplay.error", Session.Langue));
                            Session.SendPacket(new PurchaseOKComposer());
                            return;
                        }

                        if (!Session.GetHabbo().GetBadgeComponent().HasBadge(ExtraData))
                        {
                            Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.buybadgedisplay.error", Session.Langue));
                            Session.SendPacket(new PurchaseOKComposer());
                            return;
                        }

                        Session.GetHabbo().GetBadgeComponent().RemoveBadge(ExtraData);
                        Session.SendPacket(Session.GetHabbo().GetBadgeComponent().Serialize());

                        break;
                    }

                case InteractionType.BADGE_DISPLAY:
                    if (!Session.GetHabbo().GetBadgeComponent().HasBadge(ExtraData))
                    {
                        Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.buybadgedisplay.error", Session.Langue));
                        Session.SendPacket(new PurchaseOKComposer());
                        return;
                    }

                    ExtraData = ExtraData + Convert.ToChar(9) + Session.GetHabbo().Username + Convert.ToChar(9) + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year;
                    break;

               case InteractionType.BADGE:
                    {
                        if (Session.GetHabbo().GetBadgeComponent().HasBadge(Item.Badge))
                        {
                            Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.buybadge.error", Session.Langue));
                            Session.SendPacket(new PurchaseOKComposer());
                            return;
                        }
                        break;
                    }
                default:
                    ExtraData = "";
                    break;
            }
            #endregion


            if (Item.IsLimited)
            {
                if (Item.LimitedEditionStack <= Item.LimitedEditionSells)
                {
                    Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.buyltd.error", Session.Langue));
                    Session.SendPacket(new PurchaseOKComposer());
                    return;
                }

                Item.LimitedEditionSells++;
                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("UPDATE `catalog_items` SET `limited_sells` = @limitSells WHERE `id` = @itemId LIMIT 1");
                    dbClient.AddParameter("limitSells", Item.LimitedEditionSells);
                    dbClient.AddParameter("itemId", Item.Id);
                    dbClient.RunQuery();

                    LimitedEditionSells = Item.LimitedEditionSells;
                    LimitedEditionStack = Item.LimitedEditionStack;
                }
            }

            if (Item.CostCredits > 0)
            {
                Session.GetHabbo().Credits -= TotalCreditsCost;
                Session.SendPacket(new CreditBalanceComposer(Session.GetHabbo().Credits));
            }

            if (Item.CostDuckets > 0)
            {
                Session.GetHabbo().Duckets -= TotalPixelCost;
                Session.SendPacket(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, Session.GetHabbo().Duckets));
            }

            if (Item.CostDiamonds > 0)
            {
                Session.GetHabbo().WibboPoints -= TotalDiamondCost;
                Session.SendPacket(new HabboActivityPointNotificationComposer(Session.GetHabbo().WibboPoints, 0, 105));

                using (IQueryAdapter queryreactor = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                    queryreactor.RunQuery("UPDATE users SET vip_points = vip_points - " + TotalDiamondCost + " WHERE id = " + Session.GetHabbo().Id);
            }
            

            Item NewItem = null;
            switch (Item.Data.Type.ToString().ToLower())
            {
                default:
                    List<Item> GeneratedGenericItems = new List<Item>();

                    switch (Item.Data.InteractionType)
                    {
                        default:
                            if (AmountPurchase > 1)
                            {
                                List<Item> Items = ItemFactory.CreateMultipleItems(Item.Data, Session.GetHabbo(), ExtraData, AmountPurchase);

                                if (Items != null)
                                {
                                    GeneratedGenericItems.AddRange(Items);
                                }
                            }
                            else
                            {
                                NewItem = ItemFactory.CreateSingleItemNullable(Item.Data, Session.GetHabbo(), ExtraData, LimitedEditionSells, LimitedEditionStack);

                                if (NewItem != null)
                                {
                                    GeneratedGenericItems.Add(NewItem);
                                }
                            }
                            break;

                        case InteractionType.TELEPORT:
                        case InteractionType.ARROW:
                            for (int i = 0; i < AmountPurchase; i++)
                            {
                                List<Item> TeleItems = ItemFactory.CreateTeleporterItems(Item.Data, Session.GetHabbo());

                                if (TeleItems != null)
                                {
                                    GeneratedGenericItems.AddRange(TeleItems);
                                }
                            }
                            break;

                        case InteractionType.MOODLIGHT:
                            {
                                if (AmountPurchase > 1)
                                {
                                    List<Item> Items = ItemFactory.CreateMultipleItems(Item.Data, Session.GetHabbo(), ExtraData, AmountPurchase);

                                    if (Items != null)
                                    {
                                        GeneratedGenericItems.AddRange(Items);
                                        foreach (Item I in Items)
                                        {
                                            ItemFactory.CreateMoodlightData(I);
                                        }
                                    }
                                }
                                else
                                {
                                    NewItem = ItemFactory.CreateSingleItemNullable(Item.Data, Session.GetHabbo(), ExtraData);

                                    if (NewItem != null)
                                    {
                                        GeneratedGenericItems.Add(NewItem);
                                        ItemFactory.CreateMoodlightData(NewItem);
                                    }
                                }
                            }
                            break;
                    }

                    foreach (Item PurchasedItem in GeneratedGenericItems)
                    {
                        if (Session.GetHabbo().GetInventoryComponent().TryAddItem(PurchasedItem))
                        {
                            Session.SendPacket(new FurniListNotificationComposer(PurchasedItem.Id, 1));
                        }
                    }
                    break;
                    
                case "r":
                    Bot Bot = BotUtility.CreateBot(Item.Data, Session.GetHabbo().Id);
                    if (Bot != null)
                    {
                        Session.GetHabbo().GetInventoryComponent().TryAddBot(Bot);
                        Session.SendPacket(new BotInventoryComposer(Session.GetHabbo().GetInventoryComponent().GetBots()));
                        Session.SendPacket(new FurniListNotificationComposer(Bot.Id, 5));
                    }
                    break;

                case "p":
                    {
                        string[] PetData = ExtraData.Split('\n');

                        Pet GeneratedPet = PetUtility.CreatePet(Session.GetHabbo().Id, PetData[0], Item.Data.SpriteId, PetData[1], PetData[2]);
                        if (GeneratedPet != null)
                        {
                            Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet);

                            Session.SendPacket(new FurniListNotificationComposer(GeneratedPet.PetId, 3));
                            Session.SendPacket(new PetInventoryComposer(Session.GetHabbo().GetInventoryComponent().GetPets()));

                            ItemData PetFood = null;
                            if (ButterflyEnvironment.GetGame().GetItemManager().GetItem(320, out PetFood))
                            {
                                Item Food = ItemFactory.CreateSingleItemNullable(PetFood, Session.GetHabbo(), "");
                                if (Food != null)
                                {
                                    Session.GetHabbo().GetInventoryComponent().TryAddItem(Food);
                                    Session.SendPacket(new FurniListNotificationComposer(Food.Id, 1));
                                }
                            }
                        }
                        break;
                    }

                case "b":
                    {
                        break;
                    }
            }

            if (!string.IsNullOrEmpty(Item.Badge) && !Session.GetHabbo().GetBadgeComponent().HasBadge(Item.Badge))
            {
                Session.GetHabbo().GetBadgeComponent().GiveBadge(Item.Badge, true);
                Session.SendPacket(new ReceiveBadgeComposer(Item.Badge));

                Session.SendPacket(new FurniListNotificationComposer(0, 4));
            }

            Session.SendPacket(new PurchaseOKComposer(Item, Item.Data));
        }
    }
}
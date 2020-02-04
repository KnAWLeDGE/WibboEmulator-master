﻿using System;
using System.Threading.Tasks;
using System.Threading;
using Butterfly.Core;
using Butterfly.HabboHotel.Achievements;
using Butterfly.HabboHotel.Catalog;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Groups;
using Butterfly.HabboHotel.Items;
using Butterfly.HabboHotel.Navigators;
using Butterfly.HabboHotel.Quests;
using Butterfly.HabboHotel.Roles;
using Butterfly.HabboHotel.Rooms;
using Butterfly.HabboHotel.Support;
using Butterfly.HabboHotel.HotelView;
using Butterfly.Database.Interfaces;
using System.Diagnostics;
using Butterfly.HabboHotel.Guides;
using Butterfly.Communication.Packets;
using Butterfly.HabboHotel.Rooms.Chat;
using Butterfly.HabboHotel.EffectsInventory;
using Butterfly.HabboHotel.WebClients;
using Butterfly.HabboHotel.Roleplay;
using Butterfly.HabboHotel.Animations;
using Butterfly.HabboHotel.NotifTop;

namespace Butterfly.HabboHotel
{
    public class Game
    {
        private readonly GameClientManager _clientManager;
        private readonly WebClientManager _clientWebManager;
        private readonly RoleManager _roleManager;
        private readonly CatalogManager _catalogManager;
        private readonly NavigatorManager _navigatorManager;
        private readonly ItemDataManager _itemDataManager;
        private readonly RoomManager _roomManager;
        private readonly AchievementManager _achievementManager;
        private readonly ModerationManager _moderationManager;
        private readonly QuestManager _questManager;
        private readonly GroupManager _groupManager;
        private readonly HotelViewManager _hotelViewManager;
        private readonly GuideManager _guideManager;
        private readonly PacketManager _packetManager;
        private readonly ChatManager _chatManager;
        private readonly EffectsInventoryManager _effectsInventory;
        private readonly RoleplayManager _roleplayManager;
        private readonly AnimationManager _animationManager;
        private readonly NotifTopManager _notiftopManager;

        private Task gameLoop;
        public static bool gameLoopEnabled = true;
        public bool gameLoopActive;
        public bool gameLoopEnded;

        private Stopwatch moduleWatch;

        public Game()
        {
            this._clientManager = new GameClientManager();
            this._clientWebManager = new WebClientManager();

            this._roleManager = new RoleManager();
            this._roleManager.Init();

            this._itemDataManager = new ItemDataManager();
            this._itemDataManager.Init();

            this._catalogManager = new CatalogManager();
            this._catalogManager.Init(this._itemDataManager);

            this._navigatorManager = new NavigatorManager();
            this._navigatorManager.Init();

            this._roleplayManager = new RoleplayManager();
            this._roleplayManager.Init();
            
            this._roomManager = new RoomManager();
            this._roomManager.LoadModels();

            this._groupManager = new GroupManager();
            this._groupManager.Init();

            this._moderationManager = new ModerationManager();
            this._moderationManager.LoadMessageTopics();
            this._moderationManager.LoadMessagePresets();
            this._moderationManager.LoadPendingTickets();
            this._moderationManager.LoadTicketResolution();

            this._questManager = new QuestManager();
            this._questManager.Initialize();

            this._hotelViewManager = new HotelViewManager();
            this._guideManager = new GuideManager();
            this._packetManager = new PacketManager();
            this._chatManager = new ChatManager();

            this._effectsInventory = new EffectsInventoryManager();
            this._effectsInventory.Init();

            this._achievementManager = new AchievementManager();

            this._animationManager = new AnimationManager();
            this._animationManager.Init();

            this._notiftopManager = new NotifTopManager();
            this._notiftopManager.Init();

            DatabaseCleanup();
            LowPriorityWorker.Init();

            this.moduleWatch = new Stopwatch();
        }

        #region Return values

        public NotifTopManager GetNotifTopManager()
        {
            return this._notiftopManager;
        }

        public AnimationManager GetAnimationManager()
        {
            return this._animationManager;
        }


        public EffectsInventoryManager GetEffectsInventoryManager()
        {
            return this._effectsInventory;
        }

        public ChatManager GetChatManager()
        {
            return this._chatManager;
        }

        public PacketManager GetPacketManager()
        {
            return _packetManager;
        }

        public GuideManager GetGuideManager()
        {
            return this._guideManager;
        }

        public RoleplayManager GetRoleplayManager()
        {
            return this._roleplayManager;
        }

        public GameClientManager GetClientManager()
        {
            return this._clientManager;
        }

        public WebClientManager GetClientWebManager()
        {
            return this._clientWebManager;
        }

        public RoleManager GetRoleManager()
        {
            return this._roleManager;
        }

        public CatalogManager GetCatalog()
        {
            return this._catalogManager;
        }

        public NavigatorManager GetNavigator()
        {
            return this._navigatorManager;
        }

        public ItemDataManager GetItemManager()
        {
            return this._itemDataManager;
        }

        public RoomManager GetRoomManager()
        {
            return this._roomManager;
        }

        public AchievementManager GetAchievementManager()
        {
            return this._achievementManager;
        }

        public ModerationManager GetModerationTool()
        {
            return this._moderationManager;
        }

        public QuestManager GetQuestManager()
        {
            return this._questManager;
        }

        public GroupManager GetGroupManager()
        {
            return this._groupManager;
        }

        public HotelViewManager GetHotelView()
        {
            return this._hotelViewManager;
        }
        #endregion

        public void StartGameLoop()
        {
            this.gameLoopActive = true;
            this.gameLoop = new Task(this.MainGameLoop, TaskCreationOptions.LongRunning);
            this.gameLoop.Start();
        }

        public void StopGameLoop()
        {
            this.gameLoopActive = false;
            int i = 0;
            while (!this.gameLoopEnded)
            {
                Thread.Sleep(250);
                i++;
                if (i > 100)
                    this.gameLoopEnded = true;
            }
            this.gameLoop.Dispose();
        }

        private void MainGameLoop()
        {

            while (this.gameLoopActive)
            {
                try
                {
                    if (gameLoopEnabled)
                    {
                        moduleWatch.Restart();

                        LowPriorityWorker.Process();

                        if (moduleWatch.ElapsedMilliseconds > 500)
                            Console.WriteLine("High latency in LowPriorityWorker.Process ({0} ms)", moduleWatch.ElapsedMilliseconds);
                        moduleWatch.Restart();

                        this._roomManager.OnCycle(moduleWatch);
                        this._animationManager.OnCycle(moduleWatch);

                        if (moduleWatch.ElapsedMilliseconds > 500)
                            Console.WriteLine("High latency in RoomManager ({0} ms)", moduleWatch.ElapsedMilliseconds);
                    }
                }
                catch (OperationCanceledException e)
                {
                    Console.WriteLine("Canceled operation {0}", e);

                }

                Thread.Sleep(500);
            }
            this.gameLoopEnded = true;
        }

        public static void DatabaseCleanup()
        {
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE users SET online = '0' WHERE online = '1'");
                dbClient.RunQuery("UPDATE users SET auth_ticket = '' WHERE auth_ticket != ''");
                dbClient.RunQuery("UPDATE user_websocket SET auth_ticket = '' WHERE auth_ticket != ''");
                dbClient.RunQuery("UPDATE rooms SET users_now = '0' WHERE users_now > '0'");
                dbClient.RunQuery("UPDATE server_status SET status = '1', users_online = '0', rooms_loaded = '0', stamp = '" + ButterflyEnvironment.GetUnixTimestamp() + "'");
            }
        }

        public void Destroy()
        {
            DatabaseCleanup();
            this.GetClientManager();
            Console.WriteLine("Destroyed Habbo Hotel.");
        }
    }
}
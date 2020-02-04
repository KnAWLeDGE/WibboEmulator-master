﻿using Butterfly.Core;
using Butterfly.HabboHotel;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Users;
using Butterfly.HabboHotel.Users.UserData;
using Butterfly.Net;
using Butterfly.Database;
using Butterfly.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Buttefly.Communication.Encryption;
using Buttefly.Communication.Encryption.Keys;
using Butterfly.Core.FigureData;
using ConnectionManager;
using System.Collections.Concurrent;
using Butterfly.Communication.WebSocket;
using System.Reflection;
using System.Threading;
using Butterfly.Communication.Packets.Outgoing;

namespace Butterfly
{
    public static class ButterflyEnvironment
    {
        private static ConfigurationData _configuration;
        private static ConnectionHandeling _connectionManager;
        private static WebSocketManager _webSocketManager;
        private static Game _game;
        private static DatabaseManager _datebasemanager;
        private static RCONSocket _rcon;
        private static FigureDataManager _figureManager;
        private static LanguageManager _languageManager;

        private static ConcurrentDictionary<int, Habbo> _usersCached = new ConcurrentDictionary<int, Habbo>();

        public static DateTime ServerStarted;
        public static bool StaticEvents;
        public static string PatchDir;

        public static Random Random = new Random();
        public static int onlineUsersFr = 0;
        public static int onlineUsersEn = 0;
        public static int onlineUsersBr = 0;
        private static readonly List<char> Allowedchars = new List<char>(new[]
            {
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l',
                'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x',
                'y', 'z',
                '1', '2', '3', '4', '5', '6', '7', '8', '9', '0',
                '-', '.', '=', '?', '!', ':',
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L',
                'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X',
                'Y', 'Z'
            });

        public static void Initialize()
        {
            Console.Clear();

            ServerStarted = DateTime.Now;

            Console.ForegroundColor = ConsoleColor.Gray;

            PatchDir = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/";

            Console.Title = "Butterfly Emulator";

            try
            {
                _configuration = new ConfigurationData(PatchDir + "Settings/configuration.ini", false);
                _datebasemanager = new DatabaseManager(uint.Parse(GetConfig().data["db.pool.maxsize"]), uint.Parse(GetConfig().data["db.pool.minsize"]), GetConfig().data["db.hostname"], uint.Parse(GetConfig().data["db.port"]), GetConfig().data["db.username"], GetConfig().data["db.password"], GetConfig().data["db.name"]);


                int TryCount = 0;
                while(!_datebasemanager.IsConnected())
                {
                    TryCount++;
                    Thread.Sleep(5000); //sleep 5sec

                    if (TryCount > 10)
                    {
                        Logging.WriteLine("Failed to connect to the specified MySQL server.");
                        Console.ReadKey(true);
                        Environment.Exit(1);
                        return;
                    }
                }
                
                HabboEncryptionV2.Initialize(new RSAKeys());

                _languageManager = new LanguageManager();
                _languageManager.Init();

                _game = new Game();
                _game.StartGameLoop();

                _figureManager = new FigureDataManager();
                _figureManager.Init();

                if(_configuration.data["Websocketenable"] == "true")
                    _webSocketManager = new WebSocketManager(527, int.Parse(GetConfig().data["game.tcp.conlimit"]), int.Parse(GetConfig().data["game.tcp.conlimit"]));

                _connectionManager = new ConnectionHandeling(int.Parse(GetConfig().data["game.tcp.port"]), int.Parse(GetConfig().data["game.tcp.conlimit"]), int.Parse(GetConfig().data["game.tcp.conperip"]));

                if (_configuration.data["Musenable"] == "true")
                    _rcon = new RCONSocket(int.Parse(GetConfig().data["mus.tcp.port"]), GetConfig().data["mus.tcp.allowedaddr"].Split(new char[1] { ';' }));

                StaticEvents = _configuration.data["static.events"] == "true";
                
                Logging.WriteLine("ENVIRONMENT -> READY!");

                if (Debugger.IsAttached)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Logging.WriteLine("Server is debugging: Console writing enabled");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Logging.WriteLine("Server is not debugging: Console writing disabled");
                    Logging.DisablePrimaryWriting(false);
                }
            }
            catch (KeyNotFoundException ex)
            {
                Logging.WriteLine("Please check your configuration file - some values appear to be missing.");
                Logging.WriteLine("Press any key to shut down ...");
                Logging.WriteLine((ex).ToString());
                Console.ReadKey(true);
            }
            catch (InvalidOperationException ex)
            {
                Logging.WriteLine("Failed to initialize ButterflyEmulator: " + ex.Message);
                Logging.WriteLine("Press any key to shut down ...");
                Console.ReadKey(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fatal error during startup: " + (ex).ToString());
                Console.WriteLine("Press a key to exit");
                Console.ReadKey();
                Environment.Exit(1);
            }
        }

        public static bool EnumToBool(string Enum)
        {
            return Enum == "1";
        }

        public static string BoolToEnum(bool Bool)
        {
            return Bool ? "1" : "0";
        }

        public static int GetRandomNumber(int Min, int Max)
        {
            return Random.Next(Min, Max + 1);
        }

        public static int GetUnixTimestamp()
        {
            return (int)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
        }

        public static FigureDataManager GetFigureManager()
        {
            return _figureManager;
        }

        private static bool isValid(char character)
        {
            return Allowedchars.Contains(character);
        }

        public static bool IsValidAlphaNumeric(string inputStr)
        {
            if (string.IsNullOrEmpty(inputStr))
                return false;
            for (int index = 0; index < inputStr.Length; ++index)
            {
                if (!isValid(inputStr[index]))
                    return false;
            }
            return true;
        }

        public static bool usernameExists(string username)
        {
            int integer;
            using (IQueryAdapter queryreactor = GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.SetQuery("SELECT id FROM users WHERE username = @username LIMIT 1");
                queryreactor.AddParameter("username", username);
                integer = queryreactor.GetInteger();
            }
            if (integer <= 0)
                return false;

            return true;
        }

        public static string GetUsernameById(int UserId)
        {
            string Name = "Unknown User";

            GameClient Client = GetGame().GetClientManager().GetClientByUserID(UserId);
            if (Client != null && Client.GetHabbo() != null)
                return Client.GetHabbo().Username;


            if (_usersCached.ContainsKey(UserId))
                return _usersCached[UserId].Username;

            using (IQueryAdapter dbClient = GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `username` FROM `users` WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("id", UserId);
                Name = dbClient.GetString();
            }

            if (string.IsNullOrEmpty(Name))
                Name = "Unknown User";

            return Name;
        }

        public static Habbo GetHabboByUsername(string UserName)
        {
            try
            {
                using (IQueryAdapter dbClient = GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("SELECT `id` FROM `users` WHERE `username` = @user LIMIT 1");
                    dbClient.AddParameter("user", UserName);
                    int id = dbClient.GetInteger();
                    if (id > 0)
                        return GetHabboById(Convert.ToInt32(id));
                }
                return null;
            }
            catch { return null; }
        }

        public static Habbo GetHabboById(int UserId)
        {
            try
            {
                GameClient Client = GetGame().GetClientManager().GetClientByUserID(UserId);
                if (Client != null)
                {
                    Habbo User = Client.GetHabbo();
                    if (User != null && User.Id > 0)
                    {
                        if (_usersCached.ContainsKey(UserId))
                            _usersCached.TryRemove(UserId, out User);

                        return User;
                    }
                }
                else
                {
                    try
                    {
                        if (_usersCached.ContainsKey(UserId))
                            return _usersCached[UserId];
                        else
                        {
                            UserData data = UserDataFactory.GetUserData(UserId);
                            if (data != null)
                            {
                                Habbo Generated = data.user;
                                if (Generated != null)
                                {
                                    _usersCached.TryAdd(UserId, Generated);
                                    return Generated;
                                }
                            }
                        }
                    }
                    catch { return null; }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public static LanguageManager GetLanguageManager()
        {
            return _languageManager;
        }

        public static ConfigurationData GetConfig()
        {
            return _configuration;
        }

        public static ConnectionHandeling GetConnectionManager()
        {
            return _connectionManager;
        }

        public static RCONSocket GetRCONSocket()
        {
            return _rcon;
        }

        public static Game GetGame()
        {
            return _game;
        }

        public static DatabaseManager GetDatabaseManager()
        {
            return _datebasemanager;
        }

        public static void PreformShutDown()
        {
            PreformShutDown(true);
        }

        public static void PreformShutDown(bool ExitWhenDone)
        {
            StringBuilder builder = new StringBuilder();

            DateTime now1 = DateTime.Now;

            DateTime now2 = DateTime.Now;

            ServerPacket message = new ServerPacket(ServerPacketHeader.BroadcastMessageAlertMessageComposer);
            message.WriteString("L'hôtel subit un redémarrage d'entretien, nous revenons dans quelques instants, nous nous excusons pour la gêne occasionnée !");
            GetGame().GetClientManager().SendMessage(message);

            Thread.Sleep(10000);

            AppendTimeStampWithComment(ref builder, now2, "Hotel pre-warning");

            _game.StopGameLoop();
            Console.Write("Game loop stopped");

            DateTime now3 = DateTime.Now;
            Console.WriteLine("Server shutting down...");
            Console.Title = "<<- SERVER SHUTDOWN ->>";

            GetConnectionManager().Destroy();
            AppendTimeStampWithComment(ref builder, now3, "Socket close");

            DateTime now5 = DateTime.Now;
            Console.WriteLine("<<- SERVER SHUTDOWN ->> ROOM SAVE");
            _game.GetRoomManager().RemoveAllRooms();
            AppendTimeStampWithComment(ref builder, now5, "Room destructor");

            DateTime now4 = DateTime.Now;
            GetGame().GetClientManager().CloseAll();
            AppendTimeStampWithComment(ref builder, now4, "Furni pre-save and connection close");

            //ButterflyEnvironment.Game.GetPromotedRooms().executeUpdates();

            DateTime now7 = DateTime.Now;
            if(_connectionManager != null)
                _connectionManager.Destroy();
            if(_webSocketManager != null)
                _webSocketManager.Destroy();

            AppendTimeStampWithComment(ref builder, now7, "Connection shutdown");

            DateTime now8 = DateTime.Now;
            _game.Destroy();
            AppendTimeStampWithComment(ref builder, now8, "Game destroy");

            DateTime now9 = DateTime.Now;

            TimeSpan span = DateTime.Now - now1;
            builder.AppendLine("Total time on shutdown " + TimeSpanToString(span));
            builder.AppendLine("You have reached ==> [END OF SESSION]");
            builder.AppendLine();
            builder.AppendLine();
            builder.AppendLine();
            Logging.LogShutdown(builder);
            Console.WriteLine("System disposed, goodbye!");
            if (!ExitWhenDone)
                return;
            Environment.Exit(Environment.ExitCode);
        }

        public static string TimeSpanToString(TimeSpan span)
        {
            return string.Concat(new object[4]
      {
         span.Seconds,
         " s, ",
         span.Milliseconds,
         " ms"
      });
        }

        public static void AppendTimeStampWithComment(ref StringBuilder builder, DateTime time, string text)
        {
            builder.AppendLine(text + " =>[" + TimeSpanToString(DateTime.Now - time) + "]");
        }
    }
}

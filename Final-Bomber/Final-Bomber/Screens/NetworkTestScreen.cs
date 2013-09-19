using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Final_Bomber.Entities.AI;
using Microsoft.Xna.Framework;
using Final_Bomber.Controls;
using Microsoft.Xna.Framework.Input;
using Lidgren.Network;
using Final_Bomber.Network;
using System.Diagnostics;
using Final_Bomber.Network.Core;
using Final_Bomber.Entities;
using Final_Bomber.TileEngine;
using Microsoft.Xna.Framework.Graphics;
using Final_Bomber.WorldEngine;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace Final_Bomber.Screens
{
    public class NetworkTestScreen : BaseGameState
    {
        #region Field region
        Process _serverProcess;
        private string _publicIp;
        private bool _hasConnected;

        private OnlineHumanPlayer _me;

        // Game logic
        Engine _engine;

        // Map
        private Map _map;

        // HUD
        Point _hudOrigin;

        // Entity collection
        private EntityCollection _entities;
        private PlayerCollection _players;

        // Dead Players number
        int _deadPlayersNumber;

        // Random
        private static Random Random { get; set; }

        // Timer
        private Stopwatch _timer;

        #endregion

        #region Property region

        public World World { get; set; }

        // Sudden Death
        public SuddenDeath SuddenDeath { get; private set; }

        #endregion

        #region Constructor region
        public NetworkTestScreen(Game game, GameStateManager manager)
            : base(game, manager)
        {
            _publicIp = "?";
        }
        #endregion

        #region XNA Method region

        public override void Initialize()
        {
            _timer = new Stopwatch();
            _timer.Start();

            Random = new Random();
            _hudOrigin = new Point(GameRef.GraphicsDevice.Viewport.Width - 234, 0);

            // Launch the dedicated server as host
            _serverProcess = new Process 
            { 
                StartInfo = 
                { 
                    FileName = "Server.exe", 
                    Arguments = "COUCOU", 
                    //WindowStyle = ProcessWindowStyle.Hidden 
                } 
            };
            
            //_serverProcess.Start();

            _players = new PlayerCollection();
            _entities = new EntityCollection();

            _hasConnected = false;

            // Events
            GameSettings.GameServer.StartInfo += new GameServer.StartInfoEventHandler(GameServer_StartInfo);
            GameSettings.GameServer.StartGame += new GameServer.StartGameEventHandler(GameServer_StartGame);

            GameSettings.GameServer.NewPlayer += new GameServer.NewPlayerEventHandler(GameServer_NewPlayer);
            GameSettings.GameServer.MovePlayer += new GameServer.MovePlayerEventHandler(GameServer_MovePlayer);
            GameSettings.GameServer.End += new GameServer.EndEventHandler(GameServer_End);

            // Engine
            _engine = new Engine(32, 32, Vector2.Zero);

            // Map
            _map = new Map();

            base.Initialize();

            Reset();
        }

        protected override void LoadContent()
        {
            _map.LoadContent();
            _map.Parse("classic.map");

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            GameSettings.GameServer.EndClientConnection("Quit the game !");

            //_serverProcess.Kill();

            GameSettings.GameServer.StartInfo -= new GameServer.StartInfoEventHandler(GameServer_StartInfo);
            GameSettings.GameServer.StartGame -= new GameServer.StartGameEventHandler(GameServer_StartGame);

            GameSettings.GameServer.NewPlayer -= new GameServer.NewPlayerEventHandler(GameServer_NewPlayer);
            GameSettings.GameServer.MovePlayer -= new GameServer.MovePlayerEventHandler(GameServer_MovePlayer);
            GameSettings.GameServer.End -= new GameServer.EndEventHandler(GameServer_End);

            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            ControlManager.Update(gameTime, PlayerIndex.One);

            if (!_hasConnected)
            {
                GameSettings.GameServer.StartClientConnection("127.0.0.1", "2643");

                var connectedTmr = new Timer();
                connectedTmr.Start();
                while (!_hasConnected)
                {
                    GameSettings.GameServer.RunClientConnection();
                    if (GameSettings.GameServer.Connected)
                    {
                        _hasConnected = true;
                        //_publicIp = GetPublicIP();
                    }
                    else
                    {
                        if (connectedTmr.Each(5000))
                        {
                            Debug.Print("Couldn't connect to the Game Server, please refresh the game list");
                            GameRef.Exit();
                        }
                    }
                }
            }
            else
            {
                if (GameSettings.GameServer.HasStarted)
                    GameSettings.GameServer.RunClientConnection();

                foreach (Player p in _players)
                {
                    p.Update(gameTime);
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GameRef.SpriteBatch.Begin();

            ControlManager.Draw(GameRef.SpriteBatch);

            if (_hasConnected)
            {
                World.DrawLevel(gameTime, GameRef.SpriteBatch, null);

                foreach (var player in _players)
                    player.Draw(gameTime);
            }

            const string str = "Networking Tests";
            GameRef.SpriteBatch.DrawString(this.BigFont, str,
                    new Vector2(
                        Config.Resolutions[Config.IndexResolution, 0] / 2f -
                        this.BigFont.MeasureString(str).X / 2,
                        0),
            Color.Black);

            // Draw IP adress
            GameRef.SpriteBatch.DrawString(this.BigFont, _publicIp, new Vector2(530, 60), Color.Black);

            // Draw ping
            GameRef.SpriteBatch.DrawString(this.BigFont, GameServer.Ping.ToString(), new Vector2(740, 100), Color.Black);

            GameRef.SpriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion

        private void Reset()
        {
            //_timer = TimeSpan.Zero;
            _deadPlayersNumber = 0;

            CreateWorld();

            var origin = new Vector2(_hudOrigin.X / 2 - ((32 * World.Levels[World.CurrentLevel].Size.X) / 2),
                GameRef.GraphicsDevice.Viewport.Height / 2 - ((32 * World.Levels[World.CurrentLevel].Size.Y) / 2));

            Engine.Origin = origin;

            SuddenDeath = new SuddenDeath(GameRef, Config.PlayersPositions[0]);
        }

        private void CreateWorld()
        {
            World = new World(GameRef, GameRef.ScreenRectangle);
            World.Levels.Add(_map);
            World.CurrentLevel = 0;

            /*
            foreach (int playerID in playerPositions.Keys)
            {
                if (Config.AIPlayers[playerID])
                {
                    var player = new AIPlayer(Math.Abs(playerID));
                    PlayerList.Add(player);
                    board[playerPositions[playerID].X, playerPositions[playerID].Y] = player;
                }
                else
                {
                    var player = new HumanPlayer(Math.Abs(playerID));
                    PlayerList.Add(player);
                    board[playerPositions[playerID].X, playerPositions[playerID].Y] = player;
                }
            }
            */

            _me = new OnlineHumanPlayer(0) { Name = "Me" };
            _players.Add(_me);
        }

        #region Game Server events

        private bool _isReady = false;
        private void GameServer_StartInfo() 
        {
            _isReady = true;
        }

        private void GameServer_StartGame(bool gameInProgress, int playerId, float moveSpeed, int suddenDeathTime)
        {
            if (true /*!mainGame.IsStarted*/)
            {
                /*
                if (lobbyScreen.IsLoaded)
                {
                    lobbyScreen.End();
                    lobbyScreen.Unload();
                }
                */
                if (!gameInProgress)
                {
                    _me.Id = playerId;
                    /*
                    me.MoveSpeed = moveSpeed;
                    suddenDeathTime = suddenDeathTime;
                    */
                }
                else
                {
                    /*
                    mainGame.me.Kill();
                    mainGame.Spectator = true;
                    */
                }

                //mainGame.Start();
            }
        }


        private void GameServer_NewPlayer(int playerID, float moveSpeed, string username)
        {
            if (_players.GetPlayerByID(playerID) == null)
            {
                var player = new OnlinePlayer(playerID)
                {
                    Name = "Online Player"
                };
                //player.MoveSpeed = moveSpeed;
                //Entities.Add(player);
                _players.Add(player);
            }
        }

        private void GameServer_MovePlayer(object sender, MovePlayerArgs arg)
        {
            Player player = _players.GetPlayerByID(arg.PlayerID);

            if (player != null)
            {
                // TODO => Move Players on the map
                player.Sprite.Position = arg.Position;
                player.ChangeLookDirection(arg.Action);
                /*
                player.MapPosition = arg.pos;
                if (arg.action != 255)
                    player.movementAction = (Player.ActionEnum)arg.action;
                player.UpdateAnimation();
                */
            }
        }

        private void GameServer_End(bool won)
        {
            /*
            endTmr.Start();
            if (!Spectator)
            {
                shouldEnd = true;
                haveWon = won;
            }
            */
        }

        #endregion

        private string GetMyIpAddress()
        {
            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                }
            }
            return localIP;
        }

private string GetPublicIP()
{
    String direction = "";
    WebRequest request = WebRequest.Create("http://checkip.dyndns.org/");
    using (WebResponse response = request.GetResponse())
    using (StreamReader stream = new StreamReader(response.GetResponseStream()))
    {
        direction = stream.ReadToEnd();
    }

    //Search for the ip in the html
    int first = direction.IndexOf("Address: ") + 9;
    int last = direction.LastIndexOf("</body>");
    direction = direction.Substring(first, last - first);

    return direction;
}
    }

}

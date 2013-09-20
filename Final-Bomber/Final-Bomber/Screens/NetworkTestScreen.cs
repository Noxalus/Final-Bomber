using System;
using FBLibrary.Core;
using Final_Bomber.Core;
using Final_Bomber.Core.Players;
using Microsoft.Xna.Framework;
using Final_Bomber.Controls;
using Final_Bomber.Network;
using System.Diagnostics;
using Final_Bomber.Entities;
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

        // Game manager
        private GameManager _gameManager;

        // HUD
        Point _hudOrigin;

        // Timer
        private Stopwatch _timer;

        #endregion

        #region Constructor region
        public NetworkTestScreen(Game game, GameStateManager manager)
            : base(game, manager)
        {
            _publicIp = "?";

            _gameManager = new GameManager();
        }
        #endregion

        #region XNA Method region

        public override void Initialize()
        {
            _timer = new Stopwatch();
            _timer.Start();

            _hudOrigin = new Point(FinalBomber.Instance.GraphicsDevice.Viewport.Width - 234, 0);

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

            _hasConnected = false;

            // Events TODO: Move theses events (and associated methods) into a specific network class
            GameSettings.GameServer.StartInfo += new GameServer.StartInfoEventHandler(GameServer_StartInfo);
            GameSettings.GameServer.StartGame += new GameServer.StartGameEventHandler(GameServer_StartGame);

            GameSettings.GameServer.NewPlayer += new GameServer.NewPlayerEventHandler(GameServer_NewPlayer);
            GameSettings.GameServer.MovePlayer += new GameServer.MovePlayerEventHandler(GameServer_MovePlayer);
            GameSettings.GameServer.End += new GameServer.EndEventHandler(GameServer_End);

            base.Initialize();

            _gameManager.Reset();
        }

        protected override void LoadContent()
        {
            _gameManager.LoadMap("classic.map");

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
                            FinalBomber.Instance.Exit();
                        }
                    }
                }
            }
            else
            {
                if (GameSettings.GameServer.HasStarted)
                {
                    GameSettings.GameServer.RunClientConnection();

                    _gameManager.Update(gameTime);
                }

            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            FinalBomber.Instance.SpriteBatch.Begin();

            ControlManager.Draw(FinalBomber.Instance.SpriteBatch);

            if (_hasConnected)
            {
                _gameManager.Draw(gameTime);
            }

            const string str = "Networking Tests";
            FinalBomber.Instance.SpriteBatch.DrawString(this.BigFont, str,
                    new Vector2(
                        Config.Resolutions[Config.IndexResolution, 0] / 2f -
                        this.BigFont.MeasureString(str).X / 2,
                        0),
            Color.Black);

            // Draw IP adress
            FinalBomber.Instance.SpriteBatch.DrawString(this.BigFont, _publicIp, new Vector2(530, 60), Color.Black);

            // Draw ping
            FinalBomber.Instance.SpriteBatch.DrawString(this.BigFont, GameServer.Ping.ToString(), new Vector2(740, 100), Color.Black);

            FinalBomber.Instance.SpriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion



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
                    _gameManager.Me.Id = playerId;
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
            if (_gameManager.Players.GetPlayerByID(playerID) == null)
            {
                var player = new OnlinePlayer(playerID)
                {
                    Name = "Online Player"
                };
                //player.MoveSpeed = moveSpeed;
                //Entities.Add(player);
                _gameManager.Players.Add(player);
            }
        }

        private void GameServer_MovePlayer(object sender, MovePlayerArgs arg)
        {
            Player player = _gameManager.Players.GetPlayerByID(arg.PlayerID);

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

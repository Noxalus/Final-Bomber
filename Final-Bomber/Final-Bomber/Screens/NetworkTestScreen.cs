using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        private OnlineHumanPlayer me;

        // Game logic
        Engine _engine;

        // Map
        Point _mapSize;
        Texture2D _mapTexture;
        Texture2D _wallTexture;

        // HUD
        Point _hudOrigin;

        // Entity collection
        public EntityCollection Entities;
        public PlayerCollection Players;
        private List<Wall> _wallList;
        private List<Item> _itemList;
        private List<EdgeWall> _edgeWallList;
        public List<Bomb> BombList { get; private set; }
        public List<UnbreakableWall> UnbreakableWallList { get; private set; }
        public List<Teleporter> TeleporterList { get; private set; }
        public List<Arrow> ArrowList { get; set; }

        // Dead Players number
        int _deadPlayersNumber;

        // Random
        public static Random Random { get; private set; }

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
            _serverProcess = new Process {StartInfo = {FileName = "Server.exe", Arguments = "COUCOU"}};
            //server.Start();

            Players = new PlayerCollection();
            Entities = new EntityCollection();

            _hasConnected = false;

            // Events
            GameSettings.GameServer.StartInfo += new GameServer.StartInfoEventHandler(GameServer_StartInfo);
            GameSettings.GameServer.StartGame += new GameServer.StartGameEventHandler(GameServer_StartGame);

            GameSettings.GameServer.NewPlayer += new GameServer.NewPlayerEventHandler(GameServer_NewPlayer);
            GameSettings.GameServer.MovePlayer += new GameServer.MovePlayerEventHandler(GameServer_MovePlayer);
            GameSettings.GameServer.End += new GameServer.EndEventHandler(GameServer_End);

            base.Initialize();

            Reset();
        }

        protected override void LoadContent()
        {
            _wallTexture = GameRef.Content.Load<Texture2D>("Graphics/Characters/edgeWall");
            _mapTexture = GameRef.Content.Load<Texture2D>("Graphics/Tilesets/tileset1");

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            GameSettings.GameServer.EndClientConnection("Quit the game !");
            /*
            server.Close();
            server.Kill();
            */

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

                Timer connectedTmr = new Timer();
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

                foreach (Player p in Players)
                {
                    p.Update(gameTime);
                }
            }

            /*
            var config = new NetPeerConfiguration("Final-Bomber");

            NetClient client;
            client = new NetClient(config);

            client.Connect("127.0.0.1", 2643);

            //client.Connect(Ip, int.Parse(2643));

            //msgOut = client.CreateMessage();
            /*
            NetOutgoingMessage sendMsg = server.CreateMessage();
            sendMsg.Write("Hello");
            sendMsg.Write(42);

            server.SendMessage(sendMsg, recipient, NetDeliveryMethod.ReliableOrdered);
            */

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GameRef.SpriteBatch.Begin();

            base.Draw(gameTime);

            ControlManager.Draw(GameRef.SpriteBatch);

            if (_hasConnected)
            {
                // Background
                var position = new Vector2(
                    Engine.Origin.X - (int)(Engine.Origin.X / Engine.TileWidth) * Engine.TileWidth - Engine.TileWidth,
                    Engine.Origin.Y - (int)(Engine.Origin.Y / Engine.TileHeight) * Engine.TileHeight - Engine.TileHeight);

                for (int i = 0; i < (GraphicsDevice.Viewport.Width / Engine.TileWidth) + 2; i++)
                {
                    for (int j = 0; j < (GraphicsDevice.Viewport.Height / Engine.TileHeight) + 2; j++)
                    {
                        if (!((position.X + i * Engine.TileWidth > Engine.Origin.X &&
                            position.X + i * Engine.TileWidth < Engine.Origin.X + _mapSize.X * Engine.TileWidth - Engine.TileWidth) &&
                            (position.Y + j * Engine.TileHeight > Engine.Origin.Y &&
                            position.Y + j * Engine.TileHeight < Engine.Origin.Y + _mapSize.Y * Engine.TileHeight - Engine.TileHeight)))
                        {
                            GameRef.SpriteBatch.Draw(_wallTexture, new Vector2(position.X + (i * Engine.TileWidth), position.Y + (j * Engine.TileHeight)), Color.White);
                        }
                    }
                }

                World.DrawLevel(gameTime, GameRef.SpriteBatch, null);

                #region Draw each entities

                foreach (EdgeWall e in _edgeWallList)
                    e.Draw(gameTime);

                foreach (UnbreakableWall u in UnbreakableWallList)
                    u.Draw(gameTime);

                foreach (Wall w in _wallList)
                    w.Draw(gameTime);

                foreach (Item i in _itemList)
                    i.Draw(gameTime);

                foreach (Teleporter t in TeleporterList)
                    t.Draw(gameTime);

                foreach (Arrow a in ArrowList)
                    a.Draw(gameTime);

                foreach (Bomb b in BombList)
                    b.Draw(gameTime);

                foreach (Player p in Players)
                    p.Draw(gameTime);

                #endregion
            }

            string str = "Networking Tests";
            GameRef.SpriteBatch.DrawString(this.BigFont, str,
                    new Vector2(
                        Config.Resolutions[Config.IndexResolution, 0] / 2 -
                        this.BigFont.MeasureString(str).X / 2,
                        0),
            Color.Black);

            // Draw IP adress
            GameRef.SpriteBatch.DrawString(this.BigFont, _publicIp, new Vector2(530, 60), Color.Black);

            // Draw ping
            GameRef.SpriteBatch.DrawString(this.BigFont, GameServer.Ping.ToString(), new Vector2(740, 100), Color.Black);

            GameRef.SpriteBatch.End();
        }

        #endregion

        private void Reset()
        {
            //_timer = TimeSpan.Zero;

            _engine = new Engine(32, 32, Vector2.Zero);

            // Lists
            _wallList = new List<Wall>();
            _itemList = new List<Item>();
            BombList = new List<Bomb>();
            UnbreakableWallList = new List<UnbreakableWall>();
            _edgeWallList = new List<EdgeWall>();
            TeleporterList = new List<Teleporter>();
            ArrowList = new List<Arrow>();

            _deadPlayersNumber = 0;

            CreateWorld();

            var origin = new Vector2(_hudOrigin.X / 2 - ((32 * World.Levels[World.CurrentLevel].Size.X) / 2),
                GameRef.GraphicsDevice.Viewport.Height / 2 - ((32 * World.Levels[World.CurrentLevel].Size.Y) / 2));

            Engine.Origin = origin;

            SuddenDeath = new SuddenDeath(GameRef, Config.PlayersPositions[0]);
        }

        private void CreateWorld()
        {
            var tilesets = new List<Tileset>() { new Tileset(_mapTexture, 64, 32, 32, 32) };

            var collisionLayer = new bool[Config.MapSize.X, Config.MapSize.Y];
            var mapPlayersPosition = new int[Config.MapSize.X, Config.MapSize.Y];
            var map = new Entity[Config.MapSize.X, Config.MapSize.Y];
            var layer = new MapLayer(Config.MapSize.X, Config.MapSize.Y);
            var voidPosition = new List<Point>();

            // Item Map

            // List of player position
            var playerPositions = new Dictionary<int, Point>();

            // We don't put wall around the players
            for (int i = 0; i < Config.PlayersNumber; i++)
            {
                Point playerPosition = Config.PlayersPositions[i];
                playerPositions[i + 1] = playerPosition;

                mapPlayersPosition[playerPosition.X, playerPosition.Y] = 2;
                mapPlayersPosition[playerPosition.X + 1, playerPosition.Y] = 1;
                mapPlayersPosition[playerPosition.X, playerPosition.Y + 1] = 1;
                mapPlayersPosition[playerPosition.X, playerPosition.Y - 1] = 1;
                mapPlayersPosition[playerPosition.X - 1, playerPosition.Y] = 1;

                mapPlayersPosition[playerPosition.X - 1, playerPosition.Y - 1] = 1;
                mapPlayersPosition[playerPosition.X - 1, playerPosition.Y + 1] = 1;
                mapPlayersPosition[playerPosition.X + 1, playerPosition.Y - 1] = 1;
                mapPlayersPosition[playerPosition.X + 1, playerPosition.Y + 1] = 1;
            }

            /*
            mapItem = new Teleporter(GameRef, new Vector2(
                        5 * Engine.TileWidth,
                        1 * Engine.TileHeight));
            teleporterList.Add((Teleporter)mapItem);
            map[5,1] = mapItem;

            mapItem = new Teleporter(GameRef, new Vector2(
                        10 * Engine.TileWidth,
                        1 * Engine.TileHeight));
            teleporterList.Add((Teleporter)mapItem);
            map[10, 1] = mapItem;
            */

            for (int x = 0; x < Config.MapSize.X; x++)
            {
                for (int y = 0; y < Config.MapSize.Y; y++)
                {
                    if (!(x == 0 || y == 0 || x == (Config.MapSize.X - 1) || y == (Config.MapSize.Y - 1) ||
                        (x % 2 == 0 && y % 2 == 0)) && (mapPlayersPosition[x, y] != 1 && mapPlayersPosition[x, y] != 2))
                        voidPosition.Add(new Point(x, y));
                }
            }

            #region Teleporter
            /*
            if (Config.ActiveTeleporters)
            {
                if (Config.TeleporterPositionType == TeleporterPositionTypeEnum.Randomly)
                {
                    int randomVoid = 0;
                    for (int i = 0; i < MathHelper.Clamp(Config.TeleporterNumber, 0, voidPosition.Count - 1); i++)
                    {
                        randomVoid = Random.Next(voidPosition.Count);
                        mapItem = new Teleporter(GameRef, new Vector2(
                            voidPosition[randomVoid].X * Engine.TileWidth,
                            voidPosition[randomVoid].Y * Engine.TileHeight));
                        TeleporterList.Add((Teleporter)mapItem);
                        map[voidPosition[randomVoid].X, voidPosition[randomVoid].Y] = mapItem;
                        voidPosition.Remove(voidPosition[randomVoid]);
                    }
                }
                else if (Config.TeleporterPositionType == TeleporterPositionTypeEnum.PlusForm)
                {
                    var teleporterPositions = new Point[]
                    {
                        new Point((int)Math.Ceiling((double)(Config.MapSize.X - 2)/(double)2), 1),
                        new Point(1, (int)Math.Ceiling((double)(Config.MapSize.Y - 2)/(double)2)),
                        new Point((int)Math.Ceiling((double)(Config.MapSize.X - 2)/(double)2), Config.MapSize.Y - 2),
                        new Point(Config.MapSize.X - 2, (int)Math.Ceiling((double)(Config.MapSize.Y - 2)/(double)2))
                    };

                    for (int i = 0; i < teleporterPositions.Length; i++)
                    {
                        mapItem = new Teleporter(GameRef, new Vector2(
                            teleporterPositions[i].X * Engine.TileWidth,
                            teleporterPositions[i].Y * Engine.TileHeight));
                        TeleporterList.Add((Teleporter)mapItem);
                        map[teleporterPositions[i].X, teleporterPositions[i].Y] = mapItem;
                    }
                }
            }
            */
            #endregion

            #region Arrow
            /*
            if (Config.ActiveArrows)
            {
                if (Config.ArrowPositionType == ArrowPositionTypeEnum.Randomly)
                {
                    var lookDirectionArray = new LookDirection[] { LookDirection.Up, LookDirection.Down, LookDirection.Left, LookDirection.Right };
                    for (int i = 0; i < MathHelper.Clamp(Config.ArrowNumber, 0, voidPosition.Count - 1); i++)
                    {
                        int randomVoid = Random.Next(voidPosition.Count);
                        mapItem = new Arrow(GameRef, new Vector2(
                            voidPosition[randomVoid].X * Engine.TileWidth,
                            voidPosition[randomVoid].Y * Engine.TileHeight),
                            lookDirectionArray[Random.Next(lookDirectionArray.Length)]);
                        ArrowList.Add((Arrow)mapItem);
                        map[voidPosition[randomVoid].X, voidPosition[randomVoid].Y] = mapItem;
                        voidPosition.Remove(voidPosition[randomVoid]);
                    }
                }
                else if (Config.ArrowPositionType == ArrowPositionTypeEnum.SquareForm)
                {
                    int outsideArrowsLag = 0;
                    var ratio = (int)Math.Ceiling((double)(4 * (Config.MapSize.X - 2)) / (double)5);
                    if (ratio % 2 == 0)
                        outsideArrowsLag = 1;

                    var arrowPositions = new Point[]
                    {
                        // ~~ Inside ~~ //
                        // Top left
                        new Point(
                            (int)Math.Ceiling((double)(Config.MapSize.X - 2)/(double)4) + 1, 
                            (int)Math.Ceiling((double)(Config.MapSize.Y - 2)/(double)4) + 1),

                        // Top right
                        new Point(
                            (int)Math.Ceiling((double)(3 * (Config.MapSize.X - 2))/(double)4) - 1, 
                            (int)Math.Ceiling((double)(Config.MapSize.Y - 2)/(double)4) + 1),

                        // Bottom left
                        new Point(
                            (int)Math.Ceiling((double)(Config.MapSize.X - 2)/(double)4) + 1, 
                            (int)Math.Ceiling((double)(3 * (Config.MapSize.Y - 2))/(double)4) - 1),

                        // Bottom right
                        new Point(
                            (int)Math.Ceiling((double)(3 * (Config.MapSize.X - 2))/(double)4) - 1, 
                            (int)Math.Ceiling((double)(3 * (Config.MapSize.Y - 2))/(double)4) - 1),

                        // ~~ Outside ~~ //
                        // Top left
                        new Point(
                            (int)Math.Ceiling((double)(Config.MapSize.X - 2)/(double)5), 
                            (int)Math.Ceiling((double)(Config.MapSize.Y - 2)/(double)5)),

                        // Top right
                        new Point(
                            (int)Math.Ceiling((double)(4 * (Config.MapSize.X - 2))/(double)5) + outsideArrowsLag, 
                            (int)Math.Ceiling((double)(Config.MapSize.Y - 2)/(double)5)),

                        // Bottom left
                        new Point(
                            (int)Math.Ceiling((double)(Config.MapSize.X - 2)/(double)5), 
                            (int)Math.Ceiling((double)(4 * (Config.MapSize.Y - 2))/(double)5) + outsideArrowsLag),

                        // Bottom right
                        new Point(
                            (int)Math.Ceiling((double)(4 * (Config.MapSize.X - 2))/(double)5 + outsideArrowsLag), 
                            (int)Math.Ceiling((double)(4 * (Config.MapSize.Y - 2))/(double)5) + outsideArrowsLag)
                    };

                    for (int i = 0; i < arrowPositions.Length; i++)
                    {
                        mapItem = new Arrow(GameRef, new Vector2(
                            arrowPositions[i].X * Engine.TileWidth,
                            arrowPositions[i].Y * Engine.TileHeight),
                            Config.ArrowLookDirection[i % 4]);
                        ArrowList.Add((Arrow)mapItem);
                        map[arrowPositions[i].X, arrowPositions[i].Y] = mapItem;
                    }
                }
            }
            */
            #endregion

            int counter = 0;
            for (int x = 0; x < Config.MapSize.X; x++)
            {
                Entity mapItem;
                for (int y = 0; y < Config.MapSize.Y; y++)
                {
                    var tile = new Tile(0, 0);
                    if (x == 0 || y == 0 || x == (Config.MapSize.X - 1) || y == (Config.MapSize.Y - 1) ||
                        (x % 2 == 0 && y % 2 == 0))
                    {
                        // Inside wallList
                        if ((x % 2 == 0 && y % 2 == 0 && !(x == 0 || y == 0 || x == (Config.MapSize.X - 1) || y == (Config.MapSize.Y - 1))) && mapPlayersPosition[x, y] != 2)
                        {
                            mapItem = new UnbreakableWall(GameRef, new Vector2(x * Engine.TileHeight, y * Engine.TileWidth));
                            this.UnbreakableWallList.Add((UnbreakableWall)mapItem);
                            map[x, y] = mapItem;
                            collisionLayer[x, y] = true;
                        }
                        // Outside wallList
                        else if (mapPlayersPosition[x, y] != 2)
                        {
                            mapItem = new EdgeWall(GameRef, new Vector2(x * Engine.TileHeight, y * Engine.TileWidth));
                            _edgeWallList.Add((EdgeWall)mapItem);
                            counter++;
                            map[x, y] = mapItem;
                            collisionLayer[x, y] = true;
                        }
                    }
                    else
                    {
                        // Wall
                        if ((mapPlayersPosition[x, y] != 1 && mapPlayersPosition[x, y] != 2) && map[x, y] == null &&
                            Random.Next(0, 100) < MathHelper.Clamp(Config.WallNumber, 0, 100))
                        {
                            collisionLayer[x, y] = true;
                            mapItem = new Wall(GameRef, new Vector2(x * Engine.TileWidth, y * Engine.TileHeight));
                            _wallList.Add((Wall)mapItem);
                            map[x, y] = mapItem;
                        }
                        //tile = new Tile(0, 0);
                    }
                    layer.SetTile(x, y, tile);
                }
            }

            var mapLayers = new List<MapLayer> { layer };

            var tileMap = new TileMap(tilesets, mapLayers);
            var level = new Level(Config.MapSize, tileMap, map, collisionLayer);

            World = new World(GameRef, GameRef.ScreenRectangle);
            World.Levels.Add(level);
            World.CurrentLevel = 0;

            me = new OnlineHumanPlayer(0) { Name = "Me" };
            Players.Add(me);
            //map[playerPositions[playerID].X, playerPositions[playerID].Y] = me;

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
                    me.Id = playerId;
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
            if (Players.GetPlayerByID(playerID) == null)
            {
                var player = new OnlinePlayer(playerID)
                {
                    Name = "Online Player"
                };
                //player.MoveSpeed = moveSpeed;
                //Entities.Add(player);
                Players.Add(player);
            }
        }

        private void GameServer_MovePlayer(object sender, MovePlayerArgs arg)
        {
            Player player = Players.GetPlayerByID(arg.PlayerID);
            if (player != null)
            {
                Debug.Print(arg.Position.ToString());
                // TODO => Move Players on the map
                player.Sprite.Position = arg.Position;
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

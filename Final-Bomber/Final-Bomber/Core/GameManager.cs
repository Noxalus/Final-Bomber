using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FBLibrary.Core;
using Final_Bomber.Core.Entities;
using Final_Bomber.Core.Players;
using Final_Bomber.Entities;
using Final_Bomber.WorldEngine;
using Microsoft.Xna.Framework;

namespace Final_Bomber.Core
{
    class GameManager
    {
        // Game logic
        private Engine _engine;

        // Random
        private Random _random;

        // Players
        public OnlineHumanPlayer Me;
        public PlayerCollection Players;

        // Dead Players number
        int _deadPlayersNumber;

        // World
        public World World { get; set; }
        private Map _currentMap;
        private int[,] _hazardMap;

        // Dynamic entities
        private List<Wall> _wallList;
        private List<PowerUp> _powerUpList;
        private List<Bomb> _bombList;

        // Sudden Death
        public SuddenDeath SuddenDeath { get; private set; }

        public GameManager()
        {
            Players = new PlayerCollection();

            _random = new Random();

            _wallList = new List<Wall>();
            _powerUpList = new List<PowerUp>();
            _bombList = new List<Bomb>();

            // Engine
            _engine = new Engine(32, 32, Vector2.Zero);

            World = new World(FinalBomber.Instance, FinalBomber.Instance.ScreenRectangle);
        }

        public void LoadMap(string mapName)
        {
            _currentMap = new Map();
            _currentMap.Parse(mapName);

            _hazardMap = new int[_currentMap.Size.X, _currentMap.Size.Y];

            World.Levels.Add(_currentMap);
        }

        public void Update(GameTime gameTime)
        {
            foreach (Player p in Players)
            {
                p.Update(gameTime, _currentMap, _hazardMap);
            }
        }

        public void Draw(GameTime gameTime)
        {
            World.DrawLevel(gameTime, FinalBomber.Instance.SpriteBatch, null);

            foreach (var player in Players)
                player.Draw(gameTime);

            foreach (var wall in _wallList)
                wall.Draw(gameTime);

            foreach (var powerUp in _powerUpList)
                powerUp.Draw(gameTime);

            foreach (var bomb in _bombList)
                bomb.Draw(gameTime);
        }

        public void Reset()
        {
            //_timer = TimeSpan.Zero;
            _deadPlayersNumber = 0;

            CreateWorld();

            var origin = new Vector2(/*_hudOrigin.X / 2 -*/ ((32 * _currentMap.Size.X) / 2f),
                FinalBomber.Instance.GraphicsDevice.Viewport.Height / 2 - ((32 * _currentMap.Size.Y) / 2));

            Engine.Origin = origin;

            SuddenDeath = new SuddenDeath(FinalBomber.Instance, Config.PlayersPositions[0]);
        }

        private void CreateWorld()
        {
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

            Me = new OnlineHumanPlayer(0) { Name = "Me" };
            Players.Add(Me);
        }
    }
}

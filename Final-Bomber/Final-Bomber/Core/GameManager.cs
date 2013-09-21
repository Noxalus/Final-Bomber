﻿using System;
using System.Collections.Generic;
using FBLibrary.Core;
using FBLibrary.Core.BaseEntities;
using Final_Bomber.Core.Entities;
using Final_Bomber.Entities;
using Final_Bomber.TileEngine;
using Final_Bomber.WorldEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Final_Bomber.Core
{
    public class GameManager : BaseGameManager
    {
        // Game logic

        // Players
        public PlayerCollection Players;

        // Collections
        private readonly List<Bomb> _bombList;
        private readonly List<PowerUp> _powerUpList;
        private readonly List<Wall> _wallList;

        private SoundEffect _bombExplosionSound;

        // Dead Players number
        private Map _currentMap;
        private int _deadPlayersNumber;
        private int[,] _hazardMap;

        // Songs & sounds effect
        private Song _mapHurrySong;
        private Song _mapSong;
        private SoundEffect _itemPickUpSound;
        private SoundEffect _playerDeathSound;

        // Sudden Death
        private SuddenDeath _suddenDeath;

        #region Properties

        public World World { get; set; }

        public List<Wall> WallList
        {
            get { return _wallList; }
        }

        #endregion

        public GameManager()
        {
            Players = new PlayerCollection();

            _wallList = new List<Wall>();
            _powerUpList = new List<PowerUp>();
            _bombList = new List<Bomb>();

            World = new World(FinalBomber.Instance, FinalBomber.Instance.ScreenRectangle);
        }

        public void Initialize()
        {
        }

        public void LoadContent()
        {
            // Musics
            _mapSong = FinalBomber.Instance.Content.Load<Song>("Audio/Musics/map1");
            _mapHurrySong = FinalBomber.Instance.Content.Load<Song>("Audio/Musics/map1_hurry");

            // Sounds effects
            _bombExplosionSound = FinalBomber.Instance.Content.Load<SoundEffect>("Audio/Sounds/boom");
            _itemPickUpSound = FinalBomber.Instance.Content.Load<SoundEffect>("Audio/Sounds/item");
            _playerDeathSound = FinalBomber.Instance.Content.Load<SoundEffect>("Audio/Sounds/playerDeath");
        }

        public void LoadMap(string mapName)
        {
            _currentMap = new Map();
            _currentMap.Parse(mapName, this);

            _hazardMap = new int[_currentMap.Size.X, _currentMap.Size.Y];

            World.Levels.Add(_currentMap);
            World.CurrentLevel++;
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
            World.DrawLevel(gameTime, FinalBomber.Instance.SpriteBatch, new Camera(FinalBomber.Instance.ScreenRectangle));

            foreach (Wall wall in WallList)
                wall.Draw(gameTime);

            foreach (PowerUp powerUp in _powerUpList)
                powerUp.Draw(gameTime);

            foreach (Bomb bomb in _bombList)
                bomb.Draw(gameTime);

            foreach (Player player in Players)
                player.Draw(gameTime);
        }

        public void Reset()
        {
            //_timer = TimeSpan.Zero;
            _deadPlayersNumber = 0;

            CreateWorld();

            var origin = new Vector2( /*_hudOrigin.X / 2 -*/ ((32 * _currentMap.Size.X) / 2f),
                FinalBomber.Instance.GraphicsDevice.Viewport.Height / 2 - ((32 * _currentMap.Size.Y) / 2));

            Engine.Origin = origin;

            _suddenDeath = new SuddenDeath(FinalBomber.Instance, Config.PlayersPositions[0]);
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
        }
    }
}
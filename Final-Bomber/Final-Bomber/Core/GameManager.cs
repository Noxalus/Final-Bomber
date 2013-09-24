﻿using System;
using System.Collections.Generic;
using System.Linq;
using FBLibrary.Core;
using FBLibrary.Core.BaseEntities;
using Final_Bomber.Core.Entities;
using Final_Bomber.Entities;
using Final_Bomber.TileEngine;
using Final_Bomber.WorldEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Final_Bomber.Core
{
    public class GameManager : BaseGameManager
    {
        // Game logic

        // Players
        public PlayerCollection Players;
        private int _deadPlayersNumber;

        // Collections
        private readonly List<Bomb> _bombList;
        private readonly List<PowerUp> _powerUpList;
        private readonly List<Wall> _wallList;

        // Map
        private Map _currentMap;

        // Songs & sounds effect
        private Song _mapHurrySong;
        private Song _mapSong;
        public SoundEffect BombExplosionSound;
        public SoundEffect PowerUpPickUpSound;
        public SoundEffect PlayerDeathSound;

        // Timer
        TimeSpan _timer;

        private GameTime _gameTime;

        // Sudden Death
        private SuddenDeath _suddenDeath;

        #region Properties

        public Map CurrentMap
        {
            get { return _currentMap; }
        }

        public List<Wall> WallList
        {
            get { return _wallList; }
        }

        public List<Bomb> BombList
        {
            get { return _bombList; }
        }

        public TimeSpan Timer
        {
            get { return _timer; }
        }

        #endregion

        public GameManager()
        {
            Players = new PlayerCollection();

            _wallList = new List<Wall>();
            _powerUpList = new List<PowerUp>();
            _bombList = new List<Bomb>();

            _currentMap = new Map();
            BaseCurrentMap = _currentMap;

            _timer = TimeSpan.Zero;

            _gameTime = new GameTime();
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
            BombExplosionSound = FinalBomber.Instance.Content.Load<SoundEffect>("Audio/Sounds/boom");
            PowerUpPickUpSound = FinalBomber.Instance.Content.Load<SoundEffect>("Audio/Sounds/item");
            PlayerDeathSound = FinalBomber.Instance.Content.Load<SoundEffect>("Audio/Sounds/playerDeath");

            CurrentMap.LoadContent();
        }

        public void Update(GameTime gameTime)
        {
            _timer += gameTime.ElapsedGameTime;

            _gameTime = gameTime;

            base.Update();
        }

        protected override void UpdateWalls()
        {
            for (int i = 0; i < _wallList.Count; i++)
            {
                _wallList[i].Update(_gameTime);

                // We clean the obsolete elements
                if (!_wallList[i].IsAlive)
                {
                    RemoveWall(_wallList[i]);
                }
            }

            base.UpdateWalls();
        }

        protected override void UpdateBombs()
        {
            for (int i = 0; i < _bombList.Count; i++)
            {
                _bombList[i].Update(_gameTime, CurrentMap, HazardMap);

                // Do we delete the bomb
                if (!_bombList[i].IsAlive)
                {
                    RemoveBomb(_bombList[i]);
                }
            }

            base.UpdateBombs();
        }

        protected override void UpdatePowerUps()
        {
            for (int i = 0; i < _powerUpList.Count; i++)
            {
                _powerUpList[i].Update(_gameTime);

                if (!_powerUpList[i].IsAlive)
                {
                    RemovePowerUp(_powerUpList[i]);
                }
            }

            base.UpdatePowerUps();
        }

        protected override void UpdatePlayers()
        {
            for (int i = 0; i < Players.Count; i++)
            {
                if (Players[i].IsAlive)
                {
                    Players[i].Update(_gameTime, CurrentMap, HazardMap);
                }
            }

            base.UpdatePlayers();
        }

        public void Draw(GameTime gameTime)
        {
            CurrentMap.Draw(gameTime, FinalBomber.Instance.SpriteBatch, new Camera(FinalBomber.Instance.ScreenRectangle));

            foreach (Wall wall in WallList)
                wall.Draw(gameTime);

            foreach (PowerUp powerUp in _powerUpList)
                powerUp.Draw(gameTime);

            foreach (Bomb bomb in _bombList)
                bomb.Draw(gameTime);

            foreach (Player player in Players)
            {
                if (player.IsAlive)
                    player.Draw(gameTime);
            }
        }

        public void Reset()
        {
            // Song
            MediaPlayer.Play(_mapSong);

            //_timer = TimeSpan.Zero;
            _deadPlayersNumber = 0;

            CreateWorld();

            var origin = new Vector2((FinalBomber.Instance.GraphicsDevice.Viewport.Width - 234) / 2f - ((32 * _currentMap.Size.X) / 2f),
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
                    Players.Add(player);
                    board[playerPositions[playerID].X, playerPositions[playerID].Y] = player;
                }
                else
                {
                    var player = new HumanPlayer(Math.Abs(playerID));
                    Players.Add(player);
                    board[playerPositions[playerID].X, playerPositions[playerID].Y] = player;
                }
            }
            */
        }

        public void AddPlayer(Player player)
        {
            Players.Add(player);

            base.AddPlayer(player);
        }

        public void RemovePlayer(Player player)
        {
            Players.Remove(player);

            base.RemovePlayer(player);
        }

        public void AddWalls(IEnumerable<Point> wallPositions)
        {
            foreach (var position in wallPositions)
            {
                AddWall(position);
            }
        }

        #region Wall methods

        public override void AddWall(Point position)
        {
            var wall = new Wall(position);
            _wallList.Add(wall);

            base.AddWall(wall);
        }

        public void AddWall(Wall wall)
        {
            _wallList.Add(wall);

            base.AddWall(wall);
        }

        protected override void DestroyWall(Point position)
        {
            var wall = _wallList.Find(w => w.CellPosition == position);

            base.DestroyWall(wall);
        }

        private void RemoveWall(Wall wall)
        {
            _wallList.Remove(wall);

            base.RemoveWall(wall);
        }

        #endregion

        #region Bomb methods

        public void AddBomb(Bomb bomb)
        {
            _bombList.Add(bomb);

            base.AddBomb(bomb);
        }

        private void RemoveBomb(Bomb bomb)
        {
            _bombList.Remove(bomb);

            // We don't forget to give it back to its owner
            if (bomb.PlayerId >= 0)
            {
                BasePlayer player = BasePlayerList.Find(p => p.Id == bomb.PlayerId);

                if (player != null && player.CurrentBombAmount < player.TotalBombAmount)
                    player.CurrentBombAmount++;
            }

            base.RemoveBomb(bomb);
        }

        #endregion

        #region Power up methods

        public override void AddPowerUp(Point position)
        {
            throw new NotImplementedException();
        }

        protected override void PickUpPowerUp(BasePlayer player, BasePowerUp powerUp)
        {
            powerUp.ApplyEffect(player);
            powerUp.PickUp();

            powerUp.Remove();
        }

        protected override void DestroyPowerUp(Point position)
        {
            var powerUp = _powerUpList.Find(pu => pu.CellPosition == position);

            base.DestroyPowerUp(powerUp);
        }

        private void RemovePowerUp(PowerUp powerUp)
        {
            _powerUpList.Remove(powerUp);

            base.RemovePowerUp(powerUp);
        }

        #endregion

        public void AddPowerUp(PowerUpType type, Point position)
        {
            var powerUp = new PowerUp(position, type);
            _powerUpList.Add(powerUp);

            base.AddPowerUp(powerUp);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FBClient.Core.Entities;
using FBClient.Entities;
using FBClient.WorldEngine;
using FBLibrary;
using FBLibrary.Core;
using FBLibrary.Core.BaseEntities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace FBClient.Core
{
    /// <summary>
    /// This class is used to regroup common behaviors between network and local game logic.
    /// </summary>
    public abstract class GameManager : BaseGameManager
    {
        // Game logic

        // Players
        public readonly PlayerCollection Players;

        // Collections
        public readonly List<Bomb> BombList;
        protected readonly List<PowerUp> PowerUpList;
        protected readonly List<Wall> WallList;

        // Map
        private readonly Map _currentMap;

        // Songs & sounds effect
        private Song _mapHurrySong;
        private Song _mapSong;

        // Timer
        private TimeSpan _gameStopWatch;
        private GameTime _gameTime;

        // Sudden Death
        private SuddenDeath _suddenDeath;

        // Camera
        public Camera2D Camera;

        #region Properties

        public Map CurrentMap
        {
            get { return _currentMap; }
        }

        public TimeSpan GameTimer
        {
            get { return _gameStopWatch; }
        }

        #endregion

        protected GameManager()
        {
            Players = new PlayerCollection();

            WallList = new List<Wall>();
            PowerUpList = new List<PowerUp>();
            BombList = new List<Bomb>();

            _currentMap = new Map();
            BaseCurrentMap = _currentMap;

            _gameStopWatch = TimeSpan.Zero;

            _gameTime = new GameTime();

        }

        public virtual void Initialize()
        {
            _gameStopWatch = TimeSpan.Zero;

            _suddenDeath = new SuddenDeath(FinalBomber.Instance, Config.PlayersPositions[0]);

            GameEventManager = new GameEventManager(this);

            GameEventManager.Initialize();

            Camera = new Camera2D(FinalBomber.Instance.Graphics.GraphicsDevice.Viewport, CurrentMap.Size, 1f);
        }

        public virtual void Dispose()
        {
            GameEventManager.Dispose();
        }

        public override void Reset()
        {
            MediaPlayer.Play(_mapSong);

            _gameStopWatch = TimeSpan.Zero;

            Players.ForEach(player => player.Reset());

            WallList.Clear();
            PowerUpList.Clear();
            BombList.Clear();

            base.Reset();
        }

        public virtual void LoadContent()
        {
            // Musics
            _mapSong = FinalBomber.Instance.Content.Load<Song>("Audio/Musics/map2");
            _mapHurrySong = FinalBomber.Instance.Content.Load<Song>("Audio/Musics/map1_hurry");

            MediaPlayer.Play(_mapSong);

            CurrentMap.LoadContent();

            // Load players content
            Players.ForEach(player => player.LoadContent());
        }

        public override void Update()
        {
            _gameStopWatch += TimeSpan.FromTicks(GameConfiguration.DeltaTime);

            var gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromTicks(GameConfiguration.DeltaTime));
            _gameTime = gameTime;

            // Camera position
            var cameraPosition = Vector2.Zero;
            var firstPlayer = Players.Find(p => p.Id == 0);
            if (firstPlayer != null)
                cameraPosition = firstPlayer.Position;

            Camera.Update(cameraPosition);

            base.Update();
        }

        #region Update entities
        protected override void UpdateWalls()
        {
            for (int i = 0; i < WallList.Count; i++)
            {
                WallList[i].Update(_gameTime);

                // We clean the obsolete elements
                if (!WallList[i].IsAlive)
                {
                    RemoveWall(WallList[i]);
                }
            }

            base.UpdateWalls();
        }

        protected override void UpdateBombs()
        {
            for (int i = 0; i < BombList.Count; i++)
            {
                BombList[i].Update(_gameTime);

                // Do we delete the bomb
                if (!BombList[i].IsAlive)
                {
                    RemoveBomb(BombList[i]);
                }
            }

            base.UpdateBombs();
        }

        protected override void UpdatePowerUps()
        {
            for (int i = 0; i < PowerUpList.Count; i++)
            {
                PowerUpList[i].Update(_gameTime);

                if (!PowerUpList[i].IsAlive)
                {
                    RemovePowerUp(PowerUpList[i]);
                }
            }

            base.UpdatePowerUps();
        }

        protected override void UpdatePlayers()
        {
            var alivePlayers = Players.FindAll(p => p.IsAlive);
            foreach (var player in alivePlayers)
            {
                player.Update(_gameTime, CurrentMap, HazardMap);
            }

            base.UpdatePlayers();
        }
        #endregion

        public void Draw(GameTime gameTime, Camera2D camera)
        {
            CurrentMap.Draw(gameTime, FinalBomber.Instance.SpriteBatch, camera);

            foreach (Wall wall in WallList)
                wall.Draw(gameTime);

            foreach (PowerUp powerUp in PowerUpList)
                powerUp.Draw(gameTime);

            foreach (Bomb bomb in BombList)
                bomb.Draw(gameTime);

            Players.Sort(new PlayerOverlappingSort());
            foreach (Player player in Players)
            {
                if (player.IsAlive)
                    player.Draw(gameTime);
            }
        }

        #region Entity methods

        #region Player methods

        public virtual void AddPlayer(Player player)
        {
            Players.Add(player);

            base.AddPlayer(player);
        }

        protected override void DestroyPlayer(int playerId)
        {
            var player = Players.Find(p => p.Id == playerId);

            base.DestroyPlayer(player);
        }

        /*
        public void RemovePlayer(Player player)
        {
            Players.Remove(player);

            base.RemovePlayer(player);
        }
        */

        #endregion

        #region Wall methods

        public override void AddWall(Point position)
        {
            var wall = new Wall(position);
            WallList.Add(wall);

            base.AddWall(wall);
        }

        public void AddWalls(IEnumerable<Point> wallPositions)
        {
            foreach (var position in wallPositions)
            {
                AddWall(position);
            }
        }

        protected override void DestroyWall(Point position)
        {
            var wall = WallList.Find(w => w.CellPosition == position);

            base.DestroyWall(wall);
        }

        protected virtual void RemoveWall(Wall wall)
        {
            WallList.Remove(wall);

            base.RemoveWall(wall);
        }

        #endregion

        #region Bomb methods

        public void AddBomb(Bomb bomb)
        {
            BombList.Add(bomb);

            base.AddBomb(bomb);
        }

        protected override void DestroyBomb(Point position)
        {
            var bomb = BombList.Find(b => b.CellPosition == position);

            base.DestroyBomb(bomb);
        }

        private void RemoveBomb(Bomb bomb)
        {
            BombList.Remove(bomb);

            base.RemoveBomb(bomb);
        }

        #endregion

        #region Power up methods

        public override void AddPowerUp(Point position)
        {
            var powerUp = new PowerUp(position);
            PowerUpList.Add(powerUp);

            base.AddPowerUp(powerUp);
        }

        public void AddPowerUp(PowerUpType type, Point position)
        {
            var powerUp = new PowerUp(position, type);
            PowerUpList.Add(powerUp);

            base.AddPowerUp(powerUp);
        }

        protected override void PickUpPowerUp(BasePlayer player, BasePowerUp powerUp)
        {
            powerUp.ApplyEffect(player);
            powerUp.PickUp();

            powerUp.Remove();
        }

        protected override void DestroyPowerUp(Point position)
        {
            var powerUp = PowerUpList.Find(pu => pu.CellPosition == position);

            base.DestroyPowerUp(powerUp);
        }

        private void RemovePowerUp(PowerUp powerUp)
        {
            PowerUpList.Remove(powerUp);

            base.RemovePowerUp(powerUp);
        }

        #endregion

        #endregion

        #region Events

        public virtual void PlayerDeathAction(BasePlayer sender, EventArgs args)
        {
            //sender.Remove();
        }

        public virtual void RoundEndAction()
        {
            Reset();
        }


        #endregion
    }
}
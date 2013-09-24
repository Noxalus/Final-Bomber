using System;
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

            UpdateWalls(gameTime);

            UpdateBombs(gameTime);

            #region Power Up

            for (int i = 0; i < _powerUpList.Count; i++)
            {
                _powerUpList[i].Update(gameTime);

                // Is it die ?
                if (HazardMap[_powerUpList[i].CellPosition.X, _powerUpList[i].CellPosition.Y] == 3)
                    _powerUpList[i].Destroy();

                // We clean the obsolete elements
                if (!_powerUpList[i].IsAlive)
                {
                    if (CurrentMap.Board[_powerUpList[i].CellPosition.X, _powerUpList[i].CellPosition.Y] is PowerUp)
                        CurrentMap.Board[_powerUpList[i].CellPosition.X, _powerUpList[i].CellPosition.Y] = null;

                    _powerUpList.Remove(_powerUpList[i]);
                }
            }

            #endregion

            #region Player

            for (int i = 0; i < Players.Count; i++)
            {
                // We clean the obsolete players
                if (!Players[i].IsAlive)
                {
                    if (!Players[i].OnEdge)
                    {
                        if (CurrentMap.Board[Players[i].CellPosition.X, Players[i].CellPosition.Y] is Player)
                        {
                            var p = (Player)CurrentMap.Board[Players[i].CellPosition.X, Players[i].CellPosition.Y];
                            if (p.Id == Players[i].Id)
                                CurrentMap.Board[Players[i].CellPosition.X, Players[i].CellPosition.Y] = null;
                        }

                        _deadPlayersNumber++;
                    }

                    if (true /*(Config.ActiveSuddenDeath && SuddenDeath.HasStarted)*/)
                    {
                        //Players.Remove(Players[i]);
                    }
                    else
                    {
                        Players[i].OnEdge = true;
                    }
                }
                else
                {

                    Players[i].Update(gameTime, CurrentMap, HazardMap);

                    // Pick up a power up ?
                    var powerUp = CurrentMap.Board[Players[i].CellPosition.X, Players[i].CellPosition.Y] as PowerUp;
                    if (powerUp != null)
                    {
                        if (!powerUp.InDestruction)
                        {
                            if (!Players[i].HasBadEffect ||
                                (Players[i].HasBadEffect && powerUp.Type != PowerUpType.BadEffect))
                            {
                                powerUp.ApplyEffect(Players[i]);
                                PowerUpPickUpSound.Play();
                                powerUp.Remove();
                            }
                        }
                    }

                    // Is it die ?
                    if (!Players[i].InDestruction && !Players[i].IsInvincible &&
                        HazardMap[Players[i].CellPosition.X, Players[i].CellPosition.Y] == 3)
                    {
                        // Bomb
                        int bombId = -42;
                        List<Bomb> bl = BombList.FindAll(b => b.InDestruction);
                        foreach (Bomb b in bl)
                        {
                            if (b.ActionField.Any(po => po == Players[i].CellPosition))
                            {
                                bombId = b.PlayerId;
                            }
                        }

                        // Suicide
                        if (bombId == Players[i].Id)
                            Players[i].Stats.Suicides++;

                        else if (bombId >= 0 && bombId < Config.PlayersNumber)
                        {
                            Players[i].Stats.Kills++;
                            Player player = Players.Find(p => p.Id == bombId);
                            if (player.OnEdge)
                            {
                                player.Rebirth(Players[i].Position);
                                _deadPlayersNumber--;
                            }
                        }
                        Players[i].Destroy();
                    }
                }
            }

            #endregion

            base.Update();
        }

        private void UpdateWalls(GameTime gameTime)
        {
            for (int i = 0; i < _wallList.Count; i++)
            {
                _wallList[i].Update(gameTime);

                // We clean the obsolete elements
                if (!_wallList[i].IsAlive)
                {
                    RemoveWall(_wallList[i]);
                }
            }
        }

        private void UpdateBombs(GameTime gameTime)
        {
            for (int i = 0; i < _bombList.Count; i++)
            {
                _bombList[i].Update(gameTime, CurrentMap, HazardMap);

                // Do we delete the bomb
                if (!_bombList[i].IsAlive)
                {
                    RemoveBomb(_bombList[i]);
                }
            }
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

        public void AddPowerUp(PowerUpType type, Point position)
        {
            var powerUp = new PowerUp(position, type);
            CurrentMap.Board[position.X, position.Y] = powerUp;
            _powerUpList.Add(powerUp);
        }
    }
}
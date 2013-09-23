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
        public SoundEffect ItemPickUpSound;
        public SoundEffect PlayerDeathSound;

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

        #endregion

        public GameManager()
        {
            Players = new PlayerCollection();

            _wallList = new List<Wall>();
            _powerUpList = new List<PowerUp>();
            _bombList = new List<Bomb>();

            _currentMap = new Map();
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
            ItemPickUpSound = FinalBomber.Instance.Content.Load<SoundEffect>("Audio/Sounds/item");
            PlayerDeathSound = FinalBomber.Instance.Content.Load<SoundEffect>("Audio/Sounds/playerDeath");

            CurrentMap.LoadContent();
        }

        public void ParseMap(string mapName)
        {
            _currentMap.Parse(mapName, this);
            HazardMap = new int[_currentMap.Size.X, _currentMap.Size.Y];
        }

        public void Update(GameTime gameTime)
        {
            #region Walls

            for (int i = 0; i < _wallList.Count; i++)
            {
                _wallList[i].Update(gameTime);

                // Is it die ?
                if (HazardMap[_wallList[i].CellPosition.X, _wallList[i].CellPosition.Y] == 3)
                {
                    HazardMap[_wallList[i].CellPosition.X, _wallList[i].CellPosition.Y] = 0;
                    _wallList[i].Destroy();
                }

                // We clean the obsolete elements
                if (!_wallList[i].IsAlive)
                {
                    CurrentMap.CollisionLayer[_wallList[i].CellPosition.X, _wallList[i].CellPosition.Y] = false;
                    /*
                    if (Random.Next(0, 100) < MathHelper.Clamp(Config.ItemNumber, 0, 100))
                    {
                        var powerUp = new PowerUp(_wallList[i].CellPosition);
                        _powerUpList.Add(powerUp);
                        CurrentMap.Board[_wallList[i].CellPosition.X, _wallList[i].CellPosition.Y] = powerUp;
                    }
                    else
                    {*/
                        CurrentMap.Board[_wallList[i].CellPosition.X, _wallList[i].CellPosition.Y] = null;
                    //}

                    _wallList.Remove(_wallList[i]);
                }
            }

            #endregion

            #region Bombs
            for (int i = 0; i < BombList.Count; i++)
            {
                BombList[i].Update(gameTime, CurrentMap, HazardMap);

                // Is it die ?
                /*
                if (HazardMap[BombList[i].CellPosition.X, BombList[i].CellPosition.Y] == 3 &&
                    !BombList[i].InDestruction)
                {
                    BombList[i].Destroy();
                }
                */

                // We clean the obsolete elements
                if (!BombList[i].IsAlive)
                {
                    if (CurrentMap.Board[BombList[i].CellPosition.X, BombList[i].CellPosition.Y] is Bomb)
                        CurrentMap.Board[BombList[i].CellPosition.X, BombList[i].CellPosition.Y] = null;

                    CurrentMap.CollisionLayer[BombList[i].CellPosition.X, BombList[i].CellPosition.Y] = false;

                    // We don't forget to give it back to its owner
                    if (BombList[i].PlayerId >= 0)
                    {
                        Player pl = Players.Find(p => p.Id == BombList[i].PlayerId);
                        if (pl != null && pl.CurrentBombAmount < pl.TotalBombAmount)
                            pl.CurrentBombAmount++;
                    }

                    // Update the hazard map
                    List<Bomb> bL = BombList.FindAll(b => !b.InDestruction);
                    foreach (Point p in BombList[i].ActionField)
                    {
                        bool sameCellThanAnOther = false;
                        if (BombList.Where(b => !(b.CellPosition.X == BombList[i].CellPosition.X &&
                                                  b.CellPosition.Y == BombList[i].CellPosition.Y)).Any(b => b.ActionField.Find(c => c.X == p.X && c.Y == p.Y) != Point.Zero))
                        {
                            HazardMap[p.X, p.Y] = 2;
                            sameCellThanAnOther = true;
                        }
                        if (!sameCellThanAnOther)
                            HazardMap[p.X, p.Y] = 0;
                    }

                    BombList.Remove(BombList[i]);
                }
            }
            #endregion

            #region Player

            for (int i = 0; i < Players.Count; i++)
            {
                Players[i].Update(gameTime, CurrentMap, HazardMap);

                // Is it die ?
                if (!Players[i].InDestruction && !Players[i].IsInvincible && HazardMap[Players[i].CellPosition.X, Players[i].CellPosition.Y] == 3)
                {
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
                        Players.Remove(Players[i]);
                    else
                        Players[i].OnEdge = true;
                }
            }

            #endregion
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
                player.Draw(gameTime);
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

        public void AddWalls(IEnumerable<Point> wallPositions)
        {
            foreach (var position in wallPositions)
            {
                var wall = new Wall(position);
                _wallList.Add(wall);
                CurrentMap.Board[position.X, position.Y] = wall;
                CurrentMap.CollisionLayer[position.X, position.Y] = true;
            }
        }

        public void AddBomb(Bomb bomb)
        {
            CurrentMap.Board[bomb.CellPosition.X, bomb.CellPosition.Y] = bomb;
            CurrentMap.CollisionLayer[bomb.CellPosition.X, bomb.CellPosition.Y] = true;

            BombList.Add(bomb);
        }

        public void AddPowerUp(PowerUpType type, Point position)
        {
            var powerUp = new PowerUp(position, type);
            _powerUpList.Add(powerUp);
            CurrentMap.Board[position.X, position.Y] = powerUp;
        }
    }
}
using System;
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
        private int _deadPlayersNumber;

        // Collections
        private readonly List<Bomb> _bombList;
        private readonly List<PowerUp> _powerUpList;
        private readonly List<Wall> _wallList;

        private SoundEffect _bombExplosionSound;

        // Map
        private Map _currentMap;
        private int[,] _hazardMap;

        // Songs & sounds effect
        private Song _mapHurrySong;
        private Song _mapSong;
        private SoundEffect _itemPickUpSound;
        private SoundEffect _playerDeathSound;

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
            _bombExplosionSound = FinalBomber.Instance.Content.Load<SoundEffect>("Audio/Sounds/boom");
            _itemPickUpSound = FinalBomber.Instance.Content.Load<SoundEffect>("Audio/Sounds/item");
            _playerDeathSound = FinalBomber.Instance.Content.Load<SoundEffect>("Audio/Sounds/playerDeath");


            CurrentMap.LoadContent();
        }

        public void ParseMap(string mapName)
        {
            _currentMap.Parse(mapName, this);
            _hazardMap = new int[_currentMap.Size.X, _currentMap.Size.Y];
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
            if (CurrentMap.Board[bomb.CellPositionX, bomb.CellPositionY] is Player)
            {
                CurrentMap.Board[bomb.CellPosition.X, bomb.CellPosition.Y] = bomb;
                CurrentMap.CollisionLayer[bomb.CellPosition.X, bomb.CellPosition.Y] = true;
            }

            BombList.Add(bomb);
        }
    }
}
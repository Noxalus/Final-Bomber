using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FBLibrary;
using FBLibrary.Core;
using FBLibrary.Core.BaseEntities;
using Final_BomberServer.Core.Entities;
using Final_BomberServer.Core.WorldEngine;
using Final_BomberServer.Host;
using Microsoft.Xna.Framework;

namespace Final_BomberServer.Core
{
    public class GameManager : BaseGameManager
    {
        // Collections
        private readonly List<Bomb> _bombList;
        private readonly List<PowerUp> _powerUpList;
        private readonly List<Wall> _wallList;

        // Map
        private Map _currentMap;

        #region Properties

        public BaseMap CurrentMap
        {
            get { return BaseCurrentMap; }
        }

        public List<Wall> WallList
        {
            get { return _wallList; }
        }

        public List<Bomb> BombList
        {
            get { return _bombList; }
        }

        public List<PowerUp> PowerUpList
        {
            get { return _powerUpList; }
        }

        #endregion

        public GameManager()
        {
            _wallList = new List<Wall>();
            _powerUpList = new List<PowerUp>();
            _bombList = new List<Bomb>();

            _currentMap = new Map();
            BaseCurrentMap = _currentMap;
        }

        public override void LoadMap(string mapName)
        {
            base.LoadMap(mapName);

            // We generate wall
            GenerateWalls();
        }

        private void GenerateWalls()
        {
            for (int x = 0; x < CurrentMap.Size.X; x++)
            {
                for (int y = 0; y < CurrentMap.Size.Y; y++)
                {
                    if (CurrentMap.Board[x, y] == null && GameConfiguration.Random.Next(0, 100) < ServerSettings.WallPercentage)
                    {
                        if (!NearPlayer(x, y))
                        {
                            var wall = new Wall(new Point(x, y));
                            WallList.Add(wall);
                            CurrentMap.Board[x, y] = wall;
                            CurrentMap.CollisionLayer[x, y] = true;
                        }
                    }
                }
            }
        }

        private bool NearPlayer(int x, int y)
        {
            return
                CurrentMap.PlayerSpawnPoints.Contains(new Point(x, y)) ||
                CurrentMap.PlayerSpawnPoints.Contains(new Point(x, y - 1)) ||
                CurrentMap.PlayerSpawnPoints.Contains(new Point(x, y + 1)) ||
                CurrentMap.PlayerSpawnPoints.Contains(new Point(x - 1, y)) ||
                CurrentMap.PlayerSpawnPoints.Contains(new Point(x + 1, y)) ||
                CurrentMap.PlayerSpawnPoints.Contains(new Point(x + 1, y - 1)) ||
                CurrentMap.PlayerSpawnPoints.Contains(new Point(x - 1, y - 1)) ||
                CurrentMap.PlayerSpawnPoints.Contains(new Point(x + 1, y + 1)) ||
                CurrentMap.PlayerSpawnPoints.Contains(new Point(x - 1, y + 1));
        }

        public override void AddWall(Point position)
        {
            var wall = new Wall(position);
            _wallList.Add(wall);

            base.AddWall(wall);
        }

        public void AddPlayer(Client client, Player player)
        {
            client.Player = player;

            base.AddPlayer(player);
        }

        public void RemovePlayer(Client client, Player player)
        {
            client.Player = null;

            base.RemovePlayer(player);
        }

        public void AddBomb(Bomb bomb)
        {
            _bombList.Add(bomb);

            base.AddBomb(bomb);
        }

        public void AddPowerUp(Point position)
        {
            var powerUp = new PowerUp(position);
            _powerUpList.Add(powerUp);
            _currentMap.Board[position.X, position.Y] = powerUp;

            GameSettings.gameServer.SendPowerUpDrop(powerUp);
        }

        #region Displaying region

        public void DisplayHazardMap()
        {

            for (int y = 0; y < CurrentMap.Size.Y; y++)
            {
                for (int x = 0; x < CurrentMap.Size.X; x++)
                {
                    Console.Write("{0} ", HazardMap[x, y]);
                }

                Console.WriteLine();
            }
        }


        #endregion
    }
}

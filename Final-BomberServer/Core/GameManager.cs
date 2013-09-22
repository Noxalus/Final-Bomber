using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FBLibrary.Core;
using Final_BomberServer.Core.Entities;
using Final_BomberServer.Core.WorldEngine;
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
        private int[,] _hazardMap;

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
            _wallList = new List<Wall>();
            _powerUpList = new List<PowerUp>();
            _bombList = new List<Bomb>();
        }

        public void LoadMap(string mapName)
        {
            _currentMap = new Map();
            _currentMap.Parse(mapName, this);

            // We affect a position to all players
            foreach (var player in GameSettings.gameServer.Clients.GetAlivePlayers())
            {
                player.ChangePosition(CurrentMap.PlayerSpawnPoints[player.Id]);
            }

            
            // We generate wall
            GenerateWalls();

            _hazardMap = new int[_currentMap.Size.X, _currentMap.Size.Y];
        }

        private void GenerateWalls()
        {
            for (int x = 0; x < CurrentMap.Size.X; x++)
            {
                for (int y = 0; y < CurrentMap.Size.Y; y++)
                {
                    if (CurrentMap.Board[x, y] == null && Random.Next(0, 100) < ServerSettings.WallPercentage)
                    {
                        var wall = new Wall(new Point(x ,y));
                        WallList.Add(wall);
                        CurrentMap.Board[x, y] = wall;
                        CurrentMap.CollisionLayer[x, y] = true;
                    }
                }
            }
        }
    }
}

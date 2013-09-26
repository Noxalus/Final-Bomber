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

        public override void Update()
        {
            base.Update();
        }

        protected override void UpdateWalls()
        {
            for (int i = 0; i < _wallList.Count; i++)
            {
                // We clean the obsolete elements
                if (!_wallList[i].IsAlive)
                {
                    if (GameConfiguration.Random.Next(0, 100) < MathHelper.Clamp(GameConfiguration.PowerUpPercentage, 0, 100))
                    {
                        AddPowerUp(_wallList[i].CellPosition);
                    }

                    RemoveWall(_wallList[i]);
                }
            }

            base.UpdateWalls();
        }

        protected override void UpdateBombs()
        {
            for (int i = 0; i < _bombList.Count; i++)
            {
                _bombList[i].Update();

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
                if (!_powerUpList[i].IsAlive)
                {
                    RemovePowerUp(_powerUpList[i]);
                }
            }

            base.UpdatePowerUps();
        }

        protected override void UpdatePlayers()
        {
            var players = GameSettings.GameServer.Clients.GetPlayers();

            for (int i = 0; i < players.Count; i++)
            {
                if (!players[i].IsAlive)
                {
                    if (!BasePlayerList[i].OnEdge)
                    {
                        if (BaseCurrentMap.Board[BasePlayerList[i].CellPosition.X, BasePlayerList[i].CellPosition.Y] is BasePlayer)
                        {
                            RemovePlayer(BasePlayerList[i]);
                        }
                    }
                }

            }

            base.UpdatePlayers();
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
                            AddWall(new Point(x, y));
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

        #region Player methods

        public void AddPlayer(Client client, Player player)
        {
            client.Player = player;

            base.AddPlayer(player);
        }

        protected override void DestroyPlayer(int playerId)
        {
            var player = GameSettings.GameServer.Clients.GetPlayerFromId(playerId);

            base.DestroyPlayer(player);
        }

        public void RemovePlayer(Client client, Player player)
        {
            var p = (BasePlayer)BaseCurrentMap.Board[player.CellPosition.X, player.CellPosition.Y];
            if (p.Id == player.Id)
                BaseCurrentMap.Board[player.CellPosition.X, player.CellPosition.Y] = null;

            client.Player = null;

            base.RemovePlayer(player);
        }

        #endregion

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

        protected override void DestroyBomb(Point position)
        {
            var bomb = _bombList.Find(b => b.CellPosition == position);

            base.DestroyBomb(bomb);
        }

        private void RemoveBomb(Bomb bomb)
        {
            _bombList.Remove(bomb);

            base.RemoveBomb(bomb);
        }

        #endregion

        #region Power up methods

        public override void AddPowerUp(Point position)
        {
            var powerUp = new PowerUp(position);
            _powerUpList.Add(powerUp);

            GameSettings.GameServer.SendPowerUpDrop(powerUp);

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
            var powerUp = _powerUpList.Find(pu => pu.CellPosition == position);

            base.DestroyPowerUp(powerUp);
        }

        private void RemovePowerUp(PowerUp powerUp)
        {
            _powerUpList.Remove(powerUp);

            base.RemovePowerUp(powerUp);
        }

        #endregion
    }
}

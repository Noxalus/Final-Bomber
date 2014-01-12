using System.Collections.Generic;
using FBLibrary;
using FBLibrary.Core;
using FBLibrary.Core.BaseEntities;
using FBServer.Core.Entities;
using FBServer.Core.WorldEngine;
using FBServer.Host;
using Microsoft.Xna.Framework;

namespace FBServer.Core
{
    public class GameManager : BaseGameManager
    {
        // Collections
        private readonly List<Player> _playerList;
        private readonly List<Bomb> _bombList;
        private readonly List<PowerUp> _powerUpList;
        private readonly List<Wall> _wallList;

        #region Properties

        public BaseMap CurrentMap
        {
            get { return BaseCurrentMap; }
        }

        public List<Player> PlayerList
        {
            get { return _playerList; }
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
            _playerList = new List<Player>();
            _wallList = new List<Wall>();
            _powerUpList = new List<PowerUp>();
            _bombList = new List<Bomb>();

            BaseCurrentMap = new Map();

            GameEventManager = new GameEventManager(this);
        }

        public override void Reset()
        {
            base.Reset();

            _wallList.Clear();
            _powerUpList.Clear();
            _bombList.Clear();
            _playerList.ForEach(player => player.Reset());
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
            var alivePlayers = _playerList.FindAll(p => p.IsAlive);
            foreach (var player in alivePlayers)
            {
                player.Update();
            }

            /*
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
            */

            base.UpdatePlayers();
        }

        public override void LoadMap(string mapName)
        {
            base.LoadMap(mapName);

            // We generate wall
            GenerateRandomWalls(ServerSettings.WallPercentage);

            CurrentMap.DisplayBoard();
        }

        #region Player methods

        public void AddPlayer(Client client, Player player)
        {
            _playerList.Add(player);

            player.Name = client.Username;
            client.Player = player;

            base.AddPlayer(player);
        }

        protected override void DestroyPlayer(int playerId)
        {
            var player = _playerList.Find(p => p.Id == playerId);

            base.DestroyPlayer(player);
        }

        /*
        public void RemovePlayer(Client client, Player player)
        {
            var p = (BasePlayer)BaseCurrentMap.Board[player.CellPosition.X, player.CellPosition.Y];
            if (p.Id == player.Id)
                BaseCurrentMap.Board[player.CellPosition.X, player.CellPosition.Y] = null;

            client.Player = null;

            base.RemovePlayer(player);
        }
        */

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

            if (wall == null)
                Program.Log.Error("Wall at " + position + " doesn't exist !");
            else
                Program.Log.Info("Wall at " + position + " has been removed.");
        }

        private void RemoveWall(Wall wall)
        {
            if (GameConfiguration.Random.Next(0, 100) < MathHelper.Clamp(GameConfiguration.PowerUpPercentage, 0, 100))
            {
                AddPowerUp(wall.CellPosition);
            }

            _wallList.Remove(wall);

            base.RemoveWall(wall);

            Program.Log.Info("Delete wall at " + wall.CellPosition);
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

            if (bomb == null)
                Program.Log.Error("Bomb at " + position + "doesn't exist !");
            else
                Program.Log.Info("Bomb exploded at " + position);
        }

        private void RemoveBomb(Bomb bomb)
        {
            _bombList.Remove(bomb);

            base.RemoveBomb(bomb);

            Program.Log.Info("Bomb at " + bomb.CellPosition + " has been removed.");
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

            Program.Log.Info("Power up at " + powerUp.CellPosition + " has been taken by player " + player.Name + ".");

            powerUp.Remove();
        }

        protected override void DestroyPowerUp(Point position)
        {
            var powerUp = _powerUpList.Find(pu => pu.CellPosition == position);
            
            base.DestroyPowerUp(powerUp);

            if (powerUp == null)
                Program.Log.Error("Power up at " + position + "doesn't exist !");
            else
                Program.Log.Info("Power up at " + position + " has been destroyed.");
        }

        private void RemovePowerUp(PowerUp powerUp)
        {
            _powerUpList.Remove(powerUp);

            base.RemovePowerUp(powerUp);

            Program.Log.Info("Power up at " + powerUp.CellPosition + " has been removed.");
        }

        #endregion
    }
}

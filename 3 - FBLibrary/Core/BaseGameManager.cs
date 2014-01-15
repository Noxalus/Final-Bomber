using System;
using System.Collections.Generic;
using System.Linq;
using FBLibrary.Core.BaseEntities;
using Microsoft.Xna.Framework;

namespace FBLibrary.Core
{
    public abstract class BaseGameManager
    {
        // Engine
        private Engine _engine;

        public int[,] HazardMap;
        protected BaseMap BaseCurrentMap;

        private readonly List<BasePlayer> _basePlayerList;
        private readonly List<BasePlayer> _removedPlayers;
        private readonly List<BaseBomb> _baseBombList;
        private readonly List<BaseWall> _baseWallList;
        private readonly List<BasePowerUp> _basePowerUpList;

        // Events
        public BaseGameEventManager GameEventManager;

        // For network game manager (client side only)
        protected bool WaitServerResponse;

        protected BaseGameManager()
        {
            // Engine
            _engine = new Engine(32, 32, Vector2.Zero);

            _basePlayerList = new List<BasePlayer>();
            _removedPlayers = new List<BasePlayer>();
            _baseBombList = new List<BaseBomb>();
            _baseWallList = new List<BaseWall>();
            _basePowerUpList = new List<BasePowerUp>();

            WaitServerResponse = false;
        }

        public virtual void Reset()
        {
            BaseCurrentMap.Reset();

            HazardMap = new int[BaseCurrentMap.Size.X, BaseCurrentMap.Size.Y];

            _basePlayerList.ForEach(basePlayer => basePlayer.Reset());
            _removedPlayers.Clear();
            _baseBombList.Clear();
            _basePowerUpList.Clear();
            _baseWallList.Clear();

            // Generate walls for another game
            GenerateRandomWalls();

            // Randomize players position
            _basePlayerList.Sort(new PlayerRandomSorter());

            // Replace players to their original spawn point
            for (int i = 0; i < _basePlayerList.Count; i++)
            {
                _basePlayerList[i].ChangePosition(BaseCurrentMap.PlayerSpawnPoints[i]);
            }
        }

        #region Updates

        public virtual void Update()
        {
            UpdateBombs();
            UpdateWalls();
            UpdatePowerUps();
            UpdatePlayers();
        }

        protected virtual void UpdateWalls()
        {
            for (int i = 0; i < _baseWallList.Count; i++)
            {
                if (HazardMap[_baseWallList[i].CellPosition.X, _baseWallList[i].CellPosition.Y] == 3 &&
                    !_baseWallList[i].InDestruction)
                {
                    DestroyWall(_baseWallList[i].CellPosition);
                    HazardMap[_baseWallList[i].CellPosition.X, _baseWallList[i].CellPosition.Y] = 0;
                }
            }
        }

        protected virtual void UpdateBombs()
        {
            for (int i = 0; i < _baseBombList.Count; i++)
            {
                // Is it die ?
                if ((HazardMap[_baseBombList[i].CellPosition.X, _baseBombList[i].CellPosition.Y] == 3 ||
                    _baseBombList[i].Timer <= TimeSpan.Zero) && !_baseBombList[i].InDestruction)
                {
                    DestroyBomb(_baseBombList[i].CellPosition);
                }
            }
        }

        protected virtual void UpdatePowerUps()
        {
            for (int i = 0; i < _basePowerUpList.Count; i++)
            {
                // Is it die ?
                if (HazardMap[_basePowerUpList[i].CellPosition.X, _basePowerUpList[i].CellPosition.Y] == 3)
                    DestroyPowerUp(_basePowerUpList[i].CellPosition);
            }
        }

        protected virtual void UpdatePlayers()
        {
            for (var i = 0; i < _basePlayerList.Count; i++)
            {
                var basePlayer = _basePlayerList[i];
                if (basePlayer.IsAlive)
                {
                    // We clean the obsolete players
                    if (basePlayer.IsAlive)
                    {
                        if (!WaitServerResponse)
                        {
                            // Pick up a power up ?
                            var powerUp =
                                BaseCurrentMap.Board[basePlayer.CellPosition.X, basePlayer.CellPosition.Y] as
                                    BasePowerUp;
                            if (powerUp != null)
                            {
                                if (!powerUp.InDestruction)
                                {
                                    if (!basePlayer.HasBadEffect ||
                                        (basePlayer.HasBadEffect && powerUp.Type != PowerUpType.BadEffect))
                                    {
                                        PickUpPowerUp(basePlayer, powerUp);
                                    }
                                }
                            }

                        // Is it die ?
                            if (!basePlayer.InDestruction && !basePlayer.IsInvincible &&
                                HazardMap[basePlayer.CellPosition.X, basePlayer.CellPosition.Y] == 3)
                            {
                                // Bomb
                                int bombId = -42;
                                List<BaseBomb> bl = _baseBombList.FindAll(b => b.InDestruction);
                                foreach (BaseBomb b in bl)
                                {
                                    if (b.ActionField.Any(po => po == basePlayer.CellPosition))
                                    {
                                        bombId = b.PlayerId;
                                    }
                                }

                                // Suicide
                                if (bombId == basePlayer.Id)
                                {
                                    basePlayer.Stats.Suicides++;
                                    basePlayer.Stats.Score -= GameConfiguration.ScoreBySuicide;
                                }
                                    // Kill
                                else if (bombId >= 0 && bombId < _basePlayerList.Count)
                                {
                                    GetPlayerById(bombId).Stats.Kills++;
                                    GetPlayerById(bombId).Stats.Score += GameConfiguration.ScoreByKill;
                                    BasePlayer player = _basePlayerList.Find(p => p.Id == bombId);
                                    if (player.OnEdge)
                                    {
                                        //player.Rebirth(BasePlayerList[i].Position);
                                        //_deadBasePlayerListNumber--;
                                    }
                                }

                                DestroyPlayer(basePlayer.Id);
                            }
                        }
                    }
                }
                else
                {
                    // We check 
                    if (!_removedPlayers.Contains(basePlayer))
                    {
                        _removedPlayers.Add(basePlayer);
                        GameEventManager.OnPlayerDeath(basePlayer);
                    }
                }
            }
        }

        #endregion

        #region Player methods

        protected virtual void AddPlayer(BasePlayer player)
        {
            _basePlayerList.Add(player);
        }

        protected abstract void DestroyPlayer(int playerId);
        protected virtual void DestroyPlayer(BasePlayer basePlayer)
        {
            if (basePlayer != null)
                basePlayer.Destroy();
        }

        /* We don't want to delete players from the list, we reset its stats instead
        protected virtual void RemovePlayer(BasePlayer player)
        {
            var p = (BasePlayer)BaseCurrentMap.Board[player.CellPosition.X, player.CellPosition.Y];

            if (p != null)
            {
                if (p.Id == player.Id)
                {
                    BaseCurrentMap.Board[player.CellPosition.X, player.CellPosition.Y] = null;

                    GameEventManager.OnPlayerDeath();
                }
            }
            else
            {
                throw new Exception("This player doesn't exist and can't be removed !");
            }

            BasePlayerList.Remove(player);
        }
        */
        #endregion

        #region Wall methods

        public abstract void AddWall(Point position);
        protected virtual void AddWall(BaseWall baseWall)
        {
            BaseCurrentMap.Board[baseWall.CellPositionX, baseWall.CellPositionY] = baseWall;
            BaseCurrentMap.CollisionLayer[baseWall.CellPositionX, baseWall.CellPositionY] = true;

            _baseWallList.Add(baseWall);
        }

        protected abstract void DestroyWall(Point position);
        protected virtual void DestroyWall(BaseWall baseWall)
        {
            if (baseWall != null)
                baseWall.Destroy();
        }

        protected virtual void RemoveWall(BaseWall wall)
        {
            if (BaseCurrentMap.Board[wall.CellPosition.X, wall.CellPosition.Y] is BaseWall)
                BaseCurrentMap.Board[wall.CellPosition.X, wall.CellPosition.Y] = null;

            BaseCurrentMap.CollisionLayer[wall.CellPosition.X, wall.CellPosition.Y] = false;

            _baseWallList.Remove(wall);
        }

        #endregion

        #region Bomb methods

        protected virtual void AddBomb(BaseBomb bomb)
        {
            BaseCurrentMap.Board[bomb.CellPosition.X, bomb.CellPosition.Y] = bomb;
            BaseCurrentMap.CollisionLayer[bomb.CellPosition.X, bomb.CellPosition.Y] = true;

            _baseBombList.Add(bomb);
        }

        protected abstract void DestroyBomb(Point position);
        protected virtual void DestroyBomb(BaseBomb baseBomb)
        {
            if (baseBomb != null)
            {
                // We don't forget to give it back to its owner
                if (baseBomb.PlayerId >= 0)
                {
                    BasePlayer player = _basePlayerList.Find(p => p.Id == baseBomb.PlayerId);

                    if (player != null && player.CurrentBombAmount < player.TotalBombAmount)
                        player.CurrentBombAmount++;
                }

                baseBomb.Destroy();
            }
        }

        protected virtual void RemoveBomb(BaseBomb bomb)
        {
            if (BaseCurrentMap.Board[bomb.CellPosition.X, bomb.CellPosition.Y] is BaseBomb)
                BaseCurrentMap.Board[bomb.CellPosition.X, bomb.CellPosition.Y] = null;

            BaseCurrentMap.CollisionLayer[bomb.CellPosition.X, bomb.CellPosition.Y] = false;

            // Update the hazard map
            List<BaseBomb> bL = _baseBombList.FindAll(b => !b.InDestruction);
            foreach (Point p in bomb.ActionField)
            {
                bool sameCellThanAnOther = false;
                if (_baseBombList.Where(
                    b => !(b.CellPosition.X == bomb.CellPosition.X &&
                    b.CellPosition.Y == bomb.CellPosition.Y)).Any(
                    b => b.ActionField.Find(c => c.X == p.X && c.Y == p.Y) != Point.Zero))
                {
                    HazardMap[p.X, p.Y] = 2;
                    sameCellThanAnOther = true;
                }
                if (!sameCellThanAnOther)
                    HazardMap[p.X, p.Y] = 0;
            }

            _baseBombList.Remove(bomb);
        }

        #endregion

        #region Power Up

        public abstract void AddPowerUp(Point position);
        protected virtual void AddPowerUp(BasePowerUp basePowerUp)
        {
            BaseCurrentMap.Board[basePowerUp.CellPositionX, basePowerUp.CellPositionY] = basePowerUp;

            _basePowerUpList.Add(basePowerUp);
        }

        protected abstract void DestroyPowerUp(Point position);
        protected virtual void DestroyPowerUp(BasePowerUp basePowerUp)
        {
            if (basePowerUp != null)
                basePowerUp.Destroy();
        }

        protected virtual void RemovePowerUp(BasePowerUp basePowerUp)
        {
            if (BaseCurrentMap.Board[basePowerUp.CellPosition.X, basePowerUp.CellPosition.Y] is BasePowerUp)
                BaseCurrentMap.Board[basePowerUp.CellPosition.X, basePowerUp.CellPosition.Y] = null;

            _basePowerUpList.Remove(basePowerUp);
        }

        protected abstract void PickUpPowerUp(BasePlayer player, BasePowerUp powerUp);

        #endregion

        #region Map region
        public virtual void LoadMap(string mapName)
        {
            BaseCurrentMap.Parse(mapName, this);
            HazardMap = new int[BaseCurrentMap.Size.X, BaseCurrentMap.Size.Y];
        }

        public void GenerateRandomWalls(int wallPercentage = -1)
        {
            if (wallPercentage == -1)
                wallPercentage = GameConfiguration.WallPercentage;

            for (int x = 0; x < BaseCurrentMap.Size.X; x++)
            {
                for (int y = 0; y < BaseCurrentMap.Size.Y; y++)
                {
                    if (BaseCurrentMap.Board[x, y] == null && GameConfiguration.Random.Next(0, 100) < wallPercentage)
                    {
                        if (!NearPlayerSpawn(x, y))
                        {
                            AddWall(new Point(x, y));
                        }
                    }
                }
            }
        }

        private bool NearPlayerSpawn(int x, int y)
        {
            return
                BaseCurrentMap.PlayerSpawnPoints.Contains(new Point(x, y)) ||
                BaseCurrentMap.PlayerSpawnPoints.Contains(new Point(x, y - 1)) ||
                BaseCurrentMap.PlayerSpawnPoints.Contains(new Point(x, y + 1)) ||
                BaseCurrentMap.PlayerSpawnPoints.Contains(new Point(x - 1, y)) ||
                BaseCurrentMap.PlayerSpawnPoints.Contains(new Point(x + 1, y)) ||
                BaseCurrentMap.PlayerSpawnPoints.Contains(new Point(x + 1, y - 1)) ||
                BaseCurrentMap.PlayerSpawnPoints.Contains(new Point(x - 1, y - 1)) ||
                BaseCurrentMap.PlayerSpawnPoints.Contains(new Point(x + 1, y + 1)) ||
                BaseCurrentMap.PlayerSpawnPoints.Contains(new Point(x - 1, y + 1));
        }
        #endregion

        private BasePlayer GetPlayerById(int id)
        {
            return _basePlayerList.FirstOrDefault(basePlayer => basePlayer.Id == id);
        }

        #region Displaying region

        public void DisplayHazardMap()
        {
            for (int y = 0; y < BaseCurrentMap.Size.Y; y++)
            {
                for (int x = 0; x < BaseCurrentMap.Size.X; x++)
                {
                    Console.Write("{0} ", HazardMap[x, y]);
                }

                Console.WriteLine();
            }
        }


        #endregion
    }
}

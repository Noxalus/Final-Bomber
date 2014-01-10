using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
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

        private readonly List<BaseBomb> _baseBombList;
        protected readonly List<BasePlayer> BasePlayerList;
        private readonly List<BaseWall> _baseWallList;
        private readonly List<BasePowerUp> _basePowerUpList;

        protected BaseGameManager()
        {
            // Engine
            _engine = new Engine(32, 32, Vector2.Zero);

            _baseBombList = new List<BaseBomb>();
            BasePlayerList = new List<BasePlayer>();
            _baseWallList = new List<BaseWall>();
            _basePowerUpList = new List<BasePowerUp>();
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
            for (int i = 0; i < BasePlayerList.Count; i++)
            {
                // We clean the obsolete players
                if (BasePlayerList[i].IsAlive)
                {
                    // Pick up a power up ?
                    var powerUp = BaseCurrentMap.Board[BasePlayerList[i].CellPosition.X, BasePlayerList[i].CellPosition.Y] as BasePowerUp;
                    if (powerUp != null)
                    {
                        if (!powerUp.InDestruction)
                        {
                            if (!BasePlayerList[i].HasBadEffect ||
                                (BasePlayerList[i].HasBadEffect && powerUp.Type != PowerUpType.BadEffect))
                            {
                                PickUpPowerUp(BasePlayerList[i], powerUp);
                            }
                        }
                    }

                    // Is it die ?
                    if (!BasePlayerList[i].InDestruction && !BasePlayerList[i].IsInvincible &&
                        HazardMap[BasePlayerList[i].CellPosition.X, BasePlayerList[i].CellPosition.Y] == 3)
                    {
                        // Bomb
                        int bombId = -42;
                        List<BaseBomb> bl = _baseBombList.FindAll(b => b.InDestruction);
                        foreach (BaseBomb b in bl)
                        {
                            if (b.ActionField.Any(po => po == BasePlayerList[i].CellPosition))
                            {
                                bombId = b.PlayerId;
                            }
                        }

                        // Suicide
                        if (bombId == BasePlayerList[i].Id)
                        {
                            BasePlayerList[i].Stats.Suicides++;
                            BasePlayerList[i].Stats.Score -= GameConfiguration.ScoreBySuicide;
                        }
                        // Kill
                        else if (bombId >= 0 && bombId < BasePlayerList.Count)
                        {
                            GetPlayerById(bombId).Stats.Kills++;
                            GetPlayerById(bombId).Stats.Score += GameConfiguration.ScoreByKill;
                            BasePlayer player = BasePlayerList.Find(p => p.Id == bombId);
                            if (player.OnEdge)
                            {
                                //player.Rebirth(BasePlayerList[i].Position);
                                //_deadBasePlayerListNumber--;
                            }
                        }

                        DestroyPlayer(BasePlayerList[i].Id);
                    }
                }
            }
        }

        #endregion

        #region Player methods

        protected virtual void AddPlayer(BasePlayer player)
        {
            BasePlayerList.Add(player);
        }

        protected abstract void DestroyPlayer(int playerId);
        protected virtual void DestroyPlayer(BasePlayer basePlayer)
        {
            if (basePlayer != null)
                basePlayer.Destroy();
        }

        protected virtual void RemovePlayer(BasePlayer player)
        {
            var p = (BasePlayer)BaseCurrentMap.Board[player.CellPosition.X, player.CellPosition.Y];

            if (p != null)
            {
                if (p.Id == player.Id)
                    BaseCurrentMap.Board[player.CellPosition.X, player.CellPosition.Y] = null;
            }
            else
            {
                throw new Exception("This player doesn't exist and can't be removed !");
            }

            BasePlayerList.Remove(player);
        }

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
                    BasePlayer player = BasePlayerList.Find(p => p.Id == baseBomb.PlayerId);

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

        public virtual void Reset()
        {
            BaseCurrentMap.Reset();

            BasePlayerList.Clear();
            _baseBombList.Clear();
            _basePowerUpList.Clear();
            _baseWallList.Clear();
        }

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
            return BasePlayerList.FirstOrDefault(basePlayer => basePlayer.Id == id);
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

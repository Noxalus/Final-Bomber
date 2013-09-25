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
        protected Engine Engine;

        public int[,] HazardMap;
        public BaseMap BaseCurrentMap;

        protected readonly List<BaseBomb> BaseBombList;
        protected readonly List<BasePlayer> BasePlayerList;
        protected readonly List<BaseWall> BaseWallList;
        protected readonly List<BasePowerUp> BasePowerUpList;

        protected BaseGameManager()
        {
            // Engine
            Engine = new Engine(32, 32, Vector2.Zero);

            BaseBombList = new List<BaseBomb>();
            BasePlayerList = new List<BasePlayer>();
            BaseWallList = new List<BaseWall>();
            BasePowerUpList = new List<BasePowerUp>();
        }

        public virtual void Update()
        {
            UpdateWalls();
            UpdateBombs();
            UpdatePowerUps();
            UpdatePlayers();
        }

        protected virtual void UpdateWalls()
        {
            for (int i = 0; i < BaseWallList.Count; i++)
            {
                if (HazardMap[BaseWallList[i].CellPosition.X, BaseWallList[i].CellPosition.Y] == 3 &&
                    !BaseWallList[i].InDestruction)
                {
                    DestroyWall(BaseWallList[i].CellPosition);
                    HazardMap[BaseWallList[i].CellPosition.X, BaseWallList[i].CellPosition.Y] = 0;
                }
            }
        }

        protected virtual void UpdateBombs()
        {
            for (int i = 0; i < BaseBombList.Count; i++)
            {
                // Is it die ?
                if ((HazardMap[BaseBombList[i].CellPosition.X, BaseBombList[i].CellPosition.Y] == 3 ||
                    BaseBombList[i].Timer >= BaseBombList[i].TimerLenght) && !BaseBombList[i].InDestruction)
                {
                    DestroyBomb(BaseBombList[i].CellPosition);
                }
            }
        }

        protected virtual void UpdatePowerUps()
        {
            for (int i = 0; i < BasePowerUpList.Count; i++)
            {
                // Is it die ?
                if (HazardMap[BasePowerUpList[i].CellPosition.X, BasePowerUpList[i].CellPosition.Y] == 3)
                    DestroyPowerUp(BasePowerUpList[i].CellPosition);
            }
        }

        protected virtual void UpdatePlayers()
        {
            for (int i = 0; i < BasePlayerList.Count; i++)
            {
                // We clean the obsolete players
                if (!BasePlayerList[i].IsAlive)
                {
                    if (!BasePlayerList[i].OnEdge)
                    {
                        if (BaseCurrentMap.Board[BasePlayerList[i].CellPosition.X, BasePlayerList[i].CellPosition.Y] is BasePlayer)
                        {
                            var p = (BasePlayer)BaseCurrentMap.Board[BasePlayerList[i].CellPosition.X, BasePlayerList[i].CellPosition.Y];
                            if (p.Id == BasePlayerList[i].Id)
                                BaseCurrentMap.Board[BasePlayerList[i].CellPosition.X, BasePlayerList[i].CellPosition.Y] = null;
                        }

                        //_deadBasePlayerListNumber++;
                    }

                    if (true)//(Config.ActiveSuddenDeath && SuddenDeath.HasStarted))
                    {
                        //BasePlayerList.Remove(BasePlayerList[i]);
                    }
                    else
                    {
                        BasePlayerList[i].OnEdge = true;
                    }
                }
                else
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
                        List<BaseBomb> bl = BaseBombList.FindAll(b => b.InDestruction);
                        foreach (BaseBomb b in bl)
                        {
                            if (b.ActionField.Any(po => po == BasePlayerList[i].CellPosition))
                            {
                                bombId = b.PlayerId;
                            }
                        }

                        // Suicide
                        if (bombId == BasePlayerList[i].Id)
                            BasePlayerList[i].Stats.Suicides++;

                        else if (bombId >= 0 && bombId < BasePlayerList.Count)
                        {
                            BasePlayerList[i].Stats.Kills++;
                            BasePlayer player = BasePlayerList.Find(p => p.Id == bombId);
                            if (player.OnEdge)
                            {
                                //player.Rebirth(BasePlayerList[i].Position);
                                //_deadBasePlayerListNumber--;
                            }
                        }

                        BasePlayerList[i].Destroy();
                    }
                }
            }
        }

        #region Player region

        protected virtual void AddPlayer(BasePlayer player)
        {
            BasePlayerList.Add(player);
        }

        protected virtual void RemovePlayer(BasePlayer player)
        {
            BasePlayerList.Remove(player);
        }

        #endregion

        #region Wall methods

        public abstract void AddWall(Point position);
        protected virtual void AddWall(BaseWall baseWall)
        {
            BaseCurrentMap.Board[baseWall.CellPositionX, baseWall.CellPositionY] = baseWall;
            BaseCurrentMap.CollisionLayer[baseWall.CellPositionX, baseWall.CellPositionY] = true;

            BaseWallList.Add(baseWall);
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

            BaseWallList.Remove(wall);
        }

        #endregion

        #region Bomb methods

        protected virtual void AddBomb(BaseBomb bomb)
        {
            BaseCurrentMap.Board[bomb.CellPosition.X, bomb.CellPosition.Y] = bomb;
            BaseCurrentMap.CollisionLayer[bomb.CellPosition.X, bomb.CellPosition.Y] = true;

            BaseBombList.Add(bomb);
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
            List<BaseBomb> bL = BaseBombList.FindAll(b => !b.InDestruction);
            foreach (Point p in bomb.ActionField)
            {
                bool sameCellThanAnOther = false;
                if (BaseBombList.Where(
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

            BaseBombList.Remove(bomb);
        }

        #endregion

        #region Power Up

        public abstract void AddPowerUp(Point position);
        protected virtual void AddPowerUp(BasePowerUp basePowerUp)
        {
            BaseCurrentMap.Board[basePowerUp.CellPositionX, basePowerUp.CellPositionY] = basePowerUp;

            BasePowerUpList.Add(basePowerUp);
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

            BasePowerUpList.Remove(basePowerUp);
        }

        protected abstract void PickUpPowerUp(BasePlayer player, BasePowerUp powerUp);

        #endregion

        public virtual void LoadMap(string mapName)
        {
            BaseCurrentMap.Parse(mapName, this);
            HazardMap = new int[BaseCurrentMap.Size.X, BaseCurrentMap.Size.Y];
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

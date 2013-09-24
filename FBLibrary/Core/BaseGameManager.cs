using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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

        protected BaseGameManager()
        {
            // Engine
            Engine = new Engine(32, 32, Vector2.Zero);

            BaseBombList = new List<BaseBomb>();
            BasePlayerList = new List<BasePlayer>();
            BaseWallList = new List<BaseWall>();
        }

        public virtual void Update()
        {
            UpdateBombs();
        }

        private void UpdateBombs()
        {
            for (int i = 0; i < BaseBombList.Count; i++)
            {
                BaseBombList[i].Update();

                // Do we delete the bomb
                if (!BaseBombList[i].IsAlive)
                {
                    if (BaseCurrentMap.Board[BaseBombList[i].CellPosition.X, BaseBombList[i].CellPosition.Y] is BaseBomb)
                        BaseCurrentMap.Board[BaseBombList[i].CellPosition.X, BaseBombList[i].CellPosition.Y] = null;

                    BaseCurrentMap.CollisionLayer[BaseBombList[i].CellPosition.X, BaseBombList[i].CellPosition.Y] = false;

                    // We don't forget to give it back to its owner
                    if (BaseBombList[i].PlayerId >= 0)
                    {
                        BasePlayer player = BasePlayerList.Find(p => p.Id == BaseBombList[i].PlayerId);

                        if (player != null && player.CurrentBombAmount < player.TotalBombAmount)
                            player.CurrentBombAmount++;
                    }

                    // Update the hazard map
                    List<BaseBomb> bL = BaseBombList.FindAll(b => !b.InDestruction);
                    foreach (Point p in BaseBombList[i].ActionField)
                    {
                        bool sameCellThanAnOther = false;
                        if (BaseBombList.Where(
                            b => !(b.CellPosition.X == BaseBombList[i].CellPosition.X &&
                            b.CellPosition.Y == BaseBombList[i].CellPosition.Y)).Any(
                            b => b.ActionField.Find(c => c.X == p.X && c.Y == p.Y) != Point.Zero))
                        {
                            HazardMap[p.X, p.Y] = 2;
                            sameCellThanAnOther = true;
                        }
                        if (!sameCellThanAnOther)
                            HazardMap[p.X, p.Y] = 0;
                    }

                    RemoveBomb(BaseBombList[i]);
                }
            }
        }


        public abstract void AddWall(Point position);
        protected virtual void AddWall(BaseWall baseWall)
        {
            BaseCurrentMap.Board[baseWall.CellPositionX, baseWall.CellPositionY] = baseWall;
            BaseCurrentMap.CollisionLayer[baseWall.CellPositionX, baseWall.CellPositionY] = true;

            BaseWallList.Add(baseWall);
        }

        protected virtual void AddPlayer(BasePlayer player)
        {
            BasePlayerList.Add(player);
        }

        protected virtual void RemovePlayer(BasePlayer player)
        {
            BasePlayerList.Remove(player);
        }

        protected virtual void AddBomb(BaseBomb bomb)
        {
            BaseCurrentMap.Board[bomb.CellPosition.X, bomb.CellPosition.Y] = bomb;
            BaseCurrentMap.CollisionLayer[bomb.CellPosition.X, bomb.CellPosition.Y] = true;

            BaseBombList.Add(bomb);
        }

        protected virtual void RemoveBomb(BaseBomb bomb)
        {
            BaseBombList.Remove(bomb);
        }

        public virtual void LoadMap(string mapName)
        {
            BaseCurrentMap.Parse(mapName, this);
            HazardMap = new int[BaseCurrentMap.Size.X, BaseCurrentMap.Size.Y];
        }
    }
}

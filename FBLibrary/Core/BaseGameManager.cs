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
            UpdateWalls();
            UpdateBombs();
        }

        private void UpdateWalls()
        {
            for (int i = 0; i < BaseWallList.Count; i++)
            {
                if (HazardMap[BaseWallList[i].CellPosition.X, BaseWallList[i].CellPosition.Y] == 3)
                {
                    DestroyWall(BaseWallList[i].CellPosition);
                    HazardMap[BaseWallList[i].CellPosition.X, BaseWallList[i].CellPosition.Y] = 0;
                }
            }
        }

        private void UpdateBombs()
        {
        }

        protected virtual void AddPlayer(BasePlayer player)
        {
            BasePlayerList.Add(player);
        }

        protected virtual void RemovePlayer(BasePlayer player)
        {
            BasePlayerList.Remove(player);
        }

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

        protected virtual void RemoveBomb(BaseBomb bomb)
        {
            if (BaseCurrentMap.Board[bomb.CellPosition.X, bomb.CellPosition.Y] is BaseBomb)
                BaseCurrentMap.Board[bomb.CellPosition.X, bomb.CellPosition.Y] = null;

            BaseCurrentMap.CollisionLayer[bomb.CellPosition.X, bomb.CellPosition.Y] = false;

            DisplayHazardMap();

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

using System;
using System.Collections.Generic;
using FBClient.Core.Entities;
using FBClient.Entities;
using FBLibrary.Core;
using FBLibrary.Core.BaseEntities;
using Microsoft.Xna.Framework;

namespace FBClient.Core
{
    /// <summary>
    /// This class is specificaly used for the logic of local games
    /// </summary>
    public class LocalGameManager : GameManager
    {
        public LocalGameManager()
        {
        }

        protected override void UpdateWalls()
        {
            for (int i = 0; i < WallList.Count; i++)
            {
                WallList[i].Update(GameTime);

                // We clean the obsolete elements
                if (!WallList[i].IsAlive)
                {
                    RemoveWall(WallList[i]);
                }
            }

            base.UpdateWalls();
        }

        protected override void UpdateBombs()
        {
            for (int i = 0; i < BombList.Count; i++)
            {
                BombList[i].Update(GameTime);

                // Do we delete the bomb
                if (!BombList[i].IsAlive)
                {
                    RemoveBomb(BombList[i]);
                }
            }

            base.UpdateBombs();
        }

        protected override void UpdatePowerUps()
        {
            for (int i = 0; i < PowerUpList.Count; i++)
            {
                PowerUpList[i].Update(GameTime);

                if (!PowerUpList[i].IsAlive)
                {
                    RemovePowerUp(PowerUpList[i]);
                }
            }

            base.UpdatePowerUps();
        }

        protected override void UpdatePlayers()
        {
            for (int i = 0; i < Players.Count; i++)
            {
                if (Players[i].IsAlive)
                {
                    Players[i].Update(GameTime, CurrentMap, HazardMap);
                }
                else
                {
                    if (!BasePlayerList[i].OnEdge)
                    {
                        if (BaseCurrentMap.Board[BasePlayerList[i].CellPosition.X, BasePlayerList[i].CellPosition.Y] is BasePlayer)
                        {
                            RemovePlayer(BasePlayerList[i]);
                        }
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
            }

            base.UpdatePlayers();
        }

        #region Entity methods

        #region Player methods

        public void AddPlayer(Player player)
        {
            Players.Add(player);

            base.AddPlayer(player);
        }

        protected override void DestroyPlayer(int playerId)
        {
            var player = Players.Find(p => p.Id == playerId);

            base.DestroyPlayer(player);
        }

        public void RemovePlayer(Player player)
        {
            Players.Remove(player);

            base.RemovePlayer(player);
        }

        #endregion

        #region Wall methods

        public override void AddWall(Point position)
        {
            var wall = new Wall(position);
            WallList.Add(wall);

            base.AddWall(wall);
        }

        public void AddWall(Wall wall)
        {
            WallList.Add(wall);

            base.AddWall(wall);
        }

        public void AddWalls(IEnumerable<Point> wallPositions)
        {
            foreach (var position in wallPositions)
            {
                AddWall(position);
            }
        }

        protected override void DestroyWall(Point position)
        {
            var wall = WallList.Find(w => w.CellPosition == position);

            base.DestroyWall(wall);
        }

        private void RemoveWall(Wall wall)
        {
            WallList.Remove(wall);

            base.RemoveWall(wall);
        }

        #endregion

        #region Bomb methods

        public void AddBomb(Bomb bomb)
        {
            BombList.Add(bomb);

            base.AddBomb(bomb);
        }

        protected override void DestroyBomb(Point position)
        {
            var bomb = BombList.Find(b => b.CellPosition == position);

            base.DestroyBomb(bomb);
        }

        private void RemoveBomb(Bomb bomb)
        {
            BombList.Remove(bomb);

            base.RemoveBomb(bomb);
        }

        #endregion

        #region Power up methods

        public override void AddPowerUp(Point position)
        {
            throw new NotImplementedException();
        }

        public void AddPowerUp(PowerUpType type, Point position)
        {
            var powerUp = new PowerUp(position, type);
            PowerUpList.Add(powerUp);

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
            var powerUp = PowerUpList.Find(pu => pu.CellPosition == position);

            base.DestroyPowerUp(powerUp);
        }

        private void RemovePowerUp(PowerUp powerUp)
        {
            PowerUpList.Remove(powerUp);

            base.RemovePowerUp(powerUp);
        }

        #endregion

        #endregion
    }
}
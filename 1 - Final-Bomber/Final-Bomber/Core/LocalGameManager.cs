
using System.Linq;
using FBClient.Core.Entities;
using FBLibrary;
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

        protected override void RemoveWall(Wall wall)
        {
            if (GameConfiguration.Random.Next(0, 100) < MathHelper.Clamp(GameConfiguration.PowerUpPercentage, 0, 100))
            {
                AddPowerUp(wall.CellPosition);
            }

            base.RemoveWall(wall);
        }

        #region Events actions

        protected override void OnPlayerDeath()
        {
            // We check if the round is finished
            if (Players.Count(p => p.IsAlive) < 1)
                OnRoundEnd();

            base.OnPlayerDeath();
        }

        protected override void RoundEndAction()
        {
            base.RoundEndAction();

            //AddPlayer(Players[0]);
        }

        #endregion
    }
}
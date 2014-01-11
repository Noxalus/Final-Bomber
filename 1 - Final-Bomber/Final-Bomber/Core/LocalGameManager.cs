
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
    }
}
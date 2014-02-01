using System;
using System.Linq;
using FBClient.Core.Entities;
using FBClient.Core.Players;
using FBClient.Entities;
using FBLibrary;
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

        public override void Initialize()
        {
            for (var i = 0; i < GameConfiguration.PlayerNumber; i++)
            {
                if (i >= CurrentMap.PlayerSpawnPoints.Count)
                    break;

                var player = new HumanPlayer(i, i) { Name = PlayerInfo.Username + i };
                player.SetGameManager(this);
                player.ChangePosition(CurrentMap.PlayerSpawnPoints[player.Id]);

                AddPlayer(player);
            }

            base.Initialize();
        }

        public override void Reset()
        {
            base.Reset();


        }

        public override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update()
        {
            base.Update();
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

        public override void PlayerDeathAction(BasePlayer sender, EventArgs args)
        {
            // We check if the round is finished
            if (Players.Count(p => p.IsAlive) == GameConfiguration.AlivePlayerRemaining)
                GameEventManager.OnRoundEnd();

            base.PlayerDeathAction(sender, args);
        }

        public override void RoundEndAction()
        {
            base.RoundEndAction();
        }

        #endregion
    }
}
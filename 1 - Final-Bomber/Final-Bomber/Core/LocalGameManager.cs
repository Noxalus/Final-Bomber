
using System;
using System.Collections.Generic;
using System.Linq;
using FBClient.Core.Entities;
using FBClient.Core.Players;
using FBClient.Entities;
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

        public override void Initialize()
        {
            for (var i = 0; i < GameConfiguration.PlayerNumber; i++)
            {
                var player = new HumanPlayer(i) { Name = PlayerInfo.Username + i };
                player.SetGameManager(this);
                player.ChangePosition(this.CurrentMap.PlayerSpawnPoints[player.Id]);

                AddPlayer(player);
            }

            base.Initialize();
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

        protected override void OnPlayerDeath()
        {
            // We check if the round is finished
            if (Players.Count(p => p.IsAlive) < GameConfiguration.AlivePlayerRemaining)
                GameEventManager.OnRoundEnd();

            base.OnPlayerDeath();
        }

        public override void RoundEndAction()
        {
            base.RoundEndAction();

            //AddPlayer(Players[0]);
        }

        #endregion
    }
}
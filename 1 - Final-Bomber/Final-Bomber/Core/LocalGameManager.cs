
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

            foreach(var player in Players)
                player.LoadContent();
        }

        public override void Update()
        {
            base.Update();

            foreach (var player in Players)
            {
                var gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromTicks(GameConfiguration.DeltaTime));
                // TODO: remove first argument => we don't need totalGameTime
                player.Update(gameTime, CurrentMap, HazardMap); 
            }
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
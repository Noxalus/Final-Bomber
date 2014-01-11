
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
        private PlayerCollection _savedPlayers;
        public LocalGameManager()
        {
        }

        public override void Initialize()
        {
            _savedPlayers = new PlayerCollection();

            for (var i = 0; i < GameConfiguration.PlayerNumber; i++)
            {
                var player = new HumanPlayer(i) { Name = PlayerInfo.Username + i };
                player.SetGameManager(this);
                player.ChangePosition(this.CurrentMap.PlayerSpawnPoints[player.Id]);

                _savedPlayers.Add(player);
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

        public override void PlayerDeathAction()
        {
            // We check if the round is finished
            if (Players.Count(p => p.IsAlive) == GameConfiguration.AlivePlayerRemaining + 1)
                GameEventManager.OnRoundEnd();

            base.PlayerDeathAction();
        }

        public override void RoundEndAction()
        {
            base.RoundEndAction();

            foreach (var player in _savedPlayers)
            {
                AddPlayer(player);
            }
        }

        #endregion
    }
}
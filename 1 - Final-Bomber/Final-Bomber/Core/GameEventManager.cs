using System;
using FBClient.Entities;
using FBLibrary.Core;
using FBLibrary.Core.BaseEntities;

namespace FBClient.Core
{
    /// <summary>
    /// Class to manage all game events specifically for client side
    /// </summary>
    public sealed class GameEventManager : BaseGameEventManager
    {
        // Reference to game manager
        private readonly GameManager _gameManager;

        public GameEventManager(GameManager gameManager)
        {
            _gameManager = gameManager;
        }

        #region Events action

        protected override void RoundEndAction()
        {
            _gameManager.RoundEndAction();

            base.RoundEndAction();
        }

        protected override void PlayerDeathAction(BasePlayer sender, EventArgs args)
        {
            _gameManager.PlayerDeathAction(sender, args);

            base.PlayerDeathAction(sender, args);
        }

        #endregion
    }
}

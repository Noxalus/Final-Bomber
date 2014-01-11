using FBLibrary.Core;

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
        }

        protected override void PlayerDeathAction()
        {
            _gameManager.PlayerDeathAction();
        }

        #endregion
    }
}

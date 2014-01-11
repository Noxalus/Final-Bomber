
namespace FBClient.Core
{
    /// <summary>
    /// Class to manage all game events like game start, bomb planted, etc...
    /// </summary>
    public class GameEventManager
    {
        // Reference to game manager
        private GameManager _gameManager;

        #region Events

        public delegate void RoundEndEventHandler();
        public event RoundEndEventHandler RoundEnd;

        #endregion

        public GameEventManager(GameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public void Initialize()
        {
            // Bind events
            RoundEnd += _gameManager.RoundEndAction;
        }

        public void Dispose()
        {
            RoundEnd -= _gameManager.RoundEndAction;
        }

        #region Events actions

        #region Round End

        public virtual void OnRoundEnd()
        {
            if (RoundEnd != null)
                RoundEnd();
        }

        #endregion

        #endregion
    }
}

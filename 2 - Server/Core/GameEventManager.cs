using FBLibrary.Core;

namespace FBServer.Core
{
    public class GameEventManager : BaseGameEventManager
    {
        // Reference to game manager
        private readonly GameManager _gameManager;

        public GameEventManager(GameManager gameManager)
        {
            _gameManager = gameManager;
        }
    }
}

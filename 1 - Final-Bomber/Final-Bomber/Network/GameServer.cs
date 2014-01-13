using FBClient.Core;

namespace FBClient.Network
{
    sealed partial class GameServer
    {
        public NetworkGameManager GameManager;

        public readonly ClientCollection Clients;

        private static GameServer _instance;
        
        public static GameServer Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameServer();
                }

                return _instance;
            }
        }

        private GameServer()
        {
            Clients = new ClientCollection();
        }

        public void SetGameManager(GameManager gameManager)
        {
            GameManager = (NetworkGameManager)gameManager;
            GameManager.Initialize();
        }
    }
}

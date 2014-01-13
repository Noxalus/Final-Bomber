using FBClient.Core;

namespace FBClient.Network
{
    sealed partial class GameServer
    {
        public NetworkGameManager GameManager;

        public readonly ClientCollection Clients;

        // Events
        /*
        #region DisconnectedClientEvent
        public delegate void DisconnectedClientEventHandler(Client sender, EventArgs e);
        public event DisconnectedClientEventHandler DisconnectedClient;

        private void OnDisconnectedClient(Client sender, EventArgs e)
        {
            if (DisconnectedClient != null)
                DisconnectedClient(sender, e);
        }
        #endregion
        #region ConnectedClientEvent
        public delegate void ConnectedClientEventHandler(Client sender, EventArgs e);
        public event ConnectedClientEventHandler ConnectedClient;

        private void OnConnectedClient(Client sender, EventArgs e)
        {
            if (ConnectedClient != null)
                ConnectedClient(sender, e);
        }
        #endregion
        */

        private static GameServer _instance;
        
        public static GameServer Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameServer();
                    _instance.Initialize();
                }

                return _instance;
            }
        }

        private GameServer()
        {
            Clients = new ClientCollection();
        }

        public void Initialize()
        {
            /*
            ConnectedClient += GameServer_ConnectedClient;
            DisconnectedClient += GameServer_DisconnectedClient;
            */

            GameManager.Initialize();
        }

        private void Dispose()
        {
            /*
            ConnectedClient -= GameServer_ConnectedClient;
            DisconnectedClient -= GameServer_DisconnectedClient;
            */
        }

        public void SetGameManager(GameManager gameManager)
        {
            GameManager = (NetworkGameManager)gameManager;
        }
    }
}

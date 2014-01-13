
using System;
using FBClient.Core;

namespace FBClient.Network
{
    sealed partial class GameServer
    {
        public GameManager GameManager;

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
        }

        private void Initialize()
        {
            /*
            ConnectedClient += GameServer_ConnectedClient;
            DisconnectedClient += GameServer_DisconnectedClient;
            */
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
            GameManager = gameManager;
        }
    }
}

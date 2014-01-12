
using System;
using FBClient.Core;

namespace FBClient.Network
{
    partial class GameServer
    {
        public ClientCollection Clients;

        public GameManager GameManager;

        // Events
        #region DisconnectedClientEvent
        public delegate void DisconnectedClientEventHandler(Client sender, EventArgs e);
        public event DisconnectedClientEventHandler DisconnectedClient;
        protected virtual void OnDisconnectedClient(Client sender, EventArgs e)
        {
            if (DisconnectedClient != null)
                DisconnectedClient(sender, e);
        }
        #endregion
        #region ConnectedClientEvent
        public delegate void ConnectedClientEventHandler(Client sender, EventArgs e);
        public event ConnectedClientEventHandler ConnectedClient;
        protected virtual void OnConnectedClient(Client sender, EventArgs e)
        {
            if (ConnectedClient != null)
                ConnectedClient(sender, e);
        }
        #endregion

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

        private void Initialize()
        {
            ConnectedClient += GameServer_ConnectedClient;
            DisconnectedClient += GameServer_DisconnectedClient;
        }

        private void Dispose()
        {
            ConnectedClient -= GameServer_ConnectedClient;
            DisconnectedClient -= GameServer_DisconnectedClient;
        }

        public void SetGameManager(GameManager gameManager)
        {
            GameManager = gameManager;
        }

        public void AddClient(Client client)
        {
            Clients.Add(client);
        }

        public void RemoveClient(Client client)
        {
            Clients.Remove(client);
        }

        private void GameServer_ConnectedClient(Client sender, EventArgs e)
        {
            Clients.Add(sender);
        }

        private void GameServer_DisconnectedClient(Client sender, EventArgs e)
        {
            Clients.Remove(sender);
        }
    }
}

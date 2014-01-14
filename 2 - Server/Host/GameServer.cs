using System;
using System.Linq;
using FBLibrary;
using FBServer.Core;
using Lidgren.Network;

namespace FBServer.Host
{
    sealed partial class GameServer
    {
        #region Fields

        // Game manager
        private readonly GameManager _gameManager;

        // Singleton
        private static GameServer _instance;

        NetServer _server;
        bool _hostStarted = false;
        int _clientId = 0;
        public ClientCollection Clients;

        public string SelectedMapName;

        private readonly Timer _pingsTimer;
        private readonly Timer _sendPlayersPositionTimer;

        #endregion

        #region Properties

        public static GameServer Instance
        {
            get { return _instance ?? (_instance = new GameServer()); }
        }
        public GameManager GameManager
        {
            get { return _gameManager; }
        }
        public bool HostStarted
        {
            get { return _hostStarted; }
        }

        #endregion

        #region Events

        // Delegates
        public delegate void DisconnectedClientEventHandler(Client sender, EventArgs e);
        public delegate void ConnectedClientEventHandler(Client sender, EventArgs e);

        // Handlers
        public event DisconnectedClientEventHandler DisconnectedClient;
        public event ConnectedClientEventHandler ConnectedClient;

        #endregion

        #region Constructor
        
        private GameServer()
        {
            _gameManager = new GameManager();
            SelectedMapName = MapLoader.MapFileDictionary.Keys.First();
            _pingsTimer = new Timer(true);
            _sendPlayersPositionTimer = new Timer(true);
        }
        
        #endregion

        #region Events handler

        /// <summary>
        /// Event raised when a client is disconnecting
        /// </summary>
        /// <param name="sender">Client that is disconnecting</param>
        /// <param name="e">Optionnal arguments</param>
        private void OnDisconnectedClient(Client sender, EventArgs e)
        {
            if (DisconnectedClient != null)
                DisconnectedClient(sender, e);
        }

        /// <summary>
        /// Event raised when a client is connecting
        /// </summary>
        /// <param name="sender">Client that is disconnecting</param>
        /// <param name="e">Optionnal arguments</param>
        private void OnConnectedClient(Client sender, EventArgs e)
        {
            if (ConnectedClient != null)
                ConnectedClient(sender, e);
        }

        #endregion

        /// <summary>
        /// Start the server
        /// </summary>
        public void StartServer()
        {
            Clients = new ClientCollection();

            // Server basic configuration
            var config = new NetPeerConfiguration("Final-Bomber")
            {
                MaximumConnections = ServerSettings.MaxConnection, 
                Port = ServerSettings.Port,
                #if DEBUG
                //PingInterval = 1f, // send ping every 1 second
                //SimulatedLoss = 0.5f, // half packets lost
                //SimulatedMinimumLatency = 0.05f, // latency of 50 ms
                #endif
            };

            // To send ping to each player frequently
            config.SetMessageTypeEnabled(NetIncomingMessageType.ConnectionLatencyUpdated, true);

            try
            {
                _server = new NetServer(config);
                _server.Start();

                _hostStarted = true;

                GameManager.Initialize();

                Program.Log.Info("[START]Game server has started");
                Program.Log.Info("[PORT]: " + config.Port);
            }
            catch (NetException ex)
            {
                Program.Log.Fatal(ex.Message);

                throw;
            }
        }

        /// <summary>
        /// Check for new messages from clients
        /// </summary>
        public void Update()
        {
            // Read messages from clients
            CheckNewClientMessages();

            // Update the game manager
            GameManager.Update();

            if (GameManager.GameHasBegun)
            {
                // Synchronize clients (players positions)
                if (_sendPlayersPositionTimer.Each(ServerSettings.SendPlayersPositionTime))
                    SendPlayersPosition();
            }

            // Every 2 seconds we send the pings of all players to all players
            if (Clients.Count > 0 && _pingsTimer.Each(1000))
            {
                SendPings();

                _pingsTimer.Reset();
            }
        }

        private void CheckNewClientMessages()
        {
            NetIncomingMessage message;

            while ((message = _server.ReadMessage()) != null)
            {
                NetConnection sender = message.SenderConnection;
                Client currentClient = Clients.GetClientFromConnection(sender);

                switch (message.MessageType)
                {
                    #region StatusChanged
                    case NetIncomingMessageType.StatusChanged:
                        if (sender.Status == NetConnectionStatus.Connected)
                        {
                            if (currentClient == null)
                            {
                                var newClient = new Client(ref sender, _clientId);

                                _clientId++;

                                OnConnectedClient(newClient, new EventArgs());
                                Program.Log.Info("[RECEIVE][Client #" + newClient.ClientId + "] Just connected with ip: " + sender.RemoteEndPoint);
                            }
                        }
                        else
                        {
                            if (sender.Status != NetConnectionStatus.Connected)
                            {
                                if (currentClient != null)
                                {
                                    // the message type
                                    message.ReadByte();

                                    Clients.RemoveClient(currentClient);

                                    OnDisconnectedClient(currentClient, new EventArgs());
                                    Program.Log.Info("[Disconnected]Client " + currentClient.ClientId + " has disconnected (" + message.ReadString() + ")");
                                }
                            }
                        }
                        break;
                    #endregion

                    case NetIncomingMessageType.Data:
                        if (message.LengthBytes > 0)
                        {
                            try
                            {
                                DataProcessing(message, ref currentClient);
                            }
                            catch (Exception e)
                            {
                                Program.Log.Fatal("[ERROR]Client Id: " + '\"' + currentClient.ClientId + '\"' + " Ip: " + sender.RemoteEndPoint + " caused server exception");
                                Program.Log.Fatal("EXCEPTION: " + e);

                                sender.Disconnect("Caused a fatal server error, please check the log.");

                                throw;
                            }
                        }
                        break;

                    case NetIncomingMessageType.ConnectionLatencyUpdated:
                        float ping = message.ReadFloat() * 1000;
                        //Program.Log.Info("Client #" + currentClient.ClientId + " ping: " + ping + "ms");
                        currentClient.Ping = ping;

                        break;

                }
            }
        }

        public void EndServer(string reason)
        {
            _server.Shutdown(reason);

            _hostStarted = false;

            Program.Log.Info("[END]Server stopped at " + DateTime.Now);
        }
    }
}

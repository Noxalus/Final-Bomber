using System.Threading;
using Lidgren.Network;
using System.Collections.Generic;
using System.Diagnostics;

namespace FBClient.Network
{
    sealed partial class GameServer
    {
        private bool _hasStarted;
        private bool _connected;
        private bool _disconnected;
        private bool _failedToConnect;

        private NetClient _client;

        private NetPeerConfiguration _config;

        // Message pool
        private Thread _messagePooling;
        private Queue<NetIncomingMessage> _messagePool;

        public bool HasStarted
        {
            get { return _hasStarted; }
        }
        public bool Connected
        {
            get { return _connected; }
        }
        public bool Disconnected
        {
            get { return _disconnected; }
        }

        public bool FailedToConnect
        {
            get { return _failedToConnect; }
            set { _failedToConnect = value; }
        }

        public const int Ping = 10;

        public void StartClientConnection(string ip, string port)
        {
            _config = new NetPeerConfiguration("Final-Bomber");
            _config.EnableMessageType(NetIncomingMessageType.ConnectionLatencyUpdated);

            _hasStarted = true;
            _connected = false;
            _disconnected = false;

            // Client initialize 
            _client = new NetClient(_config);
            _client.Start();

            _messagePool = new Queue<NetIncomingMessage>();

            var threadStart = new ThreadStart(MessagePooling);
            _messagePooling = new Thread(threadStart);
            _messagePooling.Start();

            TryToConnect(ip, port);
        }


        public void TryToConnect(string ip, string port)
        {
            _client.Connect(ip, int.Parse(port));
        }

        public void Update()
        {
            CheckNewServerMessages();
        }

        private void CheckNewServerMessages()
        {
            // Check new messages from server
            while (_messagePool.Count > 0)
            {
                NetIncomingMessage message = _messagePool.Dequeue();
                byte type;

                switch (message.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        Debug.Print("A Data type message has been received from server.");
                        type = message.ReadByte();
                        DataProcessing(type, message);
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        var statusChangedReason = message.ReadString();

                        // Connection with server
                        if (!_connected && _client.ConnectionStatus == NetConnectionStatus.Connected)
                        {
                            _connected = true;
                            _disconnected = false;

                            // Send first packet => client credentials
                            Instance.SendCredentials();
                        }

                        // Disconnection with server
                        if (_connected && _client.ConnectionStatus == NetConnectionStatus.Disconnected)
                        {
                            _connected = false;
                            _disconnected = true;
                        }

                        // Failed to connect
                        if (statusChangedReason.Equals("=Failed"))
                        {
                            _failedToConnect = true;
                            Debug.Print("Couldn't connect to the Game Server, please refresh the game list");
                        }

                        Debug.Print("A StatusChanged has been received from server. (" + statusChangedReason + ")");
                        break;
                    case NetIncomingMessageType.ConnectionLatencyUpdated:
                        Debug.Print("A ConnectionLatencyUpdated type message has been received from server.");
                        float ping = message.ReadFloat() * 1000;

                        //RecievePing(ping);
                        break;
                    case NetIncomingMessageType.DebugMessage:
                        var debugMessage = message.ReadString();
                        Debug.Print("A DebugMessage has been received from server. (" + debugMessage + ")");
                        break;
                    default:
                        type = message.ReadByte();
                        var errorMessage = "Unhandle message type (" + type + ")";
                        Debug.Print(errorMessage);
                        Program.Log.Error(errorMessage);
                        break;
                }
            }
        }

        private void MessagePooling()
        {
            _messagePool.Clear();
            while (true)
            {
                NetIncomingMessage message;
                while ((message = _client.ReadMessage()) != null)
                {
                    //Debug.Print("Packet received from server !");
                    _messagePool.Enqueue(message);
                }

                Thread.Sleep(5);
            }
        }

        public void EndClientConnection(string reason)
        {
            _client.Disconnect(reason);
            _hasStarted = false;
            _connected = false;
            _disconnected = false;
            _messagePooling.Abort();
        }
    }
}

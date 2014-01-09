using System.Threading;
using Lidgren.Network;
using System.Collections.Generic;
using System.Diagnostics;

namespace FBClient.Network
{
    partial class GameServer
    {
        private bool _hasStarted;
        private bool _connected;
        private bool _disconnected;

        private NetClient _client;

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

        public const int Ping = 10;

        public void StartClientConnection(string ip, string port)
        {
            var config = new NetPeerConfiguration("Final-Bomber");
            config.EnableMessageType(NetIncomingMessageType.ConnectionLatencyUpdated);

            _client = new NetClient(config);

            _client.Start();
            _client.Connect(ip, int.Parse(port));

            _hasStarted = true;
            _connected = false;
            _disconnected = false;

            _messagePool = new Queue<NetIncomingMessage>();

            var threadStart = new ThreadStart(MessagePooling);
            _messagePooling = new Thread(threadStart);
            _messagePooling.Start();
        }

        public void RunClientConnection()
        {
            if (!_connected && _client.ConnectionStatus == NetConnectionStatus.Connected)
            {
                _connected = true;
                _disconnected = false;
            }
            if (_connected && _client.ConnectionStatus == NetConnectionStatus.Disconnected)
            {
                _connected = false;
                _disconnected = true;
            }

            if (_messagePool.Count > 0)
            {
                NetIncomingMessage message;
                while (_messagePool.Count > 0)
                {
                    message = _messagePool.Dequeue();
                    byte type;
                    switch (message.MessageType)
                    {
                        case NetIncomingMessageType.Data:
                            type = message.ReadByte();
                            DataProcessing(type, message);
                            break;
                        case NetIncomingMessageType.ConnectionLatencyUpdated:
                            float ping = message.ReadFloat() * 1000;
                            RecievePing(ping);
                            break;
                        default:
                            type = message.ReadByte();
                            Debug.Print("Unhandle message type (" + type + ")");
                            break;
                    }
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
                    Debug.Print("Packet received from server !");
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

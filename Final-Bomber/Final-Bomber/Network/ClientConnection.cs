using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Final_Bomber.Network
{
    partial class GameServer
    {
        private bool _hasStarted;
        private bool _connected;
        private bool _disconnected;

        private NetClient _client;

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
            _client = new NetClient(config);

            _client.Start();
            _client.Connect(ip, int.Parse(port));

            //buffer = client.CreateBuffer();

            _hasStarted = true;
            _connected = false;
            _disconnected = false;
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

            NetIncomingMessage message;
            while ((message = _client.ReadMessage()) != null)
            {
                Debug.Print("Packet received from server !");
                switch (message.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        byte type = message.ReadByte();
                        DataProcessing(type, message);
                        break;
                }
            }
        }

        public void EndClientConnection(string reason)
        {
            _client.Disconnect(reason);
            _hasStarted = false;
            _connected = false;
            _disconnected = false;
        }
    }
}

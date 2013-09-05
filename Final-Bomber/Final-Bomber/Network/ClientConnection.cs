using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Final_Bomber.Network
{
    partial class GameServer
    {
        private bool hasStarted;
        private bool connected;
        private bool disconnected;

        private NetClient client;

        private NetBuffer buffer = new NetBuffer();

        public bool HasStarted
        {
            get { return hasStarted; }
        }
        public bool Connected
        {
            get { return connected; }
        }
        public bool Disconnected
        {
            get { return disconnected; }
        }

        public int Ping = 10;

        public void StartClientConnection(string Ip, string Port)
        {
            NetPeerConfiguration config = new NetPeerConfiguration("Final-Bomber");
            client = new NetClient(config);

            client.Connect(Ip, int.Parse(Port));

            //buffer = client.CreateBuffer();

            hasStarted = true;
            connected = false;
            disconnected = false;
        }

        public void RunClientConnection()
        {
            if (!connected && client.ConnectionStatus == NetConnectionStatus.Connected)
            {
                connected = true;
                disconnected = false;
            }
            if (connected && client.ConnectionStatus == NetConnectionStatus.Disconnected)
            {
                connected = false;
                disconnected = true;
            }

            //NetMessageType type;
            NetIncomingMessage type;
            while ((type = client.ReadMessage()).LengthBits > 0)
            {
                switch (type.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        DataProcessing(buffer.ReadByte());
                        break;
                }
            }
        }

        public void EndClientConnection(string reason)
        {
            client.Disconnect(reason);
            hasStarted = false;
            connected = false;
            disconnected = false;
        }
    }
}

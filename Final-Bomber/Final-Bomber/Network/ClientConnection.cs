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
        private bool hasStarted;
        private bool connected;
        private bool disconnected;

        private NetClient client;

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

            client.Start();
            NetConnection test = client.Connect(Ip, int.Parse(Port));

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

            NetIncomingMessage incMsg = client.ReadMessage();
            while ((incMsg = client.ReadMessage()) != null)
            {
                Debug.Print("Packet received from server !");
                switch (incMsg.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        byte type = incMsg.ReadByte();
                        DataProcessing(type, incMsg);
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;

namespace Final_Bomber.Net
{
    partial class GameServer
    {
        #region Field Region
        private bool hasStarted;
        private bool connected;
        private bool disconnected;

        private NetClient client;
        private NetIncomingMessage msgIn;
        private NetOutgoingMessage msgOut;

        public int Ping = 10;
        #endregion

        #region Property Region
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
        #endregion

        public void StartClientConnection(string Ip, string Port)
        {
            NetPeerConfiguration config = new NetPeerConfiguration("Final-Bomber");
            client = new NetClient(config);

            client.Connect(Ip, int.Parse(Port));

            msgOut = client.CreateMessage();

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

            while ((msgIn = client.ReadMessage()) != null)
            {
                switch (msgIn.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        DataProcessing(msgIn.ReadByte());
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

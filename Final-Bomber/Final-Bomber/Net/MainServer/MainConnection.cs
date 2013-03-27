using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;

namespace Final_Bomber.Net.MainServer
{
    public partial class MainServer
    {
        #region Field Region
        const int PORT = 6789;
        const string HOSTNAME = "final-bomber.game-server.com";

        //static string IP = Dns.GetHostAddresses(HOSTNAME)[0].ToString(); // Internet
        static string IP = "127.0.0.1"; // Local
        //static string IP = "192.160.0.32"; // Lan

        bool hasStarted;
        bool connected;
        bool disconnected;

        public NetClient client;
        NetIncomingMessage msgIn;
        NetOutgoingMessage msgOut;
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

        public void StartMainConnection()
        {
            var config = new NetPeerConfiguration("Final-Bomber");
            client = new NetClient(config);
            client.Start();
            client.Connect(IP, PORT);

            msgOut = client.CreateMessage();

            hasStarted = true;
            connected = false;
            disconnected = false;
        }

        public void RunMainConnection()
        {
            if (!connected && client.ConnectionStatus == NetConnectionStatus.Connected)
            {
                connected = true;
                disconnected = false;
            }
            if (connected && client.ConnectionStatus == NetConnectionStatus.Disconnected)
            {
                connected = false;
                /*
                MessageBox.Show("Main Server has closed");
                Environment.Exit(0);
                */
            }

            while ((msgIn = client.ReadMessage()) != null)
            {
                switch (msgIn.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        if (msgIn.LengthBytes > 1)
                        {
                            DataProcessing(msgIn.ReadByte());
                        }
                        break;
                }
            }
        }

        public void EndMainConnection(string reason)
        {
            client.Disconnect(reason);
            hasStarted = false;
            connected = false;
            disconnected = false;
        }
    }
}

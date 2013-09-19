using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;
using System.Net.Sockets;

namespace Final_BomberServer.Host
{
    partial class GameServer
    {
        # region Variables/Properties

        const int MAXCONNECTION = 50;
        const int PORT = 2643;

        db_FileIO db = new db_FileIO();
        bool hostStarted = false;

        NetServer server;
        NetBuffer buffer;

        int clientId = 1;
        public ClientCollection clients;
        public NetPeerConfiguration config;

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

        public bool HostStarted
        {
            get { return hostStarted; }
        }

        #endregion

        public void StartServer()
        {
            config = new NetPeerConfiguration("Final-Bomber");
            config.MaximumConnections = MAXCONNECTION;
            config.Port = PORT;

            bool checkPort;
            do
            {
                checkPort = false;
                try
                {
                    server = new NetServer(config);
                    server.Start();
                }
                catch (NetException)
                {
                    checkPort = true;
                    config.Port++;
                    WriteOutput("Can't use old port, trying with Port: " + config.Port);
                    if (config.Port > 6800)
                    {
                        checkPort = false;
                    }
                }
            } while (checkPort);

            clients = new ClientCollection();

            hostStarted = true;
            WriteOutput("[START]Game has started at " + System.DateTime.Now);
            WriteOutput("[PORT]: " + config.Port);
        }

        public void RunServer()
        {
            NetIncomingMessage message;

            while ((message = server.ReadMessage()) != null)
            {
                NetConnection sender = message.SenderConnection;
                Client currentClient = clients.GetClientFromConnection(sender);

                switch (message.MessageType)
                {
                    #region StatusChanged
                    case NetIncomingMessageType.StatusChanged:
                        if (sender.Status == NetConnectionStatus.Connected)
                        {
                            if (currentClient == null)
                            {
                                var con = new Client(ref sender, clientId);
                                WriteOutput("[Connected]Client " + con.ClientId + " connected with ip: " + sender.RemoteEndPoint.ToString());
                                clients.AddClient(con);
                                clientId++;
                                OnConnectedClient(con, new EventArgs());
                            }
                        }
                        else
                        {
                            if (sender.Status != NetConnectionStatus.Connected)
                            {
                                if (currentClient != null)
                                {
                                    message.ReadByte(); // the message type
                                    WriteOutput("[Disconnected]Client " + currentClient.ClientId + " has disconnected (" + message.ReadString() + ")");

                                    clients.RemoveClient(currentClient);
                                    OnDisconnectedClient(currentClient, new EventArgs());
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
                                DataProcessing(message.ReadByte(), ref currentClient);
                            }
                            catch (Exception e)
                            {
                                WriteOutput("[ERROR]Client Id: " + '\"' + currentClient.ClientId + '\"' +
                                    " Ip: " + sender.RemoteEndPoint.ToString() + " caused server exception");
                                WriteOutput("EXCEPTION: " + e.ToString());
                                sender.Disconnect("Caused server error");
                            }
                        }
                        break;

                } //ENDSWITCH
            } //ENDWHILE
        } //ENDMETHOD

        public void EndServer(string reson)
        {
            WriteOutput("[END]Server has ended at " + System.DateTime.Now.ToString());
            server.Shutdown(reson);
            hostStarted = false;
        }

        #region Private

        public void WriteOutput(string msg)
        {
            Program.Log.Info(msg);
        }

        #endregion
    }
}

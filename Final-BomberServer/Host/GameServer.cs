using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;

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
                    WriteOutput("Can't use old port, trying with Port: " + config.Port.ToString());
                    if (config.Port > 6800)
                    {
                        checkPort = false;
                    }
                }
            } while (checkPort);

            //buffer = server.CreateBuffer();

            clients = new ClientCollection();

            hostStarted = true;
            WriteOutput("[START]Game has started at " + System.DateTime.Now.ToString());
            WriteOutput("[PORT]: " + config.Port.ToString());

            //MainServer.SendCurrentPort();
        }

        public void RunServer()
        {
            NetIncomingMessage incMsg;
            NetConnection sender;
            while ((incMsg = server.ReadMessage()) != null)
            {
                sender = incMsg.SenderConnection;
                Client t_client = clients.GetClientFromConnection(sender);
                switch (incMsg.MessageType)
                {
                    #region StatusChanged
                    case NetIncomingMessageType.StatusChanged:
                        if (sender.Status == NetConnectionStatus.Connected)
                        {
                            if (t_client == null)
                            {
                                Client con = new Client(ref sender, clientId);
                                WriteOutput("[Connected]Client " + con.ClientId.ToString() + " connected with ip: " + sender.RemoteEndPoint.ToString());
                                clients.AddClient(con);
                                clientId++;
                                OnConnectedClient(con, new EventArgs());
                            }
                        }
                        else
                        {
                            if (sender.Status != NetConnectionStatus.Connected)
                            {
                                if (t_client != null)
                                {
                                    //string reason = "";
                                    //incMsg.ReadString(out reason);

                                    Console.WriteLine(incMsg.ReadString());
                                    //Console.WriteLine(reason);
                                    WriteOutput("[Disconnected]Client " + t_client.ClientId + " has disconnected ()");
                                    clients.RemoveClient(t_client);
                                    OnDisconnectedClient(t_client, new EventArgs());
                                }
                            }
                        }
                        break;
                    #endregion

                    case NetIncomingMessageType.Data:
                        if (buffer.LengthBytes > 0)
                        {
                            try
                            {
                                DataProcessing(buffer.ReadByte(), ref t_client);
                            }
                            catch (Exception e)
                            {
                                WriteOutput("[ERROR]Client Id: " + '\"' + t_client.ClientId + '\"' +
                                    " Ip: " + sender.RemoteEndPoint.ToString() + " caused server exception");
                                WriteOutput("EXCEPTION: " + e.ToString());
                                //sender.Disconnect("Caused server error", 0f);
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
            Console.WriteLine(msg);
        }

        #endregion
    }
}

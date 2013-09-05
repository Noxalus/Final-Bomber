using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Final_BomberNetwork.GameServer
{
    partial class Server
    {
        #region Field Region

        #region Constants Region

        const int MAXCONNECTION = 100;
        const int MAXPACKETPERSEK = 40;

        #endregion

        Database db = new Database();
        NetServer server;
        int clientId = 1;
        ClientCollection clients;
        ClientCollection hoster;
        NetIncomingMessage msg;
        Ranking ranker;
        Timer tmr_hoster = new Timer();
        Timer tmr_ranking = new Timer();
        int avrgRanking = 10; // milliseconds
        public bool showStatus = true;
        public bool showPackets = true;

        #endregion

        #region Property Region


        #endregion

        public void StartServer()
        {
            NetPeerConfiguration config = new NetPeerConfiguration("Final-Bomber");
            config.MaximumConnections = MAXCONNECTION;
            config.Port = 6789;
            config.PingInterval = 1f;

            server = new NetServer(config);
            server.Start();

            clients = new ClientCollection();
            hoster = new ClientCollection();

            ranker = new Ranking();
            ranker.Usernames = db.GetUsers();
            ranker.Sort(db);

            hasStarted = true;
            WriteOutput("[START]Server has started at " + System.DateTime.Now.ToString());

        }

        public void RunServer()
        {
            #region Hosting+RankSort
            if (tmr_hoster.Each(1000))
            {
                hoster.Clear();
                foreach (Client client in clients)
                {
                    if (client.Hosting)
                    {
                        hoster.Add(client);
                    }
                }

                Stopwatch w = new Stopwatch();
                w.Start();
                ranker.Usernames = db.GetUsers();
                ranker.Sort(db);
                w.Stop();
                avrgRanking = (avrgRanking + (int)w.ElapsedMilliseconds) / 2;
            }
            #endregion

            #region RankingShow
            if (tmr_ranking.Each(600000))
            {
                WriteOutput("Avrg Ranking time = " + avrgRanking.ToString() + " ms");
            }
            #endregion

            while ((msg = server.ReadMessage()) != null)
            {
                Client t_client = clients.GetClientFromConnection(msg.SenderConnection);
                switch (msg.MessageType)
                {
                    #region StatusChanged
                    case NetIncomingMessageType.StatusChanged:
                        if (showStatus)
                        {
                            NetConnectionStatus status = (NetConnectionStatus)msg.ReadByte();
                            WriteOutput("Status: " + status + " - Ip:" + msg.SenderConnection.RemoteEndPoint.ToString());
                        }
                        if (msg.SenderConnection.Status == NetConnectionStatus.Connected)
                        {
                            if (t_client == null)
                            {
                                Client client = new Client(/*ref*/ msg.SenderConnection, clientId);
                                clients.AddClient(client);
                                WriteOutput("[CONNECTED]Asigning Id: " + '\"' + clientId + '\"' + " to Ip: " + msg.SenderConnection.RemoteEndPoint.ToString());
                                clientId++;
                                SendCurrentVersion(client);
                            }
                        }
                        else
                        {
                            if (msg.SenderConnection.Status != NetConnectionStatus.Connected)
                            {
                                if (t_client != null)
                                {
                                    bool notExist = clients.RemoveClient(t_client);
                                    if (notExist)
                                    {
                                        WriteOutput("[DISCONNECTED]Client: " + '\"' + t_client.ClientId + '\"' + " with Ip: " + msg.SenderConnection.RemoteEndPoint.ToString());
                                        db.SetData(t_client.UserName, Database.UserData.LoggedIn, "0");
                                    }
                                    else
                                    {
                                        WriteOutput("[ERROR]Client: " + '\"' + t_client.ClientId + '\"' + " didnt exist");
                                    }
                                }
                            }
                        }
                        break;
                    #endregion

                    //case NetMessageType.DebugMessage:
                    //    WriteOutput("[DEBUG]" + buffer.ReadString());
                    //    break;

                    #region Data
                    case NetIncomingMessageType.Data:

                        if (msg.LengthBytes > 0 && t_client != null)
                        {
                            try
                            {
                                byte data = msg.ReadByte();
                                if (showPackets)
                                {
                                    WriteOutput("Client " + t_client.ClientId + " has sended a data packet of sort "
                                        + "\"" + data.ToString() + "\"");
                                }
                                DataProcessing(data, ref t_client);

                            }
                            catch (Exception e)
                            {
                                WriteOutput("[EXCEPTION]" + e.ToString());
                                WriteOutput("[ERROR]Client Id: " + '\"' + t_client.ClientId + '\"' +
                                    " Ip: " + msg.SenderConnection.RemoteEndPoint.ToString() + " caused server exception");
                                msg.SenderConnection.Disconnect("Error:0003");
                            }
                        }
                        break;
                    #endregion
                }
            }

            //ServerPacketProcessing();
        }

        public void EndServer(string reson)
        {
            foreach (Client client in clients)
            {
                if (client.LoggedIn)
                    db.SetData(client.UserName, Database.UserData.LoggedIn, "0");
            }
            WriteOutput("[END]Server has ended at " + System.DateTime.Now.ToString());
            server.Shutdown(reson);
            hasStarted = false;
        }

        #region Private

        private void WriteOutput(string msg)
        {
            Console.WriteLine(msg);
        }

        #endregion
    }
}

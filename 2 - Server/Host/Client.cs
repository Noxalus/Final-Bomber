using Final_BomberServer.Core;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final_BomberServer.Host
{
    public class Client
    {
        public Client(ref NetConnection client, int clientId)
        {
            this.client = client;
            this.clientId = clientId;
            AlreadyPlayed = false;
        }

        int clientId;
        NetConnection client;
        Player player;
        public bool isReady = false;
        public bool NewClient = true;
        public bool Spectating = false;
        public string Username;
        public bool AlreadyPlayed;
        public float Ping = 0f;

        public int ClientId
        {
            get { return clientId; }
            set { clientId = value; }
        }

        public NetConnection ClientConnection
        {
            get { return client; }
            set { client = value; }
        }

        public Player Player
        {
            get { return player; }
            set { player = value; }
        }

    }
}

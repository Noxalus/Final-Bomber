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
        public bool isReady = false; //Clienten säger till servern när den är klar att starta
        public bool NewClient = true; //Om clienten har precis connectat
        public bool Spectating = false;
        public string Username;
        public bool AlreadyPlayed;

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

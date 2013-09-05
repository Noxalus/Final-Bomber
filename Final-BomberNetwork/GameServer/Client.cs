using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Final_BomberNetwork.GameServer
{
    public class Client
    {
        #region Field Region
        int clientId;
        public string UserName = "";
        public bool LoggedIn = false;
        NetConnection client;

        // Hosting
        public string GameName = "";
        public string CurrentMap = "";
        public int MaxPlayers = 0;
        public int CurrentPlayers = 0;
        public int Port = 6790;
        bool hosting = false;
        #endregion

        #region Propery Region

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

        public bool Hosting
        {
            get { return hosting; }
            set { hosting = value; }
        }
        #endregion

        #region Constructor Region
        public Client(/*ref*/ NetConnection client, int clientId)
        {
            this.client = client;
            this.clientId = clientId;
        }
        #endregion

    }

    class Elo
    {
        public string Username;
        public bool Won;

        public Elo(string user, bool win)
        {
            Username = user;
            Won = win;
        }
    }
}

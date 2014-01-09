using FBServer.Core;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBServer.Host
{
    public class ClientCollection : List<Client>
    {
        //Om man hostar, så håller den reda på vilken bana man hostar just nu
        string mapName;
        public string MapName
        {
            get { return mapName; }
            set { mapName = value; }
        }

        bool hosting;
        public bool Hosting
        {
            get { return hosting; }
            set { hosting = value; }
        }

        public void AddClient(Client client)
        {
            foreach (Client con in this)
            {
                if (client.ClientId == con.ClientId)
                {
                    return;
                }
            }
            this.Add(client);
            return;
        }

        public bool RemoveClient(Client client)
        {
            foreach (Client con in this)
            {
                if (client.ClientId == con.ClientId)
                {
                    this.Remove(client);
                    return true;
                }
            }
            return false;
        }

        public bool IsClientsReady()
        {
            foreach (Client client in this)
            {
                if (!client.isReady)
                    return false;
            }

            return true;
        }

        public Client GetClientFromConnection(NetConnection connection)
        {
            foreach (Client con in this)
            {
                if (con.ClientConnection == connection)
                {
                    return con;
                }
            }
            return null;
        }

        public Client GetClientFromPlayer(Player player)
        {
            foreach (Client client in this)
            {
                if (client.Player == player)
                    return client;
            }
            return null;
        }

        public Client GetClientFromUsername(string username)
        {
            foreach (Client client in this)
            {
                if (client.Username == username)
                    return client;
            }
            return null;
        }

        public Player GetPlayerFromId(int playerId)
        {
            foreach (var client in this)
            {
                if (client.Player.Id == playerId)
                    return client.Player;
            }

            return null;
        }
        
        public List<Player> GetAlivePlayers()
        {
            var rtn = new List<Player>();
            foreach (Client client in this)
            {
                if (client.Player.IsAlive && !client.Spectating)
                    rtn.Add(client.Player);
            }
            return rtn;
        }

        public List<Player> GetPlayers()
        {
            var players = new List<Player>();
            foreach (Client client in this)
            {
                if (!client.Spectating)
                    players.Add(client.Player);
            }
            return players;
        }

    }

    public class ClientRandomSorter : Comparer<Client>
    {
        Random rnd = new Random(Environment.TickCount);
        public override int Compare(Client x, Client y)
        {
            if (x.ClientId == y.ClientId)
                return 0;
            if (rnd.Next(2) == 0)
            {
                return 1;
            }
            else
            {
                return -1;
            }

        }

    }
}

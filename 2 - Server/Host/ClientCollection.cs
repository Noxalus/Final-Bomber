using FBLibrary;
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
        // Keep a reference to hoster
        private Client _hoster;

        public ClientCollection()
        {
            _hoster = null;
        }

        public Client Hoster
        {
            get { return _hoster; }
        }

        public void AddClient(Client client)
        {
            // If client id is already taken
            if (this.Any(c => client.ClientId == c.ClientId))
            {
                throw new Exception("Client ID already taken ! (" + client.ClientId + ")");
            }

            // If it's the first client => it's the host
            if (Count == 1)
            {
                client.IsHost = true;
                _hoster = client;
            }

            Add(client);
        }

        public bool RemoveClient(Client client)
        {
            if (this.Any(c => client.ClientId == c.ClientId))
            {
                Remove(client);

                return true;
            }

            return false;
        }

        public bool IsClientsReady()
        {
            return this.All(client => client.IsReady);
        }

        public Client GetClientFromConnection(NetConnection connection)
        {
            return this.FirstOrDefault(con => con.ClientConnection == connection);
        }

        public Client GetClientFromPlayer(Player player)
        {
            return this.FirstOrDefault(client => client.Player == player);
        }

        public Client GetClientFromUsername(string username)
        {
            return this.FirstOrDefault(client => client.Username == username);
        }

        public Player GetPlayerFromId(int playerId)
        {
            return (from client in this where client.Player.Id == playerId select client.Player).FirstOrDefault();
        }

        public List<Player> GetPlayers()
        {
            return (from client in this where !client.Spectating select client.Player).ToList();
        }
    }

    public class ClientRandomSorter : Comparer<Client>
    {
        public override int Compare(Client client1, Client client2)
        {
            if ((client1 != null && client2 != null) && 
                (client1.ClientId == client2.ClientId))
                return 0;

            if (GameConfiguration.Random.Next(2) == 0)
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

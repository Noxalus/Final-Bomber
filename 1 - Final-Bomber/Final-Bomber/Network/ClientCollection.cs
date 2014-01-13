using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using FBClient.Core.Players;

namespace FBClient.Network
{
    class ClientCollection : List<Client>
    {
        private Client _me;

        public Client Me
        {
            get { return _me; }
        }

        public ClientCollection()
        {
            _me = null;
        }

        public void AddClient(Client client)
        {
            if (client.Id == _me.Id)
            {
                _me = client;
            }
            else
            {
                client.Player = new OnlinePlayer(Count);
            }

            Add(client);
        }

        public void CreateMe(int clientId)
        {
            _me = new Client(clientId) {Player = new HumanPlayer(0)};
        }

        public Client GetClientById(int id)
        {
            return this.FirstOrDefault(client => client.Id == id);
        }
    }

    public class BasicClientSorter : Comparer<Client>
    {
        public override int Compare(Client x, Client y)
        {
            if (x != null && y != null)
                return x.Id.CompareTo(y.Id);
            else
                throw new Exception("A client doesn't exist and can't be compared.");
        }
    }
}

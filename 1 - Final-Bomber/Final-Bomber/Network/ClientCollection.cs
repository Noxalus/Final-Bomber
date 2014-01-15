using System;
using System.Collections.Generic;
using System.Linq;
using FBClient.Core.Players;

namespace FBClient.Network
{
    class ClientCollection : List<Client>
    {
        public Client Me { get; private set; }

        public ClientCollection()
        {
            Me = null;
        }

        public void AddClient(Client client)
        {
            if (client.Id == Me.Id)
            {
                Me = client;
                Me.Player = new OnlineHumanPlayer(Me.Id, 0) {Name = Me.Username};
            }
            else
            {
                client.Player = new OnlinePlayer(client.Id) { Name = client.Username };
            }

            Add(client);
        }

        public void CreateMe(int clientId)
        {
            Me = new Client(clientId);
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

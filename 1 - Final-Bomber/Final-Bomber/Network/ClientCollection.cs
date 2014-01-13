using System.Collections.Generic;
using System.Linq;
using FBClient.Core.Players;

namespace FBClient.Network
{
    class ClientCollection : List<Client>
    {
        public Client Me;

        public ClientCollection()
        {
            Me = null;
        }

        public void AddClient(Client client, bool me = false)
        {
            if (me && Me == null)
            {
                Me = client;
                Me.Player = new OnlineHumanPlayer(0);
            }
            else
            {
                Me.Player = new OnlinePlayer(Count);
            }

            Add(client);
        }

        public Client GetClientById(int id)
        {
            return this.FirstOrDefault(client => client.Id == id);
        }
    }
}

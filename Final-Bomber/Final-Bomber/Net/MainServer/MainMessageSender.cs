using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;

namespace Final_Bomber.Net.MainServer
{
    partial class MainServer
    {
        public void SendGetHostedGames()
        {
            NetOutgoingMessage send = client.CreateMessage();
            send.Write((byte)SMT.GetHostedGames);
            client.SendMessage(send, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendGetStats()
        {
            NetOutgoingMessage send = client.CreateMessage();
            send.Write((byte)SMT.GetStats);
            client.SendMessage(send, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendGetRanking()
        {
            NetOutgoingMessage send = client.CreateMessage();
            send.Write((byte)SMT.GetRanking);
            client.SendMessage(send, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendLogin(string username, string password)
        {
            NetOutgoingMessage send = client.CreateMessage();
            send.Write((byte)SMT.LogIn);
            send.Write(username);
            send.Write(password);
            client.SendMessage(send, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendCreateAccount(string username, string password)
        {
            NetOutgoingMessage send = client.CreateMessage();
            send.Write((byte)SMT.CreateAccount);
            send.Write(username);
            send.Write(password);
            client.SendMessage(send, NetDeliveryMethod.ReliableOrdered);
        }
    }
}

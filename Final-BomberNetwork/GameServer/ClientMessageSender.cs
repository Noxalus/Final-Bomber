using Lidgren.Network;

namespace Final_BomberNetwork.GameServer
{
    public partial class GameServer
    {
        // Login
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

        // Game
        public void SendNeedMap()
        {
            if (client.ConnectionStatus == NetConnectionStatus.Connected)
            {
                NetOutgoingMessage send = client.CreateMessage();
                send.Write((byte)SMT.NeedMap);
                client.SendMessage(send, NetDeliveryMethod.ReliableOrdered);
            }
        }

        public void SendIsReady()
        {
            if (client.ConnectionStatus == NetConnectionStatus.Connected)
            {
                NetOutgoingMessage send = client.CreateMessage();
                send.Write((byte)SMT.Ready);
                send.Write(GameSettings.Username);
                send.Write(GameSettings.Password);
                client.SendMessage(send, NetDeliveryMethod.ReliableOrdered);
            }
        }

        public void SendMovement(byte direction)
        {
            if (client.ConnectionStatus == NetConnectionStatus.Connected)
            {
                NetOutgoingMessage send = client.CreateMessage();
                send.Write(direction);
                client.SendMessage(send, NetDeliveryMethod.ReliableOrdered);
            }
        }

        public void SendBombPlacing()
        {
            if (client.ConnectionStatus == NetConnectionStatus.Connected)
            {
                NetOutgoingMessage send = client.CreateMessage();
                send.Write((byte)SMT.PlaceBomb);
                client.SendMessage(send, NetDeliveryMethod.ReliableOrdered);
            }
        }
    }
}

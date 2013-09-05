using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Final_Bomber.Network
{
    partial class GameServer
    {
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

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
            if (_client.ConnectionStatus == NetConnectionStatus.Connected)
            {
                NetOutgoingMessage send = _client.CreateMessage();
                send.Write((byte)SMT.NeedMap);
                _client.SendMessage(send, NetDeliveryMethod.ReliableOrdered);
            }
        }

        public void SendIsReady()
        {
            if (_client.ConnectionStatus == NetConnectionStatus.Connected)
            {
                NetOutgoingMessage send = _client.CreateMessage();
                send.Write((byte)SMT.Ready);
                send.Write(GameSettings.Username);
                send.Write(GameSettings.Password);
                _client.SendMessage(send, NetDeliveryMethod.ReliableOrdered);
            }
        }

        public void SendMovement(byte direction)
        {
            if (_client.ConnectionStatus == NetConnectionStatus.Connected)
            {
                NetOutgoingMessage send = _client.CreateMessage();
                send.Write(direction);
                _client.SendMessage(send, NetDeliveryMethod.ReliableOrdered);
            }
        }

        public void SendBombPlacing()
        {
            if (_client.ConnectionStatus == NetConnectionStatus.Connected)
            {
                NetOutgoingMessage send = _client.CreateMessage();
                send.Write((byte)SMT.PlaceBomb);
                _client.SendMessage(send, NetDeliveryMethod.ReliableOrdered);
            }
        }
    }

}

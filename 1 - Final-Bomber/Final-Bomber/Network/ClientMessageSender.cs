using FBLibrary.Network;
using Lidgren.Network;

namespace FBClient.Network
{
    sealed partial class GameServer
    {
        public void SendNeedMap()
        {
            if (_client.ConnectionStatus == NetConnectionStatus.Connected)
            {
                NetOutgoingMessage send = _client.CreateMessage();
                send.Write((byte)MessageType.ClientMessage.NeedMap);
                _client.SendMessage(send, NetDeliveryMethod.ReliableOrdered);
            }
        }

        public void SendPlayerInfo()
        {
            if (_client.ConnectionStatus == NetConnectionStatus.Connected)
            {
                NetOutgoingMessage send = _client.CreateMessage();

                send.Write((byte)MessageType.ClientMessage.Credentials);
                send.Write(PlayerInfo.Username);
                send.Write(GameSettings.Password);

                _client.SendMessage(send, NetDeliveryMethod.ReliableOrdered);
            }
        }

        public void SendIsReady(bool value = true)
        {
            if (_client.ConnectionStatus == NetConnectionStatus.Connected)
            {
                NetOutgoingMessage send = _client.CreateMessage();
                
                send.Write((byte)MessageType.ClientMessage.Ready);
                send.Write(value);

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
                send.Write((byte)MessageType.ClientMessage.PlaceBomb);
                _client.SendMessage(send, NetDeliveryMethod.ReliableOrdered);
            }
        }
    }

}

using FBLibrary.Core;
using FBLibrary.Network;
using Lidgren.Network;
using System.Diagnostics;
using Lidgren.Network.Xna;

namespace FBClient.Network
{
    sealed partial class GameServer
    {
        int _counter = 0;

        private void DataProcessing(byte type, NetIncomingMessage message)
        {
            _counter++;
            Debug.Print("[" + _counter + "]Message received from server !");
            switch (type)
            {
                case (byte)MessageType.ServerMessage.ClientId:
                    Debug.Print("A message type 'ClientId' have been received from server !");
                    ReceiveMyClientId(message.ReadInt32());
                    break;
                case (byte)MessageType.ServerMessage.NewClientInfo:
                    Debug.Print("A message type 'NewClientInfo' have been received from server !");
                    ReceiveNewClientInfo(message.ReadInt32(), message.ReadString(), message.ReadBoolean());
                    break;
                case (byte)MessageType.ServerMessage.IsReady:
                    Debug.Print("A message type 'IsReady' have been received from server !");
                    ReceiveIsReady(message.ReadInt32(), message.ReadBoolean());
                    break;
                case (byte)MessageType.ServerMessage.AvailableMaps:
                    Debug.Print("A message type 'AvailableMaps' have been received from server !");
                    ReceiveAvailableMaps(message);
                    break;
                case (byte)MessageType.ServerMessage.SelectedMap:
                    Debug.Print("A message type 'SelectedMap' have been received from server !");
                    ReceiveSelectedMap(message.ReadString());
                    break;
                case (byte)MessageType.ServerMessage.Map:
                    Debug.Print("A message type 'Map' have been received from server !");
                    ReceiveMap(message);
                    break;
                case (byte)MessageType.ServerMessage.GameWillStart:
                    Debug.Print("A message type 'GameWillStart' have been received from server !");
                    ReceiveGameWillStart();
                    break;
                case (byte)MessageType.ServerMessage.StartGame:
                    Debug.Print("A message type 'StartGame' have been received from server !");
                    ReceiveStartGame(message);
                    break;
                case (byte)MessageType.ServerMessage.PlayerPosition:
                    Debug.Print("A message type 'PlayerPosition' have been received from server !");
                    ReceivePosition(message.ReadFloat(), message.ReadFloat(), message.ReadByte(), message.ReadInt32());
                    break;
                case (byte)MessageType.ServerMessage.RemovePlayer:
                    Debug.Print("A message type 'RemovePlayer' have been received from server !");
                    ReceiveRemovePlayer(message.ReadInt32());
                    break;
                case (byte)MessageType.ServerMessage.PlayerPlacingBomb:
                    Debug.Print("A message type 'PlayerPlacingBomb' have been received from server !");
                    ReceivePlacingBomb(message.ReadInt32(), message.ReadPoint());
                    break;
                case (byte)MessageType.ServerMessage.BombExploded:
                    ReceiveBombExploded(message);
                    break;
                case (byte)MessageType.ServerMessage.PlayerKill:
                    ReceivePlayerKill(message.ReadInt32(), message.ReadInt32());
                    break;
                case (byte)MessageType.ServerMessage.PlayerSuicide:
                    ReceivePlayerSuicide(message.ReadInt32());
                    break;
                case (byte)MessageType.ServerMessage.PowerUpDrop:
                    ReceivePowerupDrop((PowerUpType)message.ReadByte(), message.ReadPoint());
                    break;
                case (byte)MessageType.ServerMessage.PowerUpPickUp:
                    ReceivePowerUpPickUp(message.ReadInt32(), message.ReadPoint(), (PowerUpType)message.ReadByte());
                    break;
                case (byte)MessageType.ServerMessage.SuddenDeath:
                    //ReceiveSuddenDeath();
                    break;
                case (byte)MessageType.ServerMessage.RoundEnd:
                    Debug.Print("A message type 'RoundEnd' have been received from server !");
                    ReceiveRoundEnd();
                    break;
                case (byte)MessageType.ServerMessage.End:
                    Debug.Print("A message type 'End' have been received from server !");
                    ReceiveEnd(message.ReadBoolean());
                    break;
                case (byte)MessageType.ServerMessage.Pings:
                    //Debug.Print("A message type 'Pings have been received from server !");
                    ReceivePings(message);
                    break;
            }
        }
    }

}

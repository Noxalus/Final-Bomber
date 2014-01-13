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
                case (byte)MessageType.ServerMessage.GameStartInfo:
                    Debug.Print("A message type 'GameStartInfo' have been received from server !");
                    RecieveGameInfo(message.ReadString());
                    break;
                case (byte)MessageType.ServerMessage.Map:
                    Debug.Print("A message type 'Map' have been received from server !");
                    RecieveMap(message);
                    break;
                case (byte)MessageType.ServerMessage.StartGame:
                    Debug.Print("A message type 'StartGame' have been received from server !");
                    RecieveStartGame(message);
                    break;
                case (byte)MessageType.ServerMessage.PlayerPosition:
                    Debug.Print("A message type 'PlayerPosAndSpeed' have been received from server !");
                    RecievePosition(message.ReadFloat(), message.ReadFloat(), message.ReadByte(), message.ReadInt32());
                    break;
                case (byte)MessageType.ServerMessage.PlayerInfo:
                    Debug.Print("A message type 'PlayerInfo' have been received from server !");
                    RecievePlayerInfo(message.ReadInt32(), message.ReadString());
                    break;
                case (byte)MessageType.ServerMessage.RemovePlayer:
                    Debug.Print("A message type 'RemovePlayer' have been received from server !");
                    RecieveRemovePlayer(message.ReadInt32());
                    break;
                case (byte)MessageType.ServerMessage.PlayerPlacingBomb:
                    Debug.Print("A message type 'PlayerPlacingBomb' have been received from server !");
                    RecievePlacingBomb(message.ReadInt32(), message.ReadPoint());
                    break;
                case (byte)MessageType.ServerMessage.BombExploded:
                    RecieveBombExploded(message);
                    break;
                case (byte)MessageType.ServerMessage.PowerUpDrop:
                    RecievePowerupDrop((PowerUpType)message.ReadByte(), message.ReadPoint());
                    break;
                case (byte)MessageType.ServerMessage.PowerUpPick:
                    //RecievePowerupPick(buffer.ReadFloat(), buffer.ReadFloat(), buffer.ReadInt32(), buffer.ReadFloat());
                    break;
                case (byte)MessageType.ServerMessage.SuddenDeath:
                    //RecieveSuddenDeath();
                    break;
                case (byte)MessageType.ServerMessage.RoundEnd:
                    Debug.Print("A message type 'RoundEnd' have been received from server !");
                    RecieveRoundEnd();
                    break;
                case (byte)MessageType.ServerMessage.End:
                    Debug.Print("A message type 'End' have been received from server !");
                    RecieveEnd(message.ReadBoolean());
                    break;
            }
        }
    }

}

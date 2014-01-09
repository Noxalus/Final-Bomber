using FBLibrary.Core;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Lidgren.Network.Xna;

namespace FBClient.Network
{
    partial class GameServer
    {
        int _counter = 0;
        public void DataProcessing(byte type, NetIncomingMessage message)
        {
            _counter++;
            Debug.Print("[" + _counter + "]Message received from server !");
            switch (type)
            {
                case (byte)RMT.GameStartInfo:
                    Debug.Print("A message type 'GameStartInfo' have been received from server !");
                    RecieveGameInfo(message.ReadString());
                    break;
                case (byte)RMT.Map:
                    Debug.Print("A message type 'Map' have been received from server !");
                    RecieveMap(message);
                    break;
                case (byte)RMT.StartGame:
                    Debug.Print("A message type 'StartGame' have been received from server !");
                    RecieveStartGame(message);
                    break;
                case (byte)RMT.PlayerPosAndSpeed:
                    Debug.Print("A message type 'PlayerPosAndSpeed' have been received from server !");
                    RecievePositionAndSpeed(message.ReadFloat(), message.ReadFloat(), message.ReadByte(), message.ReadInt32());
                    break;
                case (byte)RMT.PlayerInfo:
                    Debug.Print("A message type 'PlayerInfo' have been received from server !");
                    RecievePlayerInfo(message.ReadInt32(), message.ReadFloat(), message.ReadString(), message.ReadInt32());
                    break;
                case (byte)RMT.RemovePlayer:
                    Debug.Print("A message type 'RemovePlayer' have been received from server !");
                    RecieveRemovePlayer(message.ReadInt32());
                    break;
                case (byte)RMT.PlayerPlacingBomb:
                    Debug.Print("A message type 'PlayerPlacingBomb' have been received from server !");
                    RecievePlacingBomb(message.ReadInt32(), message.ReadPoint());
                    break;
                case (byte)RMT.BombExploded:
                    RecieveBombExploded(message);
                    break;
                case (byte)RMT.Burn:
                    //RecieveBurn(buffer.ReadInt32());
                    break;
                case (byte)RMT.ExplodeTile:
                    //RecieveExplodeTile(buffer.ReadInt32());
                    break;
                case (byte)RMT.PowerupDrop:
                    RecievePowerupDrop((PowerUpType)message.ReadByte(), message.ReadPoint());
                    break;
                case (byte)RMT.PowerupPick:
                    //RecievePowerupPick(buffer.ReadFloat(), buffer.ReadFloat(), buffer.ReadInt32(), buffer.ReadFloat());
                    break;
                case (byte)RMT.SuddenDeath:
                    //RecieveSuddenDeath();
                    break;
                case (byte)RMT.SDExplosion:
                    //RecieveSDExplosion(buffer.ReadInt32());
                    break;
                case (byte)RMT.RoundEnd:
                    Debug.Print("A message type 'RoundEnd' have been received from server !");
                    RecieveRoundEnd();
                    break;
                case (byte)RMT.End:
                    Debug.Print("A message type 'End' have been received from server !");
                    RecieveEnd(message.ReadBoolean());
                    break;
            }
        }
    }

}

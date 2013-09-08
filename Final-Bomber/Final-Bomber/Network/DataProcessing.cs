using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Final_Bomber.Network
{
    partial class GameServer
    {
        int _counter = 0;
        public void DataProcessing(byte type, NetIncomingMessage incMsg)
        {
            _counter++;
            Debug.Print("[" + _counter + "]Message received from server !");
            switch (type)
            {
                case (byte)RMT.GameStartInfo:
                    Debug.Print("A message type 'GameStartInfo' have been received from server !");
                    RecieveGameInfo(incMsg.ReadInt64());
                    break;
                case (byte)RMT.Map:
                    Debug.Print("A message type 'Map' have been received from server !");
                    RecieveMap(); //Mkt info, read from the buffer in the function
                    break;
                case (byte)RMT.StartGame:
                    Debug.Print("A message type 'StartGame' have been received from server !");
                    RecieveStartGame(incMsg);
                    break;
                case (byte)RMT.PlayerPosAndSpeed:
                    Debug.Print("A message type 'PlayerPosAndSpeed' have been received from server !");
                    RecievePositionAndSpeed(incMsg.ReadFloat(), incMsg.ReadFloat(), incMsg.ReadByte(), incMsg.ReadInt32());
                    break;
                case (byte)RMT.PlayerInfo:
                    Debug.Print("A message type 'PlayerInfo' have been received from server !");
                    RecievePlayerInfo(incMsg.ReadInt32(), incMsg.ReadFloat(), incMsg.ReadString());
                    break;
                case (byte)RMT.RemovePlayer:
                    Debug.Print("A message type 'RemovePlayer' have been received from server !");
                    RecieveRemovePlayer(incMsg.ReadInt32());
                    break;
                case (byte)RMT.PlayerPlacingBomb:
                    Debug.Print("A message type 'PlayerPlacingBomb' have been received from server !");
                    RecievePlacingBomb(incMsg.ReadInt32(), incMsg.ReadFloat(), incMsg.ReadFloat());
                    break;
                case (byte)RMT.BombExploded:
                    //RecieveBombExploded(); //Mkt info, läser från buffern i funktionen
                    break;
                case (byte)RMT.Burn:
                    //RecieveBurn(buffer.ReadInt32());
                    break;
                case (byte)RMT.ExplodeTile:
                    //RecieveExplodeTile(buffer.ReadInt32());
                    break;
                case (byte)RMT.PowerupDrop:
                    //RecievePowerupDrop((Powerup.PowerupType)buffer.ReadByte(), buffer.ReadFloat(), buffer.ReadFloat());
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
                case (byte)RMT.End:
                    Debug.Print("A message type 'End' have been received from server !");
                    RecieveEnd(incMsg.ReadBoolean());
                    break;
            }
        }
    }

}

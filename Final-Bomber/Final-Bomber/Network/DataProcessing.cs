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
        public void DataProcessing(byte type, NetIncomingMessage incMsg)
        {
            switch (type)
            {
                case (byte)RMT.GameStartInfo:
                    //RecieveGameInfo(buffer.ReadInt64());
                    break;
                case (byte)RMT.Map:
                    //RecieveMap(); //Mkt info, läser från buffern i funktionen
                    break;
                case (byte)RMT.StartGame:
                    //RecieveStartGame(buffer.ReadBoolean());
                    break;
                case (byte)RMT.PlayerPosAndSpeed:
                    Debug.Print("COUCOU");
                    RecievePositionAndSpeed(incMsg.ReadFloat(), incMsg.ReadFloat(), incMsg.ReadByte(), incMsg.ReadInt32());
                    break;
                case (byte)RMT.PlayerInfo:
                    //RecievePlayerInfo(buffer.ReadInt32(), buffer.ReadFloat(), buffer.ReadString());
                    break;
                case (byte)RMT.RemovePlayer:
                    //RecieveRemovePlayer(buffer.ReadInt32());
                    break;
                case (byte)RMT.PlayerPlacingBomb:
                    //RecievePlacingBomb(buffer.ReadInt32(), buffer.ReadFloat(), buffer.ReadFloat());
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
                    //RecieveEnd(buffer.ReadBoolean());
                    break;
            }
        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Final_BomberNetwork.MainServer
{
    partial class MainServer
    {
        void DataProcessing(byte data)
        {
            switch (data)
            {
                case (byte)RMT.LoggedIn:
                    RecieveLoggedIn(msgIn.ReadByte());
                    break;
                case (byte)RMT.CreatedAccount:
                    RecieveCreatedAccount(msgIn.ReadByte());
                    break;
                case (byte)RMT.SendHostedGames:
                    RecieveHostedGames(msgIn.ReadString());
                    break;
                case (byte)RMT.CurrentVersion:
                    RecieveCurrentVersion(msgIn.ReadString());
                    break;
                case (byte)RMT.SendStats:
                    RecieveStats();
                    break;
                case (byte)RMT.SendRanking:
                    RecieveRanking();
                    break;
            }
        }
    }
}

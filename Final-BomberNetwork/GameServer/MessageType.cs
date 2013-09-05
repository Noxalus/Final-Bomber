using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Final_BomberNetwork.GameServer
{
    partial class Server
    {
        public enum RMTClient
        {
            CreateAccount = 0,
            LogIn = 1,
            GetHostedGames = 3,
            GetStats = 4,
            GetRanking = 5,
        }

        public enum SMTClient
        {
            CreatedAccount = 0,
            LoggedIn = 1,
            CurrentVersion = 2,
            SendHostedGames = 3,
            SendStats = 4,
            SendRanking = 5,
        }

        public enum RMTServer
        {
            IsHosting = 6,
            GetCurrentPlayerAmount = 7,
            GetServerPort = 8,
            GetCheckIfOnline = 9,
            NextMap = 10,
            SendStats = 11,
        }

        public enum SMTServer
        {
            SendSettings = 6,
            CurrentVersion = 7,
            AnswerCheckIfOnline = 8,
        }
    }
}

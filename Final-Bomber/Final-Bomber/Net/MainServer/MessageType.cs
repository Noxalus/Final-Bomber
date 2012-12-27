using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Final_Bomber.Net.MainServer
{
    partial class MainServer
    {
        public enum RMT
        {
            CreatedAccount = 0,
            LoggedIn = 1,
            SendHostedGames = 3,
            CurrentVersion = 2,
            SendStats = 4,
            SendRanking = 5,
        }

        public enum SMT
        {
            CreateAccount = 0,
            LogIn = 1,
            GetHostedGames = 3,
            GetStats = 4,
            GetRanking = 5,
        }
    }
}

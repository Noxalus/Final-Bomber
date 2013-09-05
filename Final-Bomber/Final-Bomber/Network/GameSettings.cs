using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Final_Bomber.Network
{
    static class GameSettings
    {
        static public GameServer GameServer = new GameServer();

        static public Int64 currentMap = -1;
        static public string CurrentVersion = "";

        public const string THISVERSION = "5";

        static public string Username, Password;

        static public long speed;
    }
}

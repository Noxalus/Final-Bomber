using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Final_Bomber.Net;

namespace Final_Bomber
{
    static class GameSettings
    {
        static public GameServer GameServer = new GameServer();

        static public Int64 currentMap = -1;
        static public string CurrentVersion = "";

        public const string THISVERSION = "5";

        static public SpriteFont font_playerName;
        static public string Username, Password;

        //static public MapCollection Maps = new MapCollection();

        static public long speed;
    }
}

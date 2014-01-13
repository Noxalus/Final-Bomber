using FBServer.Core;
using FBServer.Core.WorldEngine;
using FBServer.Host;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBServer
{
    static class GameSettings
    {
        public static string GameName;

        public static ClientCollection PlayingClients;
        public static GameValues GameValues = new GameValues();

        public static List<Map> mapPlayList = new List<Map>();
        public static List<Map> Maps = new List<Map>();
        static int currentMap = 0;
        public static int CurrentMap
        {
            get { return currentMap; }
            set
            {
                if (value > mapPlayList.Count - 1)
                {
                    currentMap = 0;
                }
                else
                {
                    currentMap = value;
                }
            }
        }

        public static Map GetCurrentMap()
        {
            if (GameSettings.mapPlayList.Count > 0)
                return GameSettings.mapPlayList[GameSettings.currentMap];
            else
                return null;
        }

        public static int Speed;

        #region Settings
        public static bool BombsExplodeBombs = false;
        #endregion
    }

    public class GameValues
    {
        public bool RecievedValues = true;
        public PowerupValues Powerup = new PowerupValues();
        public PowerupDrops PowerupDrop = new PowerupDrops();

        public class PowerupValues
        {
            public int MovementSpeed;
            public float Tickrate;
            public int Lifes;
            public int BombRange;
            public int BombAmount;
        }

        public class PowerupDrops
        {
            public int Powerups;
            public int MovementSpeed;
            public int Tickrate;
            public int Lifes;
            public int BombRange;
            public int BombAmount;

            public void SetValues(int speed, int rate, int life, int range, int bomb)
            {
                MovementSpeed = speed;
                Tickrate = speed + rate;
                Lifes = speed + rate + life;
                BombRange = speed + rate + life + range;
                BombAmount = speed + rate + life + range + bomb;
            }
        }
    }
}

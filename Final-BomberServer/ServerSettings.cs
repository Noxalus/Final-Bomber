using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final_BomberServer
{
    public static class ServerSettings
    {
        public static int SendPlayerPositionTime = 5000; // milliseconds

        // Gameplay
        public static int WallPercentage = 80; // From 0% to 100%
        public static int ScoreToWin = 20;
    }
}
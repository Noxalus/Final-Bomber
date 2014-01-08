
namespace Final_BomberServer
{
    public static class ServerSettings
    {
        // Server config
        public const int MaxConnection = 50;
        public const int Port = 2643;

        public static int SendPlayerPositionTime = 50000; // milliseconds

        // Gameplay
        public static int WallPercentage = 0; // From 0% to 100%
        public static int ScoreToWin = 20;
    }
}
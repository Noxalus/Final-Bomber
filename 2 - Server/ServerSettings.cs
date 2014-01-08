
namespace Final_BomberServer
{
    public static class ServerSettings
    {
        // Server config
        public const int MaxConnection = 50;
        public const int Port = 2643;

        public const int SendPlayerPositionTime = 50000; // milliseconds

        // Gameplay
        public const int WallPercentage = 0; // From 0% to 100%
        public const int ScoreToWin = 20;
    }
}
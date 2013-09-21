using System;
using Microsoft.Xna.Framework;

namespace FBLibrary.Core
{
    public static class GameConfiguration
    {
        // Invincibility
        public const bool Invincible = false;
        public static TimeSpan PlayerInvincibleTimer = TimeSpan.FromSeconds(3);

        // Base characteristics
        public const int BasePlayerBombPower = 1;
        public const float BasePlayerSpeed = 1000f;
        public const float BaseBombSpeed = 3f;
        public const int BasePlayerBombAmount = 1;
        // Initially => 2
        public static TimeSpan BaseBombTimer = TimeSpan.FromSeconds(2);

        // Characteristics minimum and maximum
        public const float MaxSpeed = 300f;
        public const float MinSpeed = 1f;
        public const int MinBombPower = 1;
        public const int MinBombAmount = 1;

        // Game info
        public const float PlayerSpeedIncrementeur = 0.25f;
        public static int WallPercentage = 10; // This is a percentage => from 0% to 100%

        // World
        public static Point BaseTileSize = new Point(32, 32);

        // TO DELETE
        public static int PlayerNumber = 1;
        public static string ServerIp = "127.0.0.1";
        public static string ServerPort = "2643";
    }
}

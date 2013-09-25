﻿using System;
using System.Collections.Generic;
using FBLibrary.Core;
using Microsoft.Xna.Framework;

namespace FBLibrary
{
    public static class GameConfiguration
    {
        // Random
        public static Random Random = new Random();

        // Invincibility
        public const bool Invincible = false;
        public static TimeSpan PlayerInvincibleTimer = TimeSpan.FromSeconds(3);

        // Base characteristics
        public const int BasePlayerBombPower = 1;
        public const float BasePlayerSpeed = 150f;
        public const float BaseBombSpeed = 3f;
        public const int BasePlayerBombAmount = 1;
        // Initially => 2
        public static TimeSpan BaseBombTimer = TimeSpan.FromSeconds(2);

        // Characteristics minimum and maximum
        public const float MaxSpeed = 300f;
        public const int MaxBombPower = 30;
        public const int MaxBombAmount = 30;

        public const float MinSpeed = 1f;
        public const int MinBombPower = 1;
        public const int MinBombAmount = 1;

        // Game info
        public static float PlayerSpeedIncrementeurPercentage = 25; // Percentage of base player speed
        public static int WallPercentage = 100; // From 0% to 100%
        public static int PowerUpPercentage = 100;

        public static TimeSpan BombDestructionTime = TimeSpan.FromMilliseconds(350);
        public static TimeSpan PowerUpDestructionTime = TimeSpan.FromMilliseconds(750);
        public static TimeSpan PlayerDestructionTime = TimeSpan.FromMilliseconds(1750);

        // Power up
        public static readonly List<PowerUpType> PowerUpTypeAvailable = new List<PowerUpType>() 
        { 
            PowerUpType.Power,
            PowerUpType.Bomb,
            PowerUpType.Speed,
            //PowerUpType.BadItem,
            PowerUpType.Score 
        };

        public static readonly List<BadEffect> BadEffectList = new List<BadEffect>()
        {
            BadEffect.BombDrop,
            BadEffect.BombTimerChanged,
            BadEffect.KeysInversion,
            BadEffect.NoBomb,
            BadEffect.TooSlow,
            BadEffect.TooSpeed
        };

        // World
        public static Point BaseTileSize = new Point(32, 32);

        // Time
        public static int DeltaTime = 0; // milliseconds

        // TO DELETE
        public static int PlayerNumber = 2;
        public static string ServerIp = "localhost";
        public static string ServerPort = "2643";
    }
}
using System;
using System.Collections.Generic;
using FBLibrary.Core;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace FBClient
{   
    [Serializable]
    static class Config
    {
        // Debug
        public static bool Debug = true;

        // Taille
        public static Point MapSize = new Point(17, 17);
        public static Point MinimumMapSize = new Point(9, 9);
        public static readonly Point[] MaximumMapSize =
        {
            new Point(17, 17),
            new Point(23, 23),
            new Point(33, 29),
            new Point(53, 33)
        };

        public static int PlayersNumber = 1;
        public static bool FullScreen = false;

        public static Point[] PlayersPositions =
        {
            new Point(1, 1),
            new Point(MapSize.X - 2, MapSize.Y - 2),
            new Point(1, MapSize.Y - 2),
            new Point(MapSize.X - 2, 1),
            new Point((int)Math.Ceiling((double)(MapSize.X - 2)/(double)2), (int)Math.Ceiling((double)(MapSize.Y - 2)/2))
        };

        public static readonly Keys[][] PlayersKeys =
        {
            new[]{ Keys.Up, Keys.Down, Keys.Left, Keys.Right, Keys.RightControl }, 
            new[]{ Keys.Z, Keys.S, Keys.Q, Keys.D, Keys.LeftControl }, 
            new[]{ Keys.I, Keys.K, Keys.J, Keys.L, Keys.Space }, 
            new[]{ Keys.NumPad8, Keys.NumPad5, Keys.NumPad4, Keys.NumPad6, Keys.Enter },
            new[]{ Keys.T, Keys.G, Keys.F, Keys.H, Keys.Y }
        };

        public static readonly bool[] PlayersUsingController = { true, false, false, false, false };

        public static readonly Buttons[][] PlayersButtons =
        {
            new[]{ Buttons.LeftThumbstickUp, Buttons.LeftThumbstickDown, Buttons.LeftThumbstickLeft, Buttons.LeftThumbstickRight, Buttons.A }, 
            new[]{ Buttons.LeftThumbstickUp, Buttons.LeftThumbstickDown, Buttons.LeftThumbstickLeft, Buttons.LeftThumbstickRight, Buttons.A }, 
            new[]{ Buttons.LeftThumbstickUp, Buttons.LeftThumbstickDown, Buttons.LeftThumbstickLeft, Buttons.LeftThumbstickRight, Buttons.A }, 
            new[]{ Buttons.LeftThumbstickUp, Buttons.LeftThumbstickDown, Buttons.LeftThumbstickLeft, Buttons.LeftThumbstickRight, Buttons.A }, 
            new[]{ Buttons.LeftThumbstickUp, Buttons.LeftThumbstickDown, Buttons.LeftThumbstickLeft, Buttons.LeftThumbstickRight, Buttons.A }
        };

        public const bool IsThereAIPlayer = true;
        public static readonly bool[] AIPlayers = new bool[] { false, true, true, true, true };

        // Joueur
        public static Color[] PlayersColor = new Color[] { Color.White, Color.White, Color.White, Color.White };
        
        // Characteristics minimum and maximum
        public const float MaxSpeed = 30000f;
        public const float MinSpeed = 1f;
        public static readonly int MaxBombPower = (MapSize.X + MapSize.Y) / 2;
        public const int MinBombPower = 1;
        public static readonly int MaxBombNumber = (MapSize.X * MapSize.Y) / 2;
        public const int MinBombNumber = 1;

        public const float PlayerSpeedIncrementeur = 0.25f;
        public const float BombSpeedIncrementeur = 1f;

        public const bool PlayerCanPush = false;
        public static bool PlayerCanLaunch = false;

        public static bool DisplayName = true;
        public static readonly string[] PlayersName = new string[] { "Martial", "Noxalus", "Litarium", "Klorius", "Oxilium" };

        
        public static int ItemNumber = 50; // This is a percentage => from 0% to 100%
        public static int ItemTypeNumber = 5;
        public const int TeleporterNumber = 0;
        public const int ArrowNumber = 0;

        public static bool ActiveTeleporters = false;
        public const TeleporterPositionTypeEnum TeleporterPositionType = TeleporterPositionTypeEnum.PlusForm;

        public static bool ActiveArrows = false;
        public static ArrowPositionTypeEnum ArrowPositionType = ArrowPositionTypeEnum.SquareForm;
        
        public static readonly LookDirection[] ArrowLookDirection = new LookDirection[] 
        { LookDirection.Down, LookDirection.Left, LookDirection.Right, LookDirection.Up };

        // Invincibility
        public const float InvincibleBlinkFrequency = 0.5f;

        // Bombs
        public static bool BombCollision = true;
        public static bool ExplosionThroughWall = true;

        public static readonly PowerUpType[] ItemTypeArray = new PowerUpType[]
        {
            PowerUpType.Power,
            PowerUpType.Bomb,
            PowerUpType.Speed,
            PowerUpType.BadEffect,
            PowerUpType.Score
        };
        // 0 => Power, 1 => Bomb, 2 => Speed, 3 => Bad item, 4 => Point
        public static readonly Dictionary<PowerUpType, int> ItemTypeIndex = new Dictionary<PowerUpType, int>
        {
            {PowerUpType.Power, 0},
            {PowerUpType.Bomb, 1},
            {PowerUpType.Speed, 2},
            {PowerUpType.BadEffect, 3},
            {PowerUpType.Score, 4}
        };

        // Volume
        public static float MusicVolume = 0.1f; // 0 to 1 (100%)
        public static float SoundVolume = 0.01f; // 0 to 1 (100%)

        // IA
        public static bool InactiveAI = false;

        // Resolution
        public static readonly int[,] Resolutions = new int[,] { { 800, 600 }, { 1024, 768 }, { 1280, 1024 }, { 1920, 1080 } };
        public static int IndexResolution = 0;

        public static int BonusTypeNumber = 5;
        public static bool ExitGame = false;

        

        public static readonly Dictionary<SuddenDeathTypeEnum, string> SuddenDeathTypeText = new Dictionary<SuddenDeathTypeEnum, string>
        {
            { SuddenDeathTypeEnum.BombAndWall, "Bombes et murs" },
            { SuddenDeathTypeEnum.OnlyBomb, "Bombes" },
            { SuddenDeathTypeEnum.OnlyWall, "Murs" },
            { SuddenDeathTypeEnum.Whole, "Trous" }
        };
        public static readonly SuddenDeathTypeEnum[] SuddenDeathTypeArray = new SuddenDeathTypeEnum[]
        {
            SuddenDeathTypeEnum.BombAndWall,
            SuddenDeathTypeEnum.OnlyBomb,
            SuddenDeathTypeEnum.OnlyWall,
            SuddenDeathTypeEnum.Whole         
        };

        // Scores
        public static readonly int[] PlayersScores = new int[5];

        // Map moving scale
        public const float MapMovingScale = 10f;

        // HUD
        public const int HUDPlayerInfoSpace = 105;

        // Keyboarding
        public const float TextCursorBlinkFrequency = 0.5f;
    }
}
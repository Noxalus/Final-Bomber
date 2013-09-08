using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Xml.Serialization;
using System.IO;
using Final_Bomber.Entities;

namespace Final_Bomber
{
    public enum SuddenDeathTypeEnum { OnlyWall, OnlyBomb, BombAndWall, Whole };
    public enum TeleporterPositionTypeEnum { Randomly, PlusForm };
    public enum ArrowPositionTypeEnum { Randomly, SquareForm };
    
    [Serializable]
    static class Config
    {
        // Debug
        public static bool Debug = true;

        // Taille
        public static Point MapSize = new Point(17, 17);
        public static Point MinimumMapSize = new Point(9, 9);
        public static readonly Point[] MaximumMapSize = new Point[]
        {
            new Point(17, 17),
            new Point(23, 23),
            new Point(33, 29),
            new Point(53, 33)
        };


        public static int PlayersNumber = 1;
        public static bool FullScreen = false;

        public static Point[] PlayersPositions = new Point[]
        {
            new Point(1, 1),
            new Point(MapSize.X - 2, MapSize.Y - 2),
            new Point(1, MapSize.Y - 2),
            new Point(MapSize.X - 2, 1),
            new Point((int)Math.Ceiling((double)(MapSize.X - 2)/(double)2), (int)Math.Ceiling((double)(MapSize.Y - 2)/(double)2))
        };

        public static readonly Keys[][] PlayersKeys = new Keys[][]
        {
             new Keys[]{ Keys.Up, Keys.Down, Keys.Left, Keys.Right, Keys.RightControl }, 
             new Keys[]{ Keys.Z, Keys.S, Keys.Q, Keys.D, Keys.LeftControl }, 
             new Keys[]{ Keys.I, Keys.K, Keys.J, Keys.L, Keys.Space }, 
             new Keys[]{ Keys.NumPad8, Keys.NumPad5, Keys.NumPad4, Keys.NumPad6, Keys.Enter },
             new Keys[]{ Keys.T, Keys.G, Keys.F, Keys.H, Keys.Y }
        };

        public static readonly bool[] PlayersUsingController = new bool[] { true, false, false, false, false };

        public static readonly Buttons[][] PlayersButtons = new Buttons[][]
        {
             new Buttons[]{ Buttons.LeftThumbstickUp, Buttons.LeftThumbstickDown, Buttons.LeftThumbstickLeft, Buttons.LeftThumbstickRight, Buttons.A }, 
             new Buttons[]{ Buttons.LeftThumbstickUp, Buttons.LeftThumbstickDown, Buttons.LeftThumbstickLeft, Buttons.LeftThumbstickRight, Buttons.A }, 
             new Buttons[]{ Buttons.LeftThumbstickUp, Buttons.LeftThumbstickDown, Buttons.LeftThumbstickLeft, Buttons.LeftThumbstickRight, Buttons.A }, 
             new Buttons[]{ Buttons.LeftThumbstickUp, Buttons.LeftThumbstickDown, Buttons.LeftThumbstickLeft, Buttons.LeftThumbstickRight, Buttons.A }, 
             new Buttons[]{ Buttons.LeftThumbstickUp, Buttons.LeftThumbstickDown, Buttons.LeftThumbstickLeft, Buttons.LeftThumbstickRight, Buttons.A }
        };

        public const bool IsThereAIPlayer = true;
        public static readonly bool[] AIPlayers = new bool[] { false, true, true, true, true };

        // Joueur
        public static Color[] PlayersColor = new Color[] { Color.White, Color.White, Color.White, Color.White };

        // Base characteristics
        public const int BasePlayerBombPower = 1;
        public const float BasePlayerSpeed = 2.5f;
        public const float BaseBombSpeed = 3f;
        public const int BasePlayerBombNumber = 1;

        // Characteristics minimum and maximum
        public const float MaxSpeed = 30f;
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

        public static int WallNumber = 100; // This is a percentage => from 0% to 100%
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
        public const bool Invincible = false;
        public static TimeSpan PlayerInvincibleTimer = TimeSpan.FromSeconds(3);
        public const float InvincibleBlinkFrequency = 0.5f;

        // Bombs
        public static bool BombCollision = true;
        public static bool ExplosionThroughWall = true;
        public const int BombLatency = 10; // Bomb's animation's frames par second
        // Initially => 2
        public static TimeSpan BombTimer = TimeSpan.FromSeconds(2);

        // Item
        public static readonly List<ItemType> ItemTypeAvaible = new List<ItemType>() 
        { 
            ItemType.Power,
            ItemType.Bomb,
            ItemType.Speed,
            //ItemType.BadItem,
            ItemType.Point 
        };
        public static readonly ItemType[] ItemTypeArray = new ItemType[]
        {
            ItemType.Power,
            ItemType.Bomb,
            ItemType.Speed,
            ItemType.BadItem,
            ItemType.Point
        };
        // 0 => Power, 1 => Bomb, 2 => Speed, 3 => Bad item, 4 => Point
        public static readonly Dictionary<ItemType, int> ItemTypeIndex = new Dictionary<ItemType, int>
        {
            {ItemType.Power, 0},
            {ItemType.Bomb, 1},
            {ItemType.Speed, 2},
            {ItemType.BadItem, 3},
            {ItemType.Point, 4}
        };

        // Volume
        public static float Volume = 0.0f;

        // IA
        public static bool InactiveAI = false;

        // Resolution
        public static readonly int[,] Resolutions = new int[,] { { 800, 600 }, { 1024, 768 }, { 1280, 1024 }, { 1920, 1080 } };
        public static int IndexResolution = 0;

        public static int BonusTypeNumber = 5;
        public static bool ExitGame = false;

        // Sudden Death
        public static bool ActiveSuddenDeath = true;
        public static TimeSpan SuddenDeathTimer = TimeSpan.FromSeconds(5);
        public static SuddenDeathTypeEnum SuddenDeathType = SuddenDeathTypeEnum.OnlyWall;
        public static float SuddenDeathCounterBombs = 0.3f;
        public static float SuddenDeathCounterWalls = 0.3f;
        public static float SuddenDeathWallSpeed = (float)Math.Round(0.25f, 2);
        public const float SuddenDeathMaxWallSpeed = 1f;

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

        // Bad Item
        public const int BadItemTimerMin = 10; // Seconds
        public const int BadItemTimerMax = 30; // Seconds
        public const int BadItemTimerChangedMin = 3; // Seconds
        public const int BadItemTimerChangedMax = 8; // Seconds

        public static readonly List<BadItemEffect> BadItemEffectList = new List<BadItemEffect>()
        {
            BadItemEffect.BombDrop,
            BadItemEffect.BombTimerChanged,
            BadItemEffect.KeysInversion,
            BadItemEffect.NoBomb,
            BadItemEffect.TooSlow,
            BadItemEffect.TooSpeed
        };

        // HUD
        public const int HUDPlayerInfoSpace = 105;

        // Keyboarding
        public const float TextCursorBlinkFrequency = 0.5f;
    }
}
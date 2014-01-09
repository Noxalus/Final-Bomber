
using FBClient.Controls;
using FBClient.Core;
using FBClient.GUI;
using FBClient.WorldEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FBClient.Screens.GameScreens
{
    public abstract class BaseGamePlayScreen : BaseGameState
    {
        // Game manager
        public static GameManager GameManager;

        // HUD
        protected Texture2D ItemInfoIcon;
        protected Texture2D Cross;
        protected SpriteFont GameFont;
        protected SpriteFont SmallFont;
        protected Point HudOrigin;
        protected int HudTopSpace;
        protected int HudMarginLeft;
        protected Texture2D BadItemTimerBar;

        // Window box
        protected Texture2D WindowSkin;
        protected WindowBox ScoresWindowBox;
        protected WindowBox TimerWindowBox;

        // Camera
        protected Camera2D Camera;

        protected BaseGamePlayScreen(Game game, GameStateManager manager) : base(game, manager)
        {
            GameManager = new GameManager();
        }

        protected override void LoadContent()
        {
            // Pictures      
            ItemInfoIcon = FinalBomber.Instance.Content.Load<Texture2D>("Graphics/Pictures/ItemInfo");
            Cross = FinalBomber.Instance.Content.Load<Texture2D>("Graphics/Pictures/Cross");
            BadItemTimerBar = FinalBomber.Instance.Content.Load<Texture2D>("Graphics/Pictures/BadItemTimerCross");
            WindowSkin = FinalBomber.Instance.Content.Load<Texture2D>("Graphics/Windowskins/Windowskin1");

            // Fonts
            GameFont = FinalBomber.Instance.Content.Load<SpriteFont>("Graphics/Fonts/GameFont");
            SmallFont = FinalBomber.Instance.Content.Load<SpriteFont>("Graphics/Fonts/SmallFont");

            GameManager.LoadContent();

            base.LoadContent();
        }
    }
}

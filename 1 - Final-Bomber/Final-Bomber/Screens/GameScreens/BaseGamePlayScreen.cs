
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
        // Background
        private Rectangle _screenRectangle;
        private Texture2D _backgroundTile;

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
        
        protected BaseGamePlayScreen(Game game, GameStateManager manager) : base(game, manager)
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            _screenRectangle = new Rectangle(0, 0, Config.Resolutions[Config.IndexResolution, 0], Config.Resolutions[Config.IndexResolution, 1]);
        }

        protected override void LoadContent()
        {
            // Background
            _backgroundTile = FinalBomber.Instance.Content.Load<Texture2D>("Graphics/Sprites/floor");

            // Pictures      
            ItemInfoIcon = FinalBomber.Instance.Content.Load<Texture2D>("Graphics/Pictures/ItemInfo");
            Cross = FinalBomber.Instance.Content.Load<Texture2D>("Graphics/Pictures/Cross");
            BadItemTimerBar = FinalBomber.Instance.Content.Load<Texture2D>("Graphics/Pictures/BadItemTimerCross");
            WindowSkin = FinalBomber.Instance.Content.Load<Texture2D>("Graphics/Windowskins/Windowskin1");

            // Fonts
            GameFont = FinalBomber.Instance.Content.Load<SpriteFont>("Graphics/Fonts/GameFont");
            SmallFont = FinalBomber.Instance.Content.Load<SpriteFont>("Graphics/Fonts/SmallFont");

            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            // Draw the background with repeated tiles
            FinalBomber.Instance.SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.Opaque, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullNone);

            FinalBomber.Instance.SpriteBatch.Draw(_backgroundTile, Vector2.Zero, _screenRectangle, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

            FinalBomber.Instance.SpriteBatch.End();
        }
    }
}

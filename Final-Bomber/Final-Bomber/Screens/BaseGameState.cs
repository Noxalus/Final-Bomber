using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Final_Bomber.Controls;

namespace Final_Bomber.Screens
{
    public abstract partial class BaseGameState : GameState
    {
        #region Fields region

        protected FinalBomber GameRef;

        protected ControlManager ControlManager;

        protected SpriteFont BigFont;
        
        protected PlayerIndex playerIndexInControl;

        #endregion

        #region Properties region
        #endregion

        #region Constructor Region

        public BaseGameState(Game game, GameStateManager manager)
            : base(game, manager)
        {
            GameRef = (FinalBomber)game;

            playerIndexInControl = PlayerIndex.One;
        }

        #endregion

        #region XNA Method Region

        protected override void LoadContent()
        {
            ContentManager Content = Game.Content;

            SpriteFont menuFont = Content.Load<SpriteFont>(@"Graphics\Fonts\ControlFont");
            BigFont = Content.Load<SpriteFont>(@"Graphics\Fonts\BigFont");
            ControlManager = new ControlManager(menuFont);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        #endregion

        #region Method Region
        #endregion
    }
}

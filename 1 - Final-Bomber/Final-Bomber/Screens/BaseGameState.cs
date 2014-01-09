using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using FBClient.Controls;

namespace FBClient.Screens
{
    public abstract partial class BaseGameState : GameState
    {
        #region Fields region

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
            FinalBomber.Instance = (FinalBomber)game;

            playerIndexInControl = PlayerIndex.One;
        }

        #endregion

        #region XNA Method Region

        protected override void LoadContent()
        {
            ContentManager content = Game.Content;

            var menuFont = content.Load<SpriteFont>(@"Graphics\Fonts\ControlFont");
            BigFont = content.Load<SpriteFont>(@"Graphics\Fonts\BigFont");
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

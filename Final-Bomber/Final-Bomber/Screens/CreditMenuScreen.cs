using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Final_Bomber.Components;
using Final_Bomber.Controls;
using Microsoft.Xna.Framework.Input;

namespace Final_Bomber.Screens
{
    public class CreditMenuScreen : BaseGameState
    {
        #region Constructor Region

        public CreditMenuScreen(Game game, GameStateManager manager)
            : base(game, manager)
        {
        }

        #endregion

        #region XNA Method Region

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            ControlManager.Update(gameTime, PlayerIndex.One);

            if (InputHandler.KeyDown(Keys.Escape))
                StateManager.PushState(GameRef.TitleScreen);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GameRef.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, Matrix.Identity);

            base.Draw(gameTime);

            ControlManager.Draw(GameRef.SpriteBatch);

            string s = "Game created Noxalus";
            GameRef.SpriteBatch.DrawString(BigFont, s,
                new Vector2(GameRef.GraphicsDevice.Viewport.Width / 2 - BigFont.MeasureString(s).X / 2,
                    GameRef.GraphicsDevice.Viewport.Height / 2 - BigFont.MeasureString(s).Y / 2), Color.Black);

            GameRef.SpriteBatch.End();
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Final_Bomber.Entities;
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
                StateManager.PushState(FinalBomber.Instance.TitleScreen);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            FinalBomber.Instance.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, Matrix.Identity);

            base.Draw(gameTime);

            ControlManager.Draw(FinalBomber.Instance.SpriteBatch);

            string s = "Game created Noxalus";
            FinalBomber.Instance.SpriteBatch.DrawString(BigFont, s,
                new Vector2(FinalBomber.Instance.GraphicsDevice.Viewport.Width / 2 - BigFont.MeasureString(s).X / 2,
                    FinalBomber.Instance.GraphicsDevice.Viewport.Height / 2 - BigFont.MeasureString(s).Y / 2), Color.Black);

            FinalBomber.Instance.SpriteBatch.End();
        }

        #endregion
    }
}

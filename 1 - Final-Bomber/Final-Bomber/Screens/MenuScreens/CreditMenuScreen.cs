using FBClient.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace FBClient.Screens.MenuScreens
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
            MediaPlayer.Play(FinalBomber.Instance.Content.Load<Song>("Audio/Musics/credits"));

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

            var messages = new[] { "Game programmed by Noxalus", "", "The rights of graphics and sound resources", "belong to Hudson Soft Company,", " developers of the", " Bomberman game series." };
            int i = 0;
            foreach (var s in messages)
            {
                FinalBomber.Instance.SpriteBatch.DrawString(BigFont, s,
                    new Vector2(
                        FinalBomber.Instance.GraphicsDevice.Viewport.Width/2f - BigFont.MeasureString(s).X/2,
                        FinalBomber.Instance.GraphicsDevice.Viewport.Height/2f - BigFont.MeasureString(s).Y/2 - 200 + (i*60)),
                    Color.Black);

                i++;
            }

            FinalBomber.Instance.SpriteBatch.End();
        }

        #endregion
    }
}

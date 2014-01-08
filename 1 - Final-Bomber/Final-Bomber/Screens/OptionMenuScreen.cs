using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Final_Bomber.Entities;
using Final_Bomber.Controls;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Final_Bomber.Screens
{
    public class OptionMenuScreen : BaseMenuScreen
    {

        #region Constructor Region

        public OptionMenuScreen(Game game, GameStateManager manager)
            : base(game, manager)
        {
        }

        #endregion

        #region XNA Method Region

        public override void Initialize()
        {
            MenuString = new string[] { "Changer les touches", "Résolution", "Plein écran", "Retour" };

            MediaPlayer.Play(FinalBomber.Instance.Content.Load<Song>("Audio/Musics/option"));

            base.Initialize();

            MenuPosition.X /= 2f;
        }
        

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            ControlManager.Update(gameTime, PlayerIndex.One);

            if (InputHandler.KeyDown(Keys.Escape))
            {
                StateManager.PushState(FinalBomber.Instance.TitleScreen);
                FinalBomber.Instance.graphics.ApplyChanges();
            }

            switch (IndexMenu)
            {
                case 0:
                    if(InputHandler.KeyPressed(Keys.Enter))
                        StateManager.ChangeState(FinalBomber.Instance.KeysMenuScreen);
                    break;
                case 1:
                    bool changes = false;
                    if (InputHandler.KeyPressed(Keys.Right))
                    {
                        if (Config.IndexResolution >= Config.Resolutions.Length/2 - 1)
                            Config.IndexResolution = 0;
                        else
                            Config.IndexResolution++;
                        changes = true;
                    }
                    else if (InputHandler.KeyPressed(Keys.Enter) || InputHandler.KeyPressed(Keys.Left))
                    {
                        if (Config.IndexResolution <= 0)
                            Config.IndexResolution = Config.Resolutions.Length/2 - 1;
                        else
                            Config.IndexResolution--;
                        changes = true;
                    }

                    if (changes)
                    {
                        FinalBomber.Instance.graphics.PreferredBackBufferWidth = Config.Resolutions[Config.IndexResolution, 0];
                        FinalBomber.Instance.graphics.PreferredBackBufferHeight = Config.Resolutions[Config.IndexResolution, 1];

                        FinalBomber.Instance.ScreenRectangle = new Rectangle(0, 0, Config.Resolutions[Config.IndexResolution, 0], 
                            Config.Resolutions[Config.IndexResolution, 1]);
                    }
                    break;
                case 2:
                    if (InputHandler.KeyPressed(Keys.Enter) ||
                        InputHandler.KeyPressed(Keys.Left) ||
                        InputHandler.KeyPressed(Keys.Right))
                    {
                        Config.FullScreen = !Config.FullScreen;
                        FinalBomber.Instance.graphics.IsFullScreen = Config.FullScreen;
                    }
                    break;
                case 3:
                    if (InputHandler.KeyPressed(Keys.Enter))
                    {
                        StateManager.ChangeState(FinalBomber.Instance.TitleScreen);
                        FinalBomber.Instance.graphics.ApplyChanges();
                    }
                    break;
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            FinalBomber.Instance.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, Matrix.Identity);

            base.Draw(gameTime);

            ControlManager.Draw(FinalBomber.Instance.SpriteBatch);

            for (int i = 0; i < MenuString.Length; i++)
            {
                Color textColor = Color.Black;
                if (i == IndexMenu)
                    textColor = Color.Green;

                FinalBomber.Instance.SpriteBatch.DrawString(this.BigFont, MenuString[i],
                    new Vector2(MenuPosition.X, MenuPosition.Y + (this.BigFont.MeasureString(MenuString[i]).Y) * i), textColor);

                if (i == 1)
                {
                    FinalBomber.Instance.SpriteBatch.DrawString(this.BigFont, ": " + 
                        Config.Resolutions[Config.IndexResolution, 0] + "x" + Config.Resolutions[Config.IndexResolution, 1],
                    new Vector2(MenuPosition.X + this.BigFont.MeasureString(MenuString[i]).X, MenuPosition.Y + (this.BigFont.MeasureString(MenuString[i]).Y) * i), 
                    textColor);
                }
                else if (i == 2)
                {
                    FinalBomber.Instance.SpriteBatch.DrawString(this.BigFont, ": " + Config.FullScreen,
                    new Vector2(MenuPosition.X + this.BigFont.MeasureString(MenuString[i]).X, MenuPosition.Y + (this.BigFont.MeasureString(MenuString[i]).Y) * i),
                    textColor);
                }
            }

            FinalBomber.Instance.SpriteBatch.End();
        }

        #endregion
    }
}

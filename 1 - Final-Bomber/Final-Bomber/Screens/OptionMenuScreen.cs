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
    public class OptionMenuScreen : BaseGameState
    {
        #region Field Region
        string[] menuString;
        int indexMenu;
        Vector2 menuPosition;
        #endregion

        #region Property Region

        #endregion

        #region Constructor Region

        public OptionMenuScreen(Game game, GameStateManager manager)
            : base(game, manager)
        {
        }

        #endregion

        #region XNA Method Region

        public override void Initialize()
        {
            menuString = new string[] { "Changer les touches", "Résolution", "Plein écran", "Retour" };
            indexMenu = 0;
            menuPosition = new Vector2(FinalBomber.Instance.GraphicsDevice.Viewport.Width / 4, FinalBomber.Instance.GraphicsDevice.Viewport.Height / 2);
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
            {
                StateManager.PushState(FinalBomber.Instance.TitleScreen);
                FinalBomber.Instance.graphics.ApplyChanges();
            }

            switch (indexMenu)
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

            if (InputHandler.KeyPressed(Keys.Up))
            {
                if (indexMenu <= 0)
                    indexMenu = menuString.Length - 1;
                else
                    indexMenu--;
            }
            else if (InputHandler.KeyPressed(Keys.Down))
            {
                indexMenu = (indexMenu + 1) % menuString.Length;
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            FinalBomber.Instance.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, Matrix.Identity);

            base.Draw(gameTime);

            ControlManager.Draw(FinalBomber.Instance.SpriteBatch);

            for (int i = 0; i < menuString.Length; i++)
            {
                Color textColor = Color.Black;
                if (i == indexMenu)
                    textColor = Color.Green;

                FinalBomber.Instance.SpriteBatch.DrawString(this.BigFont, menuString[i],
                    new Vector2(menuPosition.X, menuPosition.Y + (this.BigFont.MeasureString(menuString[i]).Y) * i), textColor);

                if (i == 1)
                {
                    FinalBomber.Instance.SpriteBatch.DrawString(this.BigFont, ": " + 
                        Config.Resolutions[Config.IndexResolution, 0] + "x" + Config.Resolutions[Config.IndexResolution, 1],
                    new Vector2(menuPosition.X + this.BigFont.MeasureString(menuString[i]).X, menuPosition.Y + (this.BigFont.MeasureString(menuString[i]).Y) * i), 
                    textColor);
                }
                else if (i == 2)
                {
                    FinalBomber.Instance.SpriteBatch.DrawString(this.BigFont, ": " + Config.FullScreen,
                    new Vector2(menuPosition.X + this.BigFont.MeasureString(menuString[i]).X, menuPosition.Y + (this.BigFont.MeasureString(menuString[i]).Y) * i),
                    textColor);
                }
            }

            FinalBomber.Instance.SpriteBatch.End();
        }

        #endregion

        #region Abstract Method Region
        #endregion

        #region Method Region
        #endregion
    }
}

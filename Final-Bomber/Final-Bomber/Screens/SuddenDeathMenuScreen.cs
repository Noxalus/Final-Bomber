using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FBLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Final_Bomber.Entities;
using Final_Bomber.Controls;
using Microsoft.Xna.Framework.Input;

namespace Final_Bomber.Screens
{
    public class SuddenDeathMenuScreen : BaseGameState
    {
        #region Field Region
        string[] menuString;
        int indexMenu;
        Vector2 menuPosition;
        int suddenDeathTypeIndex;
        #endregion

        #region Property Region

        #endregion

        #region Constructor Region

        public SuddenDeathMenuScreen(Game game, GameStateManager manager)
            : base(game, manager)
        {
        }

        #endregion

        #region XNA Method Region

        public override void Initialize()
        {
            menuString = new string[] { "Active", "Déclenchement", "Vitesse", "Type", "Retour" };
            indexMenu = 0;
            menuPosition = new Vector2(FinalBomber.Instance.GraphicsDevice.Viewport.Width / 4, FinalBomber.Instance.GraphicsDevice.Viewport.Height / 2);
            suddenDeathTypeIndex = 2;
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
                StateManager.PushState(FinalBomber.Instance.BattleMenuScreen);

            switch (indexMenu)
            {
                case 0:
                    if (InputHandler.KeyPressed(Keys.Left) || InputHandler.KeyPressed(Keys.Right) || InputHandler.KeyPressed(Keys.Enter))
                        GameConfiguration.ActiveSuddenDeath = !GameConfiguration.ActiveSuddenDeath;
                    break;
                case 1:
                    if (InputHandler.KeyDown(Keys.Left))
                    {
                        if (GameConfiguration.SuddenDeathTimer <= TimeSpan.Zero)
                            GameConfiguration.SuddenDeathTimer = TimeSpan.Zero;
                        else
                            GameConfiguration.SuddenDeathTimer -= TimeSpan.FromSeconds(1);
                    }
                    else if (InputHandler.KeyDown(Keys.Right))
                    {
                        GameConfiguration.SuddenDeathTimer += TimeSpan.FromSeconds(1);
                    }
                    break;
                case 2:
                    if (InputHandler.KeyDown(Keys.Right))
                    {
                        if (GameConfiguration.SuddenDeathWallSpeed <= 0.1f)
                            GameConfiguration.SuddenDeathWallSpeed = 0.1f;
                        else
                            GameConfiguration.SuddenDeathWallSpeed -= 0.01f;
                    }
                    else if (InputHandler.KeyDown(Keys.Left))
                    {
                        if (GameConfiguration.SuddenDeathWallSpeed >= GameConfiguration.SuddenDeathMaxWallSpeed)
                            GameConfiguration.SuddenDeathWallSpeed = GameConfiguration.SuddenDeathMaxWallSpeed;
                        else
                            GameConfiguration.SuddenDeathWallSpeed += 0.01f;
                    }
                    GameConfiguration.SuddenDeathWallSpeed = (float)Math.Round(GameConfiguration.SuddenDeathWallSpeed, 2);
                    break;
                case 3:
                    if (InputHandler.KeyPressed(Keys.Left))
                    {
                        if (suddenDeathTypeIndex <= 0)
                            suddenDeathTypeIndex = Config.SuddenDeathTypeArray.Length - 1;
                        else
                            suddenDeathTypeIndex--;
                    }
                    else if (InputHandler.KeyPressed(Keys.Right))
                    {
                        suddenDeathTypeIndex = (suddenDeathTypeIndex + 1) % Config.SuddenDeathTypeArray.Length;
                    }
                    GameConfiguration.SuddenDeathType = Config.SuddenDeathTypeArray[suddenDeathTypeIndex];
                    break;
                case 4:
                    if (InputHandler.KeyPressed(Keys.Enter))
                        StateManager.ChangeState(FinalBomber.Instance.BattleMenuScreen);
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

                if (i != 4)
                {
                    FinalBomber.Instance.SpriteBatch.DrawString(this.BigFont, ": ",
                        new Vector2(menuPosition.X + this.BigFont.MeasureString(menuString[i]).X,
                            menuPosition.Y + this.BigFont.MeasureString(menuString[i]).Y * i), Color.Black);

                    if (i == 0)
                    {
                        FinalBomber.Instance.SpriteBatch.DrawString(this.BigFont, GameConfiguration.ActiveSuddenDeath.ToString(),
                        new Vector2(menuPosition.X + this.BigFont.MeasureString(menuString[i] + ": ").X,
                            menuPosition.Y + this.BigFont.MeasureString(menuString[i]).Y * i), Color.Black);
                    }
                    else if (i == 1)
                    {
                        FinalBomber.Instance.SpriteBatch.DrawString(this.BigFont, GameConfiguration.SuddenDeathTimer.ToString(),
                        new Vector2(menuPosition.X + this.BigFont.MeasureString(menuString[i] + ": ").X,
                            menuPosition.Y + this.BigFont.MeasureString(menuString[i]).Y * i), Color.Black);
                    }
                    else if (i == 2)
                    {
                        FinalBomber.Instance.SpriteBatch.DrawString(this.BigFont, (100 - (GameConfiguration.SuddenDeathWallSpeed - 0.1f) * 100).ToString() + "%",
                        new Vector2(menuPosition.X + this.BigFont.MeasureString(menuString[i] + ": ").X,
                            menuPosition.Y + this.BigFont.MeasureString(menuString[i]).Y * i), Color.Black);
                    }
                    else if (i == 3)
                    {
                        FinalBomber.Instance.SpriteBatch.DrawString(this.BigFont, Config.SuddenDeathTypeText[GameConfiguration.SuddenDeathType],
                        new Vector2(menuPosition.X + this.BigFont.MeasureString(menuString[i] + ": ").X,
                            menuPosition.Y + this.BigFont.MeasureString(menuString[i]).Y * i), Color.Black);
                    }
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

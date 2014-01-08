using System;
using FBLibrary;
using Final_Bomber.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Final_Bomber.Screens.MenuScreens
{
    public class SuddenDeathMenuScreen : BaseMenuScreen
    {
        #region Field Region
        int _suddenDeathTypeIndex;
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
            MenuString = new string[] { "Active", "Déclenchement", "Vitesse", "Type", "Retour" };
            _suddenDeathTypeIndex = 2;

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
                StateManager.PushState(FinalBomber.Instance.BattleMenuScreen);

            switch (IndexMenu)
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
                        if (_suddenDeathTypeIndex <= 0)
                            _suddenDeathTypeIndex = Config.SuddenDeathTypeArray.Length - 1;
                        else
                            _suddenDeathTypeIndex--;
                    }
                    else if (InputHandler.KeyPressed(Keys.Right))
                    {
                        _suddenDeathTypeIndex = (_suddenDeathTypeIndex + 1) % Config.SuddenDeathTypeArray.Length;
                    }
                    GameConfiguration.SuddenDeathType = Config.SuddenDeathTypeArray[_suddenDeathTypeIndex];
                    break;
                case 4:
                    if (InputHandler.KeyPressed(Keys.Enter))
                        StateManager.ChangeState(FinalBomber.Instance.BattleMenuScreen);
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

                if (i != 4)
                {
                    FinalBomber.Instance.SpriteBatch.DrawString(this.BigFont, ": ",
                        new Vector2(MenuPosition.X + this.BigFont.MeasureString(MenuString[i]).X,
                            MenuPosition.Y + this.BigFont.MeasureString(MenuString[i]).Y * i), Color.Black);

                    if (i == 0)
                    {
                        FinalBomber.Instance.SpriteBatch.DrawString(this.BigFont, GameConfiguration.ActiveSuddenDeath.ToString(),
                        new Vector2(MenuPosition.X + this.BigFont.MeasureString(MenuString[i] + ": ").X,
                            MenuPosition.Y + this.BigFont.MeasureString(MenuString[i]).Y * i), Color.Black);
                    }
                    else if (i == 1)
                    {
                        FinalBomber.Instance.SpriteBatch.DrawString(this.BigFont, GameConfiguration.SuddenDeathTimer.ToString(),
                        new Vector2(MenuPosition.X + this.BigFont.MeasureString(MenuString[i] + ": ").X,
                            MenuPosition.Y + this.BigFont.MeasureString(MenuString[i]).Y * i), Color.Black);
                    }
                    else if (i == 2)
                    {
                        FinalBomber.Instance.SpriteBatch.DrawString(this.BigFont, (100 - (GameConfiguration.SuddenDeathWallSpeed - 0.1f) * 100).ToString() + "%",
                        new Vector2(MenuPosition.X + this.BigFont.MeasureString(MenuString[i] + ": ").X,
                            MenuPosition.Y + this.BigFont.MeasureString(MenuString[i]).Y * i), Color.Black);
                    }
                    else if (i == 3)
                    {
                        FinalBomber.Instance.SpriteBatch.DrawString(this.BigFont, Config.SuddenDeathTypeText[GameConfiguration.SuddenDeathType],
                        new Vector2(MenuPosition.X + this.BigFont.MeasureString(MenuString[i] + ": ").X,
                            MenuPosition.Y + this.BigFont.MeasureString(MenuString[i]).Y * i), Color.Black);
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

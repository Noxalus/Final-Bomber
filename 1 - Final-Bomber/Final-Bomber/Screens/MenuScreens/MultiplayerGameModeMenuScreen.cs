﻿using FBClient.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace FBClient.Screens.MenuScreens
{
    public class MultiplayerGameModeMenuScreen : BaseMenuScreen
    {
        #region Constructor region
        public MultiplayerGameModeMenuScreen(Game game, GameStateManager manager)
            : base(game, manager)
        {
            MenuString = new string[] { "Local game", "Online game", "Back" };
        }
        #endregion

        #region XNA Method region

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

            if (InputHandler.KeyPressed(Keys.Enter))
            {
                switch (IndexMenu)
                {
                    case 0:
                        StateManager.ChangeState(FinalBomber.Instance.BattleMenuScreen);
                        break;
                    case 1:
                        StateManager.ChangeState(FinalBomber.Instance.OnlineGameMenuScreen);
                        break;
                    case 2:
                        StateManager.ChangeState(FinalBomber.Instance.TitleScreen);
                        break;
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            FinalBomber.Instance.SpriteBatch.Begin();

            base.Draw(gameTime);

            ControlManager.Draw(FinalBomber.Instance.SpriteBatch);

            for (int i = 0; i < MenuString.Length; i++)
            {
                Color textColor = Color.Black;
                if (i == IndexMenu)
                    textColor = Color.Green;

                FinalBomber.Instance.SpriteBatch.DrawString(this.BigFont, MenuString[i],
                    new Vector2(MenuPosition.X - this.BigFont.MeasureString(MenuString[i]).X / 2,
                        MenuPosition.Y + this.BigFont.MeasureString(MenuString[i]).Y * i - this.BigFont.MeasureString(MenuString[i]).Y / 2), textColor);
            }

            FinalBomber.Instance.SpriteBatch.End();
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Final_Bomber.Controls;
using Microsoft.Xna.Framework.Input;

namespace Final_Bomber.Screens
{
    public class MultiplayerGameModeMenuScreen : BaseGameState
    {
        #region Field region
        string[] menuString;
        int indexMenu;
        Vector2 menuPosition;
        #endregion

        #region Constructor region
        public MultiplayerGameModeMenuScreen(Game game, GameStateManager manager)
            : base(game, manager)
        {
            menuString = new string[] { "Local game", "Online game", "Back" };
            indexMenu = 0;
        }
        #endregion

        #region XNA Method region

        public override void Initialize()
        {
            menuPosition = new Vector2(Config.Resolutions[Config.IndexResolution, 0] / 2, Config.Resolutions[Config.IndexResolution, 1] / 2);
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
                switch (indexMenu)
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
            FinalBomber.Instance.SpriteBatch.Begin();

            base.Draw(gameTime);

            ControlManager.Draw(FinalBomber.Instance.SpriteBatch);

            for (int i = 0; i < menuString.Length; i++)
            {
                Color textColor = Color.Black;
                if (i == indexMenu)
                    textColor = Color.Green;

                FinalBomber.Instance.SpriteBatch.DrawString(this.BigFont, menuString[i],
                    new Vector2(menuPosition.X - this.BigFont.MeasureString(menuString[i]).X / 2,
                        menuPosition.Y + this.BigFont.MeasureString(menuString[i]).Y * i - this.BigFont.MeasureString(menuString[i]).Y / 2), textColor);
            }

            FinalBomber.Instance.SpriteBatch.End();
        }

        #endregion

        #region Title Screen Methods

        #endregion
    }
}

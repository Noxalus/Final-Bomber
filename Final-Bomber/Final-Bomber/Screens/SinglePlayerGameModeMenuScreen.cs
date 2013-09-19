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
    public class SinglePlayerGameModeMenuScreen : BaseGameState
    {
        #region Field region
        string[] menuString;
        int indexMenu;
        Vector2 menuPosition;
        #endregion

        #region Constructor region
        public SinglePlayerGameModeMenuScreen(Game game, GameStateManager manager)
            : base(game, manager)
        {
            menuString = new string[] { "Adventure", "Challenge", "Match", "Back" };
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
                    case 2:
                        StateManager.ChangeState(GameRef.BattleMenuScreen);
                        break;
                    case 3:
                        StateManager.ChangeState(GameRef.TitleScreen);
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
            GameRef.SpriteBatch.Begin();

            base.Draw(gameTime);

            ControlManager.Draw(GameRef.SpriteBatch);

            for (int i = 0; i < menuString.Length; i++)
            {
                Color textColor = Color.Black;
                if (i == indexMenu)
                    textColor = Color.Green;

                GameRef.SpriteBatch.DrawString(BigFont, menuString[i],
                    new Vector2(menuPosition.X - BigFont.MeasureString(menuString[i]).X / 2,
                        menuPosition.Y + BigFont.MeasureString(menuString[i]).Y * i - BigFont.MeasureString(menuString[i]).Y / 2), textColor);
            }

            GameRef.SpriteBatch.End();
        }

        #endregion

        #region Title Screen Methods

        #endregion
    }
}

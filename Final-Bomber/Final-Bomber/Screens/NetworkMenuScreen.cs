using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Final_Bomber.Controls;
using Microsoft.Xna.Framework.Input;

namespace Final_Bomber.Screens
{
    public class NetworkMenuScreen : BaseGameState
    {
        #region Field region
        string[] menuString;
        int indexMenu;
        Vector2 menuPosition;
        #endregion

        #region Constructor region
        public NetworkMenuScreen(Game game, GameStateManager manager)
            : base(game, manager)
        {
            menuString = new string[] { "Créer un serveur", "Rejoindre un serveur", "Retour" };
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
                        StateManager.ChangeState(GameRef.CreateServerMenuScreen);
                        break;
                    case 1:
                        StateManager.ChangeState(GameRef.JoinServerMenuScreen);
                        break;
                    case 2:
                        StateManager.ChangeState(GameRef.UserLoginMenuScreen);
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

                GameRef.SpriteBatch.DrawString(this.BigFont, menuString[i],
                    new Vector2(menuPosition.X - this.BigFont.MeasureString(menuString[i]).X / 2,
                        menuPosition.Y + this.BigFont.MeasureString(menuString[i]).Y * i - this.BigFont.MeasureString(menuString[i]).Y / 2), textColor);
            }

            GameRef.SpriteBatch.End();
        }

        #endregion
    }
}

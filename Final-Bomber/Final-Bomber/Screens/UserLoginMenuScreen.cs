using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Final_Bomber.Controls;
using Microsoft.Xna.Framework.Input;

namespace Final_Bomber.Screens
{
    public class UserLoginMenuScreen : BaseGameState
    {
        #region Field region
        string[] menuString;
        int indexMenu;
        Vector2 menuPosition;

        // Keyboarding
        Keys[] prevPressedKeys;
        Keys[] pressedKeys;

        bool shiftPressed;

        bool isActive;
        bool readKey;

        string[] login;
        string statusMsg;

        private float textCursorBlinkFrequency;

        #endregion

        #region Constructor region
        public UserLoginMenuScreen(Game game, GameStateManager manager)
            : base(game, manager)
        {
            menuString = new string[] { "Pseudo", "Mot de passe", "Connexion", "Retour" };
            indexMenu = 0;

            prevPressedKeys = new Keys[50];
            pressedKeys = new Keys[50];

            shiftPressed = false;

            isActive = true;
            readKey = false;

            login = new string[] { "", "" };
            statusMsg = "";

            textCursorBlinkFrequency = Config.TextCursorBlinkFrequency;
        }
        #endregion

        #region XNA Method region

        public override void Initialize()
        {
            menuPosition = new Vector2(Config.Resolutions[Config.IndexResolution, 0] / 2, Config.Resolutions[Config.IndexResolution, 1] / 3);
            //GameRef.Server.LoggedIn += new MainServer.LoggedInEventHandler(ServerLoggedIn);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            ControlManager.Update(gameTime, PlayerIndex.One);

            if (textCursorBlinkFrequency >= 0f)
                textCursorBlinkFrequency -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            else
                textCursorBlinkFrequency = Config.TextCursorBlinkFrequency;

            if (indexMenu == 0 || indexMenu == 1)
                isActive = true;
            else
                isActive = false;

            if (isActive)
            {
                if (login[indexMenu].Length < 40)
                    pressedKeys = InputHandler.KeyboardState.GetPressedKeys();

                shiftPressed = InputHandler.KeyboardState.IsKeyDown(Keys.LeftShift) || InputHandler.KeyboardState.IsKeyDown(Keys.RightShift);

                if (InputHandler.KeyboardState != InputHandler.LastKeyboardState)
                {
                    if (InputHandler.KeyboardState.IsKeyDown(Keys.Space))
                        login[indexMenu] += " ";
                    if (InputHandler.KeyboardState.IsKeyDown(Keys.Back) && login[indexMenu].Length > 0)
                        login[indexMenu] = login[indexMenu].Remove(login[indexMenu].Length - 1, 1);
                }

                foreach (Keys key in pressedKeys)
                {
                    if (!prevPressedKeys.Contains(key))
                    {
                        string keyString = key.ToString();

                        if (keyString.Length > 1)
                            keyString = Keyboarding.SpecialChar(keyString, shiftPressed, false);

                        if (keyString != null)
                        {
                            if (!shiftPressed)
                                keyString = keyString.ToLower();

                            login[indexMenu] += keyString;
                        }
                    }
                }
                prevPressedKeys = pressedKeys;
            }

            if (InputHandler.KeyPressed(Keys.Enter))
            {
                switch (indexMenu)
                {
                    case 0:
                        break;
                    case 1:
                        break;
                    case 2:
                        //StateManager.ChangeState(GameRef.NetworkMenuScreen);
                        //GameRef.Server.SendLogin(login[0], login[1]);
                        break;
                    case 3:
                        Exit();
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
            else if (InputHandler.KeyPressed(Keys.Down) || InputHandler.KeyPressed(Keys.Tab))
            {
                indexMenu = (indexMenu + 1) % menuString.Length;
            }

            base.Update(gameTime);

            GameRef.Server.RunMainConnection();
        }

        public override void Draw(GameTime gameTime)
        {
            GameRef.SpriteBatch.Begin();

            base.Draw(gameTime);

            ControlManager.Draw(GameRef.SpriteBatch);

            if (statusMsg != "")
                GameRef.SpriteBatch.DrawString(this.BigFont, statusMsg, Vector2.Zero, Color.Red);

            for (int i = 0; i < menuString.Length; i++)
            {
                Color textColor = Color.Black;
                if (i == indexMenu)
                    textColor = Color.Green;

                GameRef.SpriteBatch.DrawString(this.BigFont, menuString[i],
                    new Vector2(menuPosition.X - this.BigFont.MeasureString(menuString[i]).X / 2,
                        menuPosition.Y + 2 * (this.BigFont.MeasureString(menuString[i]).Y * i) - 
                        this.BigFont.MeasureString(menuString[i]).Y / 2), textColor);

                if (i == 0 || i == 1)
                {
                    GameRef.SpriteBatch.DrawString(this.BigFont, ": ",
                    new Vector2(menuPosition.X + this.BigFont.MeasureString(menuString[i]).X / 2,
                    menuPosition.Y + 2 * (this.BigFont.MeasureString(menuString[i]).Y * i) -
                    this.BigFont.MeasureString(menuString[i]).Y / 2), Color.White);
                }
            }

            if ((indexMenu == 0 || indexMenu == 1) && textCursorBlinkFrequency > Config.TextCursorBlinkFrequency / 2f)
            {
                GameRef.SpriteBatch.DrawString(this.BigFont, "|",
                new Vector2(menuPosition.X + this.BigFont.MeasureString(login[indexMenu]).X / 2,
                menuPosition.Y + 2 * (this.BigFont.MeasureString(menuString[indexMenu]).Y * indexMenu) -
                this.BigFont.MeasureString(menuString[indexMenu]).Y / 2 +
                this.BigFont.MeasureString(menuString[indexMenu]).Y), Color.White);
            }

            GameRef.SpriteBatch.DrawString(this.BigFont, login[0],
            new Vector2(menuPosition.X - this.BigFont.MeasureString(login[0]).X / 2,
                menuPosition.Y - this.BigFont.MeasureString(menuString[0]).Y / 2 + 
                this.BigFont.MeasureString(menuString[0]).Y), Color.White);

            GameRef.SpriteBatch.DrawString(this.BigFont, Functions.HidePassword(login[1]),
            new Vector2(menuPosition.X - this.BigFont.MeasureString(Functions.HidePassword(login[1])).X / 2,
                menuPosition.Y - this.BigFont.MeasureString(menuString[1]).Y / 2 +
                3 * this.BigFont.MeasureString(menuString[1]).Y), Color.White);

            GameRef.SpriteBatch.End();
        }

        #endregion

        #region Private Methods Region

        private void ServerLoggedIn(byte status)
        {
            switch (status)
            {
                case 0:
                    GameRef.HasLoggedIn = true;
                    this.statusMsg = "Bien connecté !";
                    break;
                case 1:
                    this.statusMsg = "Cet utilisateur n'existe pas !";
                    break;
                case 2:
                    this.statusMsg = "Mauvais mot de passe !";
                    break;
                case 3:
                    this.statusMsg = "Vous êtes déjà connecté !";
                    break;
            }
        }

        private void Exit()
        {
            //GameRef.Server.LoggedIn -= new MainServer.LoggedInEventHandler(ServerLoggedIn);
            StateManager.ChangeState(GameRef.UserMenuScreen);
        }
        #endregion
    }
}

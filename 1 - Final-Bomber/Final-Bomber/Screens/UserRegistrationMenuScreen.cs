using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Final_Bomber.Controls;
using Microsoft.Xna.Framework.Input;
using System.Threading;

namespace Final_Bomber.Screens
{
    public class UserRegistrationMenuScreen : BaseGameState
    {
        #region Field region
        string[] menuString;
        int indexMenu;
        Vector2 menuPosition;

        // Keyboarding
        Keys[] prevPressedKeys;
        Keys[] pressedKeys;

        bool shiftPressed;
        bool altGrPressed;

        bool isActive;
        bool readKey;

        string[] login;
        string statusMsg;

        private float textCursorBlinkFrequency;

        bool pseudoCreated;

        #endregion

        #region Constructor region
        public UserRegistrationMenuScreen(Game game, GameStateManager manager)
            : base(game, manager)
        {
            menuString = new string[] { "Pseudo", "Mot de passe", "Mot de passe (bis)", "Mail", "Valider" };
            indexMenu = 0;

            textCursorBlinkFrequency = Config.TextCursorBlinkFrequency;
        }
        #endregion

        #region XNA Method region

        public override void Initialize()
        {
            menuPosition = new Vector2(Config.Resolutions[Config.IndexResolution, 0] / 2, Config.Resolutions[Config.IndexResolution, 1] / 4);
            //FinalBomber.Instance.Server.CreatedAccount += new MainServer.CreatedAccountEventHandler(ServerReceiveCreateAccount);

            prevPressedKeys = new Keys[50];
            pressedKeys = new Keys[50];

            shiftPressed = false;

            isActive = true;
            readKey = false;

            login = new string[] { "", "", "", "" };
            statusMsg = "";

            pseudoCreated = false;

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

            if (indexMenu >= 0 && indexMenu <= 3)
                isActive = true;
            else
                isActive = false;

            if (isActive)
            {
                if (login[indexMenu].Length < 40)
                    pressedKeys = InputHandler.KeyboardState.GetPressedKeys();

                shiftPressed = InputHandler.KeyboardState.IsKeyDown(Keys.LeftShift) || InputHandler.KeyboardState.IsKeyDown(Keys.RightShift);
                altGrPressed = InputHandler.KeyboardState.IsKeyDown(Keys.RightAlt) || 
                    (InputHandler.KeyboardState.IsKeyDown(Keys.LeftControl) && InputHandler.KeyboardState.IsKeyDown(Keys.LeftAlt));

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
                            keyString = Keyboarding.SpecialChar(keyString, shiftPressed, altGrPressed);

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

            if (InputHandler.KeyPressed(Keys.Enter) && indexMenu == 4)
            {
                statusMsg = "";
                if (!(login[0] == "" || login[1] == "" || login[2] == "" || login[3] == ""))
                {
                    if (Functions.CheckChars(login[0]))
                    {
                        if (login[1] == login[2])
                        {
                            if (login[1].Length >= 6 && login[1].Length <= 15)
                            {
                                if (login[0].Length < 20)
                                {
                                    //FinalBomber.Instance.Server.SendCreateAccount(login[0], login[1]);
                                }
                                else
                                {
                                    statusMsg = "Le nom d'utilisateur est trop long ! (20 caractères maximum)";
                                }
                            }
                            else
                                statusMsg = "Le mot de passe doit être compris\n entre 6 et 15 caractères !";
                        }
                        else
                        {
                            statusMsg = "Les mots passe sont différents !";
                        }
                    }
                    else
                    {
                        statusMsg = "Le nom d'utilisateur contient\n des caractères non autorisés !";
                    }
                }
                else
                {
                    statusMsg = "Veuillez remplir tous les champs !";
                }
            }

            if (InputHandler.KeyPressed(Keys.Up) ||
                (InputHandler.KeyPressed(Keys.Tab) && InputHandler.KeyDown(Keys.LeftShift)))
            {
                IndexMenuGoUp();
            }
            else if (InputHandler.KeyPressed(Keys.Down) || InputHandler.KeyPressed(Keys.Tab))
            {
                IndexMenuGoDown();
            }

            //FinalBomber.Instance.Server.RunMainConnection();

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            FinalBomber.Instance.SpriteBatch.Begin();

            base.Draw(gameTime);

            ControlManager.Draw(FinalBomber.Instance.SpriteBatch);

            if (statusMsg != "")
                FinalBomber.Instance.SpriteBatch.DrawString(this.BigFont, statusMsg, Vector2.Zero, Color.Red);

            if (pseudoCreated)
                Thread.Sleep(1000);

            for (int i = 0; i < menuString.Length; i++)
            {
                Color textColor = Color.Black;
                if (i == indexMenu)
                    textColor = Color.Green;

                FinalBomber.Instance.SpriteBatch.DrawString(this.BigFont, menuString[i],
                    new Vector2(menuPosition.X - this.BigFont.MeasureString(menuString[i]).X / 2,
                        menuPosition.Y + 2 * (this.BigFont.MeasureString(menuString[i]).Y * i) -
                        this.BigFont.MeasureString(menuString[i]).Y / 2), textColor);

                if (i == 0 || i == 1)
                {
                    FinalBomber.Instance.SpriteBatch.DrawString(this.BigFont, ": ",
                    new Vector2(menuPosition.X + this.BigFont.MeasureString(menuString[i]).X / 2,
                    menuPosition.Y + 2 * (this.BigFont.MeasureString(menuString[i]).Y * i) -
                    this.BigFont.MeasureString(menuString[i]).Y / 2), Color.White);
                }
            }

            if ((indexMenu >= 0 && indexMenu <= 3) && textCursorBlinkFrequency > Config.TextCursorBlinkFrequency / 2f)
            {
                string logins = login[indexMenu];

                if (indexMenu == 1 || indexMenu == 2)
                    logins = Functions.HidePassword(logins);

                FinalBomber.Instance.SpriteBatch.DrawString(this.BigFont, "|",
                new Vector2(menuPosition.X + this.BigFont.MeasureString(logins).X / 2,
                menuPosition.Y + 2 * (this.BigFont.MeasureString(menuString[indexMenu]).Y * indexMenu) -
                this.BigFont.MeasureString(menuString[indexMenu]).Y / 2 +
                this.BigFont.MeasureString(menuString[indexMenu]).Y), Color.White);
            }

            FinalBomber.Instance.SpriteBatch.DrawString(this.BigFont, login[0],
            new Vector2(menuPosition.X - this.BigFont.MeasureString(login[0]).X / 2,
            menuPosition.Y - this.BigFont.MeasureString(menuString[0]).Y / 2 +
            this.BigFont.MeasureString(menuString[0]).Y), Color.White);

            FinalBomber.Instance.SpriteBatch.DrawString(this.BigFont, Functions.HidePassword(login[1]),
            new Vector2(menuPosition.X - this.BigFont.MeasureString(Functions.HidePassword(login[1])).X / 2,
            menuPosition.Y - this.BigFont.MeasureString(menuString[1]).Y / 2 +
            3 * this.BigFont.MeasureString(menuString[1]).Y), Color.White);

            FinalBomber.Instance.SpriteBatch.DrawString(this.BigFont, Functions.HidePassword(login[2]),
            new Vector2(menuPosition.X - this.BigFont.MeasureString(Functions.HidePassword(login[2])).X / 2,
            menuPosition.Y - this.BigFont.MeasureString(menuString[2]).Y / 2 +
            5 * this.BigFont.MeasureString(menuString[2]).Y), Color.White);

            FinalBomber.Instance.SpriteBatch.DrawString(this.BigFont, login[3],
            new Vector2(menuPosition.X - this.BigFont.MeasureString(login[3]).X / 2,
            menuPosition.Y - this.BigFont.MeasureString(menuString[3]).Y / 2 +
            7 * this.BigFont.MeasureString(menuString[3]).Y), Color.White);


            /*
            for (int i = 0; i < menuString.Length; i++)
            {
                Color textColor = Color.Black;
                if (i == indexMenu)
                    textColor = Color.Green;

                FinalBomber.Instance.SpriteBatch.DrawString(this.BigFont, menuString[i],
                    new Vector2(menuPosition.X - this.BigFont.MeasureString(menuString[i]).X / 2,
                        menuPosition.Y + this.BigFont.MeasureString(menuString[i]).Y * i - this.BigFont.MeasureString(menuString[i]).Y / 2), textColor);
            }

            FinalBomber.Instance.SpriteBatch.DrawString(this.BigFont, ": " + login[0],
            new Vector2(menuPosition.X + this.BigFont.MeasureString(menuString[0]).X / 2,
                menuPosition.Y - this.BigFont.MeasureString(menuString[0]).Y / 2), Color.White);

            FinalBomber.Instance.SpriteBatch.DrawString(this.BigFont, ": " + Functions.HidePassword(login[1]),
            new Vector2(menuPosition.X + this.BigFont.MeasureString(menuString[1]).X / 2,
                menuPosition.Y + this.BigFont.MeasureString(menuString[1]).Y -
                this.BigFont.MeasureString(menuString[1]).Y / 2), Color.White);

            FinalBomber.Instance.SpriteBatch.DrawString(this.BigFont, ": " + Functions.HidePassword(login[2]),
            new Vector2(menuPosition.X + this.BigFont.MeasureString(menuString[2]).X / 2,
                menuPosition.Y + (this.BigFont.MeasureString(menuString[2]).Y / 2) * 2 + 
                this.BigFont.MeasureString(menuString[2]).Y / 2), Color.White);

            FinalBomber.Instance.SpriteBatch.DrawString(this.BigFont, ": " + login[3],
            new Vector2(menuPosition.X + this.BigFont.MeasureString(menuString[3]).X / 2,
            menuPosition.Y + this.BigFont.MeasureString(menuString[3]).Y * 3 - this.BigFont.MeasureString(menuString[3]).Y / 2), Color.White);
            */

            FinalBomber.Instance.SpriteBatch.End();
        }

        #endregion

        #region Private Methods Region

        private void ServerReceiveCreateAccount(byte status)
        {
            switch (status)
            {
                case 0:
                    this.statusMsg = "Votre compte a été créé !";
                    pseudoCreated = true;
                    StateManager.ChangeState(FinalBomber.Instance.UserMenuScreen);
                    break;
                case 1:
                    this.statusMsg = "Ce pseudo est déjà pris !";
                    break;
                default:
                    this.statusMsg = "Erreur inconnue lors de l'inscription !";
                    break;
            }
        }

        private void Exit()
        {
            //FinalBomber.Instance.Server.CreatedAccount -= new MainServer.CreatedAccountEventHandler(ServerReceiveCreateAccount);
            StateManager.ChangeState(FinalBomber.Instance.UserMenuScreen);
        }

        private void IndexMenuGoUp()
        {
            if (indexMenu <= 0)
                    indexMenu = menuString.Length - 1;
            else
                indexMenu--;
        }

        private void IndexMenuGoDown()
        {
            indexMenu = (indexMenu + 1) % menuString.Length;
        }

        #endregion
    }
}

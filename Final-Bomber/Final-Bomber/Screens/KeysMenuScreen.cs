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
    public class KeysMenuScreen : BaseGameState
    {
        #region Field Region
        string[] menuString;
        int indexMenu;
        Vector2 menuPosition;
        int indexPlayer;

        int counter;
        string[] keysNames;
        string[] instructions;

        #endregion

        #region Property Region

        #endregion

        #region Constructor Region

        public KeysMenuScreen(Game game, GameStateManager manager)
            : base(game, manager)
        {
        }

        #endregion

        #region XNA Method Region

        public override void Initialize()
        {
            menuString = new string[Config.PlayersName.Length + 1];
            for (int i = 0; i < Config.PlayersName.Length; i++)
                menuString[i] = Config.PlayersName[i];
            menuString[Config.PlayersName.Length] = "Retour";

            indexMenu = 0;
            indexPlayer = -1;
            menuPosition = new Vector2(GameRef.GraphicsDevice.Viewport.Width / 2.5f, GameRef.GraphicsDevice.Viewport.Height / 2.5f);

            counter = 0;
            keysNames = new string[] { "Haut", "Bas", "Gauche", "Droite", "Poser une bombe" };

            instructions = new string[2];
            instructions[0] = "Appuyez sur une touche pour la touche";
            instructions[1] = "\"" + keysNames[counter] + "\" !";

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
                StateManager.PushState(GameRef.TitleScreen);

            
            if (indexPlayer >= 0)
            {
                if (counter < 5)
                {
                    instructions[0] = "Appuyez sur une touche\n    pour la touche";
                    instructions[1] = "\"" + keysNames[counter] + "\" !";
                }
                else
                {
                    instructions[0] = "Les touches ont bien\n   été configurées !";
                    instructions[1] = "Appuyez sur Entrer !";
                }

                // Séléction des touches
                /*
                if (counter <= keysNames.Length - 1 && InputHandler.HavePressedKey() && InputHandler.GetPressedKeys().Length > 0)
                {
                    Config.PlayersKeys[indexPlayer][counter] = InputHandler.GetPressedKeys()[0];
                    counter++;
                }
                else if (InputHandler.GetPressedKeys().Length > 0 && counter >= keysNames.Length - 1)
                {
                    if (InputHandler.KeyPressed(Keys.Enter))
                    {
                        counter = 0;
                        indexPlayer = -1;
                    }
                }
                */

                // Controller inputs
                if (InputHandler.HavePressedButton(PlayerIndex.One) && InputHandler.GetPressedButton(PlayerIndex.One).Length > 0)
                {
                    Buttons buttons = InputHandler.GetPressedButton(PlayerIndex.One)[0];
                    counter++;
                }
            }
            else
            {
                if (InputHandler.KeyPressed(Keys.Enter))
                {
                    indexPlayer = -1;
                    if (indexMenu >= 0 && indexMenu <= Config.PlayersName.Length - 1)
                        indexPlayer = indexMenu;
                    else if (indexMenu == Config.PlayersName.Length)
                        StateManager.ChangeState(GameRef.OptionMenuScreen);
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
            GameRef.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, Matrix.Identity);

            base.Draw(gameTime);

            ControlManager.Draw(GameRef.SpriteBatch);

            Color textColor = Color.Black;

            if (indexPlayer >= 0)
            {
                string text = "Changement des touches\n     de " + Config.PlayersName[indexPlayer];
                GameRef.SpriteBatch.DrawString(BigFont, text,
                        new Vector2(menuPosition.X / 1.5f, (BigFont.MeasureString(text).Y) / 2), textColor);

                GameRef.SpriteBatch.DrawString(BigFont, instructions[0], 
                    new Vector2(menuPosition.X/1.5f, 2 * (BigFont.MeasureString(instructions[0]).Y)), Color.Black);
                GameRef.SpriteBatch.DrawString(BigFont, instructions[1],
                    new Vector2(menuPosition.X / 1.5f + 250 - BigFont.MeasureString(instructions[1]).X / 2, 
                        6 * (BigFont.MeasureString(instructions[1]).Y)), Color.Black);
            }
            else
            {
                for (int i = 0; i < menuString.Length; i++)
                {
                    textColor = i == indexMenu ? Color.Green : Color.Black;

                    GameRef.SpriteBatch.DrawString(BigFont, menuString[i],
                        new Vector2(menuPosition.X, menuPosition.Y + (BigFont.MeasureString(menuString[i]).Y) * i), textColor);
                }
            }

            GameRef.SpriteBatch.End();
        }

        #endregion

        #region Abstract Method Region
        #endregion

        #region Method Region
        #endregion
    }
}

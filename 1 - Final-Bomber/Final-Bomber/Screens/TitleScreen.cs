using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

using Final_Bomber.Controls;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using System.Threading;
namespace Final_Bomber.Screens
{
    public class TitleScreen : BaseGameState
    {
        #region Field region
        Texture2D backgroundImage;
        string[] menuString;
        int indexMenu;
        Vector2 menuPosition;
        bool enableMenu;

        private SoundEffect _title;
        #endregion

        #region Constructor region
        public TitleScreen(Game game, GameStateManager manager)
            : base(game, manager)
        {
            menuString = new string[] { "Single player", "Multiplayer", "Options", "Credits", "Quit" };
            indexMenu = 0;
            enableMenu = false;
        }
        #endregion

        #region XNA Method region

        public override void Initialize()
        {
            menuPosition = new Vector2(Config.Resolutions[Config.IndexResolution, 0] / 2, 3 * Config.Resolutions[Config.IndexResolution, 1] / 4 - 80);
            base.Initialize();
            MediaPlayer.Play(FinalBomber.Instance.Content.Load<Song>("Audio/Musics/title"));
            _title.Play();
        }

        protected override void LoadContent()
        {
            // Graphics
            ContentManager content = FinalBomber.Instance.Content;
            backgroundImage = content.Load<Texture2D>("Graphics/Titles/Title");

            // Music
            MediaPlayer.IsRepeating = true;
            _title = content.Load<SoundEffect>("Audio/Sounds/title");

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            ControlManager.Update(gameTime, PlayerIndex.One);

            if (InputHandler.KeyPressed(Keys.Enter))
            {
                if (!enableMenu)
                    enableMenu = true;
                else
                {
                    switch (menuString[indexMenu])
                    {
                        case "Single player":
                            StateManager.ChangeState(FinalBomber.Instance.SinglePlayerGameModeMenuScreen);
                            break;
                        case "Multiplayer":
                            StateManager.ChangeState(FinalBomber.Instance.MultiplayerGameModeMenuScreen);
                            break;
                        case "Options":
                            StateManager.ChangeState(FinalBomber.Instance.OptionMenuScreen);
                            break;
                        case "Credits":
                            StateManager.ChangeState(FinalBomber.Instance.CreditMenuScreen);
                            break;
                        case "Quit":
                            FinalBomber.Instance.Exit();
                            break;
                    }
                }
            }

            if (InputHandler.KeyPressed(Keys.F1))
            {
                StateManager.ChangeState(FinalBomber.Instance.LobbyMenuScreen);
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

            FinalBomber.Instance.SpriteBatch.Draw(backgroundImage, FinalBomber.Instance.ScreenRectangle, Color.White);

            ControlManager.Draw(FinalBomber.Instance.SpriteBatch);

            if (enableMenu)
            {
                for (int i = 0; i < menuString.Length; i++)
                {
                    Color textColor = Color.Black;
                    if (i == indexMenu)
                        textColor = Color.Green;
                    FinalBomber.Instance.SpriteBatch.DrawString(BigFont, menuString[i],
                        new Vector2(menuPosition.X - BigFont.MeasureString(menuString[i]).X / 2,
                            menuPosition.Y + BigFont.MeasureString(menuString[i]).Y * i - BigFont.MeasureString(menuString[i]).Y / 2), textColor);
                }
            }

            FinalBomber.Instance.SpriteBatch.End();
        }

        #endregion

        #region Title Screen Methods

        #endregion
    }
}

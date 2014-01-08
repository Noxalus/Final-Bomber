using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

using Final_Bomber.Controls;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
namespace Final_Bomber.Screens
{
    public class TitleScreen : BaseMenuScreen
    {
        #region Field region
        private Texture2D _backgroundImage;
        private bool _enableMenu;
        private SoundEffect _title;
        #endregion

        #region Constructor region
        public TitleScreen(Game game, GameStateManager manager)
            : base(game, manager)
        {
            MenuString = new string[] { "Single player", "Multiplayer", "Options", "Credits", "Quit" };
            _enableMenu = false;
        }
        #endregion

        #region XNA Method region

        public override void Initialize()
        {
            base.Initialize();
            MediaPlayer.Play(FinalBomber.Instance.Content.Load<Song>("Audio/Musics/title"));
            _title.Play();
        }

        protected override void LoadContent()
        {
            // Graphics
            ContentManager content = FinalBomber.Instance.Content;
            _backgroundImage = content.Load<Texture2D>("Graphics/Titles/Title");

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
                if (!_enableMenu)
                {
                    _enableMenu = true;
                }
                else
                {
                    switch (MenuString[IndexMenu])
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
            
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            FinalBomber.Instance.SpriteBatch.Begin();

            base.Draw(gameTime);

            FinalBomber.Instance.SpriteBatch.Draw(_backgroundImage, FinalBomber.Instance.ScreenRectangle, Color.White);

            ControlManager.Draw(FinalBomber.Instance.SpriteBatch);

            if (_enableMenu)
            {
                for (int i = 0; i < MenuString.Length; i++)
                {
                    Color textColor = Color.Black;
                    if (i == IndexMenu)
                        textColor = Color.Green;
                    FinalBomber.Instance.SpriteBatch.DrawString(BigFont, MenuString[i],
                        new Vector2(MenuPosition.X - BigFont.MeasureString(MenuString[i]).X / 2,
                            MenuPosition.Y + BigFont.MeasureString(MenuString[i]).Y * i - BigFont.MeasureString(MenuString[i]).Y / 2), textColor);
                }
            }

            FinalBomber.Instance.SpriteBatch.End();
        }

        #endregion

        #region Title Screen Methods

        #endregion
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Final_Bomber.Controls;
using Microsoft.Xna.Framework.Input;

namespace Final_Bomber.Screens
{
    public class KeysMenuScreen : BaseMenuScreen
    {
        #region Field Region

        int _indexPlayer;
        int _counter;
        string[] _keysNames;
        string[] _instructions;

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
            MenuString = new string[Config.PlayersName.Length + 1];
            for (int i = 0; i < Config.PlayersName.Length; i++)
                MenuString[i] = Config.PlayersName[i];
            MenuString[Config.PlayersName.Length] = "Retour";

            _indexPlayer = -1;

            _counter = 0;
            _keysNames = new string[] { "Haut", "Bas", "Gauche", "Droite", "Poser une bombe" };

            _instructions = new string[2];
            _instructions[0] = "Appuyez sur une touche pour la touche";
            _instructions[1] = "\"" + _keysNames[_counter] + "\" !";

            base.Initialize();

            MenuPosition.X = Config.Resolutions[Config.IndexResolution, 0] / 2.5f;
            MenuPosition.Y = Config.Resolutions[Config.IndexResolution, 1] / 2.5f;
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            ControlManager.Update(gameTime, PlayerIndex.One);

            if (InputHandler.KeyDown(Keys.Escape))
                StateManager.PushState(FinalBomber.Instance.TitleScreen);

            
            if (_indexPlayer >= 0)
            {
                if (_counter < 5)
                {
                    _instructions[0] = "Appuyez sur une touche\n    pour la touche";
                    _instructions[1] = "\"" + _keysNames[_counter] + "\" !";
                }
                else
                {
                    _instructions[0] = "Les touches ont bien\n   été configurées !";
                    _instructions[1] = "Appuyez sur Entrer !";
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
                    _counter++;
                }
            }
            else
            {
                if (InputHandler.KeyPressed(Keys.Enter))
                {
                    _indexPlayer = -1;
                    if (IndexMenu >= 0 && IndexMenu <= Config.PlayersName.Length - 1)
                        _indexPlayer = IndexMenu;
                    else if (IndexMenu == Config.PlayersName.Length)
                        StateManager.ChangeState(FinalBomber.Instance.OptionMenuScreen);
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            FinalBomber.Instance.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, Matrix.Identity);

            base.Draw(gameTime);

            ControlManager.Draw(FinalBomber.Instance.SpriteBatch);

            Color textColor = Color.Black;

            if (_indexPlayer >= 0)
            {
                string text = "Changement des touches\nde " + Config.PlayersName[_indexPlayer];
                FinalBomber.Instance.SpriteBatch.DrawString(BigFont, text,
                        new Vector2(MenuPosition.X / 1.5f, (BigFont.MeasureString(text).Y) / 2), textColor);

                FinalBomber.Instance.SpriteBatch.DrawString(BigFont, _instructions[0], 
                    new Vector2(MenuPosition.X/1.5f, 2 * (BigFont.MeasureString(_instructions[0]).Y)), Color.Black);
                FinalBomber.Instance.SpriteBatch.DrawString(BigFont, _instructions[1],
                    new Vector2(MenuPosition.X / 1.5f + 250 - BigFont.MeasureString(_instructions[1]).X / 2, 
                        6 * (BigFont.MeasureString(_instructions[1]).Y)), Color.Black);
            }
            else
            {
                for (int i = 0; i < MenuString.Length; i++)
                {
                    textColor = i == IndexMenu ? Color.Green : Color.Black;

                    FinalBomber.Instance.SpriteBatch.DrawString(BigFont, MenuString[i],
                        new Vector2(MenuPosition.X, MenuPosition.Y + (BigFont.MeasureString(MenuString[i]).Y) * i), textColor);
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

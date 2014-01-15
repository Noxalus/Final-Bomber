using System;
using FBLibrary;
using FBClient.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FBClient.Screens.MenuScreens
{
    public class ItemMenuScreen : BaseGameState
    {
        #region Field Region
        Texture2D _itemsTexture;
        int _indexMenu;

        // Pulsation
        float _pulsationSpeed;
        float _pulsationDecrease;
        #endregion

        #region Property Region

        #endregion

        #region Constructor Region

        public ItemMenuScreen(Game game, GameStateManager manager)
            : base(game, manager)
        {
        }

        #endregion

        #region XNA Method Region

        public override void Initialize()
        {
            _indexMenu = 0;
            _pulsationSpeed = 0;
            _pulsationDecrease = 0;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _itemsTexture = FinalBomber.Instance.Content.Load<Texture2D>("Graphics/Sprites/item");
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            ControlManager.Update(gameTime, PlayerIndex.One);

            if (InputHandler.KeyDown(Keys.Escape))
                StateManager.PushState(FinalBomber.Instance.BattleMenuScreen);

            if (InputHandler.KeyPressed(Keys.Enter))
            {
                if (GameConfiguration.PowerUpTypeAvailable.Exists(t => t == Config.ItemTypeArray[_indexMenu]))
                    GameConfiguration.PowerUpTypeAvailable.Remove(Config.ItemTypeArray[_indexMenu]);
                else
                    GameConfiguration.PowerUpTypeAvailable.Add(Config.ItemTypeArray[_indexMenu]);
            }

            if (InputHandler.KeyPressed(Keys.Left))
            {
                if (_indexMenu <= 0)
                    _indexMenu = Config.ItemTypeArray.Length - 1;
                else
                    _indexMenu--;
            }
            else if (InputHandler.KeyPressed(Keys.Right))
            {
                _indexMenu = (_indexMenu + 1) % Config.ItemTypeArray.Length;
            }

            // Pulsation
            float pulsationSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;
            _pulsationDecrease = Math.Min(_pulsationDecrease + pulsationSpeed, 1);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            FinalBomber.Instance.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, Matrix.Identity);

            base.Draw(gameTime);

            ControlManager.Draw(FinalBomber.Instance.SpriteBatch);

            int totalWidth = 40 * Config.ItemTypeArray.Length;
            for (int i = 0; i < Config.ItemTypeArray.Length; i++)
            {
                float scale = 1f;
                Color textColor;
                if (i == _indexMenu)
                {
                    textColor = Color.Green;
                    // Pulsation
                    double time = gameTime.TotalGameTime.TotalSeconds;
                    float pulsation = (float)Math.Sin(time * 6) + 1;
                    scale += pulsation * 0.05f * _pulsationDecrease;
                }
                else
                    textColor = Color.White;

                FinalBomber.Instance.SpriteBatch.Draw(_itemsTexture,
                                         new Vector2(40*i + FinalBomber.Instance.ScreenRectangle.Width/2 - totalWidth/2,
                                                     FinalBomber.Instance.ScreenRectangle.Height/2 - 32/2),
                                         GameConfiguration.PowerUpTypeAvailable.Exists(t => t == Config.ItemTypeArray[i])
                                             ? new Rectangle(0, i*32, 32, 32)
                                             : new Rectangle(32, i*32, 32, 32),
                                         textColor, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }

            FinalBomber.Instance.SpriteBatch.End();
        }

        #endregion
    }
}

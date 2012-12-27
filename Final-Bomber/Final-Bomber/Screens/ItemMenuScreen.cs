using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Final_Bomber.Components;
using Final_Bomber.Controls;
using Microsoft.Xna.Framework.Input;

namespace Final_Bomber.Screens
{
    public class ItemMenuScreen : BaseGameState
    {
        #region Field Region
        Texture2D itemsTexture;
        int indexMenu;

        // Pulsation
        float pulsationSpeed;
        float pulsationDecrease;
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
            indexMenu = 0;
            pulsationSpeed = 0;
            pulsationDecrease = 0;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            itemsTexture = GameRef.Content.Load<Texture2D>("Graphics/Characters/item");
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            ControlManager.Update(gameTime, PlayerIndex.One);

            if (InputHandler.KeyDown(Keys.Escape))
                StateManager.PushState(GameRef.BattleMenuScreen);

            if (InputHandler.KeyPressed(Keys.Enter))
            {
                if (Config.ItemTypeAvaible.Exists(t => t == Config.ItemTypeArray[indexMenu]))
                    Config.ItemTypeAvaible.Remove(Config.ItemTypeArray[indexMenu]);
                else
                    Config.ItemTypeAvaible.Add(Config.ItemTypeArray[indexMenu]);
            }

            if (InputHandler.KeyPressed(Keys.Left))
            {
                if (indexMenu <= 0)
                    indexMenu = Config.ItemTypeArray.Length - 1;
                else
                    indexMenu--;
            }
            else if (InputHandler.KeyPressed(Keys.Right))
            {
                indexMenu = (indexMenu + 1) % Config.ItemTypeArray.Length;
            }

            // Pulsation
            float pulsationSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;
            pulsationDecrease = Math.Min(pulsationDecrease + pulsationSpeed, 1);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GameRef.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, Matrix.Identity);

            base.Draw(gameTime);

            ControlManager.Draw(GameRef.SpriteBatch);

            int totalWidth = 40 * Config.ItemTypeArray.Length; 
            Color textColor = Color.White;
            for (int i = 0; i < Config.ItemTypeArray.Length; i++)
            {
                float scale = 1f;
                if (i == indexMenu)
                {
                    textColor = Color.Green;
                    // Pulsation
                    double time = gameTime.TotalGameTime.TotalSeconds;
                    float pulsation = (float)Math.Sin(time * 6) + 1;
                    scale += pulsation * 0.05f * pulsationDecrease;
                }
                else
                    textColor = Color.White;

                
                if(Config.ItemTypeAvaible.Exists(t => t == Config.ItemTypeArray[i]))
                    GameRef.SpriteBatch.Draw(itemsTexture,
                        new Vector2(40 * i + GameRef.ScreenRectangle.Width / 2 - totalWidth / 2, 
                            GameRef.ScreenRectangle.Height / 2 - 32 / 2), 
                        new Rectangle(0, i * 32, 32, 32), 
                        textColor, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                else
                    GameRef.SpriteBatch.Draw(itemsTexture,
                        new Vector2(40 * i + GameRef.ScreenRectangle.Width / 2 - totalWidth / 2, 
                            GameRef.ScreenRectangle.Height / 2 - 32 / 2), 
                        new Rectangle(32, i * 32, 32, 32),
                        textColor, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }

            GameRef.SpriteBatch.End();
        }

        #endregion
    }
}

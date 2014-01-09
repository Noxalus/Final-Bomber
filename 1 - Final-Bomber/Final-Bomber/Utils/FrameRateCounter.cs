using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FBClient.Utils
{
    public class FrameRateCounter : DrawableGameComponent
    {
        SpriteBatch _spriteBatch;
        SpriteFont _spriteFont;

        int _frameRate = 0;
        int _frameCounter = 0;
        TimeSpan _elapsedTime = TimeSpan.Zero;

        public FrameRateCounter(Game game)
            : base(game)
        {
            // Draw after Screens's Draw method
            DrawOrder = 6000;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _spriteFont = Game.Content.Load<SpriteFont>(@"Graphics\Fonts\ControlFont");

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            Game.Content.Unload();

            base.UnloadContent();
        }


        public override void Update(GameTime gameTime)
        {
            _elapsedTime += gameTime.ElapsedGameTime;

            if (_elapsedTime > TimeSpan.FromSeconds(1))
            {
                _elapsedTime -= TimeSpan.FromSeconds(1);
                _frameRate = _frameCounter;
                _frameCounter = 0;
            }
        }


        public override void Draw(GameTime gameTime)
        {
            _frameCounter++;

            string fps = string.Format("FPS: {0}", _frameRate);

            _spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null);

            _spriteBatch.DrawString(_spriteFont, fps, new Vector2(1, 1), Color.Black);
            _spriteBatch.DrawString(_spriteFont, fps, new Vector2(0, 0), Color.White);

            _spriteBatch.End();
        }
    }
}

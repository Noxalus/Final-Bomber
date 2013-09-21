using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FBLibrary.Core;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Final_Bomber.TileEngine;

namespace Final_Bomber.Sprites
{
    public class BaseSprite
    {
        #region Field Region

        protected readonly Texture2D Texture;
        protected Rectangle SourceRectangle;

        #endregion

        #region Property Region

        public int Width
        {
            get { return SourceRectangle.Width; }
        }

        public int Height
        {
            get { return SourceRectangle.Height; }
        }

        /*
        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle(
                      (int)position.X,
                      (int)position.Y,
                      Width,
                      Height);
            }
        }*/

        #endregion

        #region Constructor Region

        public BaseSprite(Texture2D image, Rectangle? sourceRectangle)
        {
            this.Texture = image;

            if (sourceRectangle.HasValue)
                this.SourceRectangle = sourceRectangle.Value;
            else
                this.SourceRectangle = new Rectangle(
                    0,
                    0,
                    image.Width,
                    image.Height);
        }

        #endregion

        #region Method Region
        #endregion

        #region Virtual Method region

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position)
        {
            if (Width != Engine.TileWidth || Height != Engine.TileHeight)
            {
                spriteBatch.Draw(
                    Texture,
                    new Rectangle(
                        (int) (Engine.Origin.X + position.X - Engine.TileWidth/4f),
                        (int) (Engine.Origin.Y + position.Y - Engine.TileHeight/2f),
                        Engine.TileWidth + Engine.TileWidth/2,
                        Engine.TileHeight + Engine.TileHeight/2),
                    SourceRectangle,
                    Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
            }
            else
            {
                spriteBatch.Draw(
                    Texture,
                    new Vector2(Engine.Origin.X + position.X, Engine.Origin.Y + position.Y),
                    SourceRectangle,
                    Color.White);
            }
        }

        #endregion
    }
}
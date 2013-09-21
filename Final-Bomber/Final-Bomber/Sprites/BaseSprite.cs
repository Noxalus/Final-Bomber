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

        protected Texture2D texture;
        protected Rectangle sourceRectangle;

        #endregion

        #region Property Region

        public int Width
        {
            get { return sourceRectangle.Width; }
        }

        public int Height
        {
            get { return sourceRectangle.Height; }
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
            this.texture = image;

            if (sourceRectangle.HasValue)
                this.sourceRectangle = sourceRectangle.Value;
            else
                this.sourceRectangle = new Rectangle(
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
            spriteBatch.Draw(
                texture,
                position,
                sourceRectangle,
                Color.White);
        }

        #endregion
    }
}
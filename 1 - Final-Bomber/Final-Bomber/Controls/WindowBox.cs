using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Final_Bomber.Controls
{
    public class WindowBox
    {
        #region Field Region
        Texture2D windowSkin;

        Rectangle topLeft;
        Rectangle topRight;
        Rectangle bottomLeft;
        Rectangle bottomRight;

        Rectangle top;
        Rectangle back;
        Rectangle bottom;
        Rectangle left;
        Rectangle right;

        Point _size;
        Vector2 _position;

        #endregion

        #region Property Region

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public Point Size
        {
            get { return _size; }
            set { _size = value; }
        }

        #endregion

        #region Constructor Region

        public WindowBox(Texture2D window, Vector2 position, Point size)
        {
            windowSkin = window;

            topLeft = new Rectangle(0, 0, 12, 12);
            topRight = new Rectangle(13, 0, 12, 12);
            bottomLeft = new Rectangle(0, 13, 12, 12);
            bottomRight = new Rectangle(13, 13, 12, 12);

            top = new Rectangle(12, 0, 1, 10);
            back = new Rectangle(12, 12, 1, 1);
            bottom = new Rectangle(12, 15, 1, 10);
            left = new Rectangle(0, 12, 10, 1);
            right = new Rectangle(15, 12, 10, 1);

            _size = size;
            _position = position;
        }

        #endregion

        public void Draw(SpriteBatch spriteBatch)
        {
            // Top corners
            spriteBatch.Draw(windowSkin, _position, topLeft, Color.White);
            spriteBatch.Draw(windowSkin, new Vector2(_position.X + _size.X - 12, _position.Y), topRight, Color.White);

            // Bottom corners
            spriteBatch.Draw(windowSkin, new Vector2(_position.X, _position.Y + _size.Y - 12), bottomLeft, Color.White);
            spriteBatch.Draw(windowSkin, new Vector2(_position.X + _size.X - 12, _position.Y + _size.Y - 12), bottomRight, Color.White);

            for (int y = 12; y < _size.Y - 12; y++)
            {
                spriteBatch.Draw(windowSkin, new Vector2(_position.X, _position.Y + y), left, Color.White);
                spriteBatch.Draw(windowSkin, new Vector2(_position.X + _size.X - 10, _position.Y + y), right, Color.White);
            }

            for (int x = 12; x < _size.X - 12; x++)
            {
                spriteBatch.Draw(windowSkin, new Vector2(_position.X + x, _position.Y), top, Color.White);
                spriteBatch.Draw(windowSkin, new Vector2(_position.X + x, _position.Y + _size.Y - 10), bottom, Color.White);
            }


            for (int x = 10; x < _size.X - 10; x++)
                for (int y = 10; y < _size.Y - 10; y++)
                    spriteBatch.Draw(windowSkin, new Vector2(_position.X + x, _position.Y + y), back, Color.White);
        }
    }
}

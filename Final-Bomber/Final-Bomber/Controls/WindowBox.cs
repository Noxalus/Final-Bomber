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

        Point size;
        Vector2 position;

        #endregion

        #region Property Region

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Point Size
        {
            get { return size; }
            set { size = value; }
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

            size = size;
            position = position;
        }

        #endregion

        public void Draw(SpriteBatch spriteBatch)
        {
            // Top corners
            spriteBatch.Draw(windowSkin, position, topLeft, Color.White);
            spriteBatch.Draw(windowSkin, new Vector2(position.X + size.X - 12, position.Y), topRight, Color.White);

            // Bottom corners
            spriteBatch.Draw(windowSkin, new Vector2(position.X, position.Y + size.Y - 12), bottomLeft, Color.White);
            spriteBatch.Draw(windowSkin, new Vector2(position.X + size.X - 12, position.Y + size.Y - 12), bottomRight, Color.White);

            for (int y = 12; y < size.Y - 12; y++)
            {
                spriteBatch.Draw(windowSkin, new Vector2(position.X, position.Y + y), left, Color.White);
                spriteBatch.Draw(windowSkin, new Vector2(position.X + size.X - 10, position.Y + y), right, Color.White);
            }

            for (int x = 12; x < size.X - 12; x++)
            {
                spriteBatch.Draw(windowSkin, new Vector2(position.X + x, position.Y), top, Color.White);
                spriteBatch.Draw(windowSkin, new Vector2(position.X + x, position.Y + size.Y - 10), bottom, Color.White);
            }


            for (int x = 10; x < size.X - 10; x++)
                for (int y = 10; y < size.Y - 10; y++)
                    spriteBatch.Draw(windowSkin, new Vector2(position.X + x, position.Y + y), back, Color.White);
        }
    }
}

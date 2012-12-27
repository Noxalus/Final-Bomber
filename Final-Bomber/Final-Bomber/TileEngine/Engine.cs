using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Final_Bomber.TileEngine
{
    public class Engine
    {
        #region Field Region

        static int tileWidth;
        static int tileHeight;
        static Vector2 origin;

        #endregion

        #region Property Region

        public static int TileWidth
        {
            get { return tileWidth; }
        }

        public static int TileHeight
        {
            get { return tileHeight; }
        }

        public static Vector2 Origin
        {
            get { return origin; }
            set { origin = value; }
        }
        
        #endregion

        #region Constructors

        public Engine(int tileWidth, int tileHeight, Vector2 origin)
        {
            Engine.tileWidth = tileWidth;
            Engine.tileHeight = tileHeight;
            Engine.origin = origin;
        }

        #endregion

        #region Methods

        public static Point VectorToCell(Vector2 position, Point dimension)
        {
            if (dimension.X != tileWidth && dimension.Y != tileHeight)
            {
                dimension = new Point(tileWidth, tileHeight);
                int positionX = 0;
                int positionY = 0;
                if (position.X > 0)
                    positionX = (int)(position.X - (dimension.X / 2)) / tileWidth + 1;
                if(position.Y > 0)
                    positionY = (int)(position.Y - (dimension.Y) / 4) / tileHeight + 1;

                return new Point(positionX, positionY);
            }
            else
                return new Point((int)(position.X - (dimension.X / 2)) / tileWidth + 1, (int)(position.Y - (dimension.Y / 2)) / tileHeight + 1);
        }

        public static Point VectorToCell(Vector2 position)
        {
            return new Point((int)position.X / tileWidth, (int)position.Y / tileHeight);
        }

        public static Vector2 CellToVector(Point position)
        {
            return new Vector2((float)(position.X * tileWidth), (float)(position.Y * tileHeight));
        }

        public static void ChangeOrigin(float x, float y)
        {
            origin.X = x;
            origin.Y = y;
        }

        public static void ChangeXOrigin(float x)
        {
            origin.X = x;
        }

        public static void ChangeYOrigin(float y)
        {
            origin.Y = y;
        }

        #endregion
    }
}

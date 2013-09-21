
using Microsoft.Xna.Framework;
namespace FBLibrary.Core
{
    public enum EntityType
    {
        Void = 0,
        UnbreakableWall = 1,
        EdgeWall = 2,
        Wall = 3,
        Player = 4,
        Teleporter = 6,
        Arrow = 7
    };

    public enum LookDirection
    {
        Idle = 0,
        Up = 1,
        Right = 2,
        Down = 3,
        Left = 4
    }

    public class Engine
    {
        #region Field Region

        static int _tileWidth;
        static int _tileHeight;
        static Vector2 _origin;

        #endregion

        #region Property Region

        public static int TileWidth
        {
            get { return _tileWidth; }
        }

        public static int TileHeight
        {
            get { return _tileHeight; }
        }

        public static Vector2 Origin
        {
            get { return _origin; }
            set { _origin = value; }
        }

        #endregion

        #region Constructors

        public Engine(int tileWidth, int tileHeight, Vector2 origin)
        {
            Engine._tileWidth = tileWidth;
            Engine._tileHeight = tileHeight;
            Engine._origin = origin;
        }

        #endregion

        #region Methods

        public static Point VectorToCell(Vector2 position, Point dimension)
        {
            if (dimension.X != _tileWidth && dimension.Y != _tileHeight)
            {
                dimension = new Point(_tileWidth, _tileHeight);
                int positionX = 0;
                int positionY = 0;
                if (position.X > 0)
                    positionX = (int)(position.X - (dimension.X / 2f)) / _tileWidth + 1;
                if (position.Y > 0)
                    positionY = (int)(position.Y - (dimension.Y) / 4f) / _tileHeight + 1;

                return new Point(positionX, positionY);
            }
            else
                return new Point((int)(position.X - (dimension.X / 2f)) / _tileWidth + 1, (int)(position.Y - (dimension.Y / 2f)) / _tileHeight + 1);
        }

        public static Point VectorToCell(Vector2 position)
        {
            return new Point((int)position.X / _tileWidth, (int)position.Y / _tileHeight);
        }

        public static Vector2 CellToVector(Point position)
        {
            return new Vector2((float)(position.X * _tileWidth), (float)(position.Y * _tileHeight));
        }

        public static void ChangeOrigin(float x, float y)
        {
            _origin.X = x;
            _origin.Y = y;
        }

        public static void ChangeXOrigin(float x)
        {
            _origin.X = x;
        }

        public static void ChangeYOrigin(float y)
        {
            _origin.Y = y;
        }

        #endregion
    }
}

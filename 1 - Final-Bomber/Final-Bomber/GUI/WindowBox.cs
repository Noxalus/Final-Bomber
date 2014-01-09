using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FBClient.GUI
{
    public class WindowBox
    {
        #region Field Region

        readonly Texture2D _windowSkin;

        readonly Rectangle _topLeft;
        readonly Rectangle _topRight;
        readonly Rectangle _bottomLeft;
        readonly Rectangle _bottomRight;

        readonly Rectangle _top;
        readonly Rectangle _back;
        readonly Rectangle _bottom;
        readonly Rectangle _left;
        readonly Rectangle _right;

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
            _windowSkin = window;

            _topLeft = new Rectangle(0, 0, 12, 12);
            _topRight = new Rectangle(13, 0, 12, 12);
            _bottomLeft = new Rectangle(0, 13, 12, 12);
            _bottomRight = new Rectangle(13, 13, 12, 12);

            _top = new Rectangle(12, 0, 1, 10);
            _back = new Rectangle(12, 12, 1, 1);
            _bottom = new Rectangle(12, 15, 1, 10);
            _left = new Rectangle(0, 12, 10, 1);
            _right = new Rectangle(15, 12, 10, 1);

            _size = size;
            _position = position;
        }

        #endregion

        public void Draw(SpriteBatch spriteBatch)
        {
            // Top corners
            spriteBatch.Draw(_windowSkin, _position, _topLeft, Color.White);
            spriteBatch.Draw(_windowSkin, new Vector2(_position.X + _size.X - 12, _position.Y), _topRight, Color.White);

            // Bottom corners
            spriteBatch.Draw(_windowSkin, new Vector2(_position.X, _position.Y + _size.Y - 12), _bottomLeft, Color.White);
            spriteBatch.Draw(_windowSkin, new Vector2(_position.X + _size.X - 12, _position.Y + _size.Y - 12), _bottomRight, Color.White);

            for (int y = 12; y < _size.Y - 12; y++)
            {
                spriteBatch.Draw(_windowSkin, new Vector2(_position.X, _position.Y + y), _left, Color.White);
                spriteBatch.Draw(_windowSkin, new Vector2(_position.X + _size.X - 10, _position.Y + y), _right, Color.White);
            }

            for (int x = 12; x < _size.X - 12; x++)
            {
                spriteBatch.Draw(_windowSkin, new Vector2(_position.X + x, _position.Y), _top, Color.White);
                spriteBatch.Draw(_windowSkin, new Vector2(_position.X + x, _position.Y + _size.Y - 10), _bottom, Color.White);
            }


            for (int x = 10; x < _size.X - 10; x++)
                for (int y = 10; y < _size.Y - 10; y++)
                    spriteBatch.Draw(_windowSkin, new Vector2(_position.X + x, _position.Y + y), _back, Color.White);
        }
    }
}

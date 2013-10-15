using Final_Bomber.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Final_Bomber.WorldEngine
{
    public class Camera2D
    {
        private const float zoomUpperLimit = 5.0f;
        private const float zoomLowerLimit = .01f;

        private float _zoom;
        private Matrix _transform;
        private Vector2 _position;
        private Vector2 _positionLag;
        private float _rotation;
        private int _viewportWidth;
        private int _viewportHeight;
        private int _worldWidth;
        private int _worldHeight;

        private Vector2 _center;

        public Camera2D(Viewport viewport, Point worldSize, float initialZoom)
        {
            _zoom = initialZoom;
            _rotation = 0.0f;
            _position = Vector2.Zero;
            _positionLag = Vector2.Zero;
            _viewportWidth = viewport.Width;
            _viewportHeight = viewport.Height;
            _worldWidth = worldSize.X;
            _worldHeight = worldSize.Y;
        }

        #region Properties

        public float Zoom
        {
            get { return _zoom; }
            set
            {
                _zoom = value;
                if (_zoom < zoomLowerLimit)
                    _zoom = zoomLowerLimit;
                if (_zoom > zoomUpperLimit)
                    _zoom = zoomUpperLimit;
            }
        }

        public float Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        public void Move(Vector2 amount)
        {
            _position += amount;
        }

        public Vector2 Position
        {
            get { return _position; }
            set
            {
                _position = value;
                /*
                float leftBarrier = (float)_viewportWidth * .5f / _zoom;
                float rightBarrier = _worldWidth - (float)_viewportWidth * .5f / _zoom;
                float topBarrier = _worldHeight - (float)_viewportHeight * .5f / _zoom;
                float bottomBarrier = (float)_viewportHeight * .5f / _zoom;

                _position = value;
                
                if (_position.X < leftBarrier)
                    _position.X = leftBarrier;
                if (_position.X > rightBarrier)
                    _position.X = rightBarrier;
                if (_position.Y > topBarrier)
                    _position.Y = topBarrier;
                if (_position.Y < bottomBarrier)
                    _position.Y = bottomBarrier;
                */
            }
        }

        #endregion

        public void Update(GameTime gameTime, Vector2 position)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Position = position;

            // Adjust zoom if the mouse wheel has moved
            if (InputHandler.ScrollUp())
            {
                if (InputHandler.KeyPressed(Keys.LeftControl))
                    Zoom += 0.01f;
                else
                    Zoom += 0.1f;
            }
            else if (InputHandler.ScrollDown())
            {
                if (InputHandler.KeyPressed(Keys.LeftControl))
                    Zoom -= 0.01f;
                else
                    Zoom -= 0.1f;
            }

            // Move the camera when the arrow keys are pressed
            Vector2 movement = Vector2.Zero;

            if (InputHandler.KeyDown(Keys.J))
                movement.X -= 1f;
            if (InputHandler.KeyDown(Keys.L))
                movement.X += 1f;
            if (InputHandler.KeyDown(Keys.I))
                movement.Y -= 1f;
            if (InputHandler.KeyDown(Keys.K))
                movement.Y += 1f;

            // Reset camera lag
            if (InputHandler.KeyPressed(Keys.End))
            {
                _positionLag = Vector2.Zero;
            }
            else
            {
                _positionLag += movement * 150 * dt;
            }

            Position += _positionLag;

            // Rotation
            if (InputHandler.KeyDown(Keys.PageDown))
                Rotation -= dt;
            else if (InputHandler.KeyDown(Keys.PageUp))
                Rotation += dt;


            _center = new Vector2(Position.X, Position.Y);

        }

        public Matrix GetTransformation()
        {
            _transform =
               Matrix.CreateTranslation(new Vector3(-_center.X, -_center.Y, 0)) *
               Matrix.CreateRotationZ(Rotation) *
               Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
               Matrix.CreateTranslation(new Vector3(_viewportWidth * 0.5f, _viewportHeight * 0.5f, 0)
               );

            return _transform;
        }
    }
}

using FBLibrary.Core;
using FBClient.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FBClient.WorldEngine
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
        private Viewport _viewport;
        private int _worldWidth;
        private int _worldHeight;

        private Vector2 _center;

        public Camera2D(Viewport viewport, Point worldSize, float initialZoom)
        {
            _zoom = initialZoom;
            _rotation = 0.0f;
            _position = Vector2.Zero;
            _positionLag = Vector2.Zero;
            _viewport = viewport;
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

                
                float leftBarrier = (float)_viewport.Width * .5f / _zoom;
                float rightBarrier = _worldWidth - (float)_viewport.Width * .5f / _zoom;
                float topBarrier = _worldHeight - (float)_viewport.Height * .5f / _zoom;
                float bottomBarrier = (float)_viewport.Height * .5f / _zoom;

                
                _position.X = MathHelper.Clamp(_position.X, leftBarrier, rightBarrier);
                _position.Y = MathHelper.Clamp(_position.Y, bottomBarrier, topBarrier);
            }
        }

        public Rectangle ViewportRectangle
        {
            get { return _viewport.Bounds; }
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
                movement.X += 1f;
            if (InputHandler.KeyDown(Keys.L))
                movement.X -= 1f;
            if (InputHandler.KeyDown(Keys.I))
                movement.Y += 1f;
            if (InputHandler.KeyDown(Keys.K))
                movement.Y -= 1f;

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
            {
                if (InputHandler.KeyDown(Keys.LeftControl))
                    Rotation -= 10 * dt;
                else
                    Rotation -= dt;
            }
            else if (InputHandler.KeyDown(Keys.PageUp))
            {
                if (InputHandler.KeyDown(Keys.LeftControl))
                    Rotation += 10 * dt;
                else
                    Rotation += dt;
            }

            _center = new Vector2(
                Position.X + Engine.Origin.X + Engine.TileWidth / 2f,
                Position.Y + Engine.Origin.Y + Engine.TileHeight / 2f
            );

        }

        public Matrix GetTransformation()
        {
            _transform =
               Matrix.CreateTranslation(new Vector3(-_center.X, -_center.Y, 0)) *
               Matrix.CreateRotationZ(Rotation) *
               Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
               Matrix.CreateTranslation(new Vector3(_viewport.Width * 0.5f, _viewport.Height * 0.5f, 0)
               );

            return _transform;
        }
    }
}

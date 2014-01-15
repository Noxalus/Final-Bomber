using System;
using FBLibrary;
using FBLibrary.Core;
using FBClient.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FBClient.WorldEngine
{
    public class Camera2D
    {
        #region Fields

        private const float ZoomUpperLimit = 5.0f;
        private const float ZoomLowerLimit = .01f;

        private float _zoom;
        private float _initialZoom;
        private Matrix _transform;
        private Vector2 _position;
        private Vector2 _positionLag;
        private float _rotation;
        private Viewport _viewport;
        private readonly int _worldWidth;
        private readonly int _worldHeight;

        private Vector2 _center;

        #endregion

        #region Properties

        public float Zoom
        {
            get { return _zoom; }
            set
            {
                _zoom = value;
                if (_zoom < ZoomLowerLimit)
                    _zoom = ZoomLowerLimit;
                if (_zoom > ZoomUpperLimit)
                    _zoom = ZoomUpperLimit;
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
                float leftBarrier = (float)_viewport.Width * .5f / _zoom;
                float rightBarrier = _worldWidth - (float)_viewport.Width * .5f / _zoom;
                float topBarrier = _worldHeight - (float)_viewport.Height * .5f / _zoom;
                float bottomBarrier = (float)_viewport.Height * .5f / _zoom;


                _position.X = MathHelper.Clamp(_position.X, leftBarrier, rightBarrier);
                _position.Y = MathHelper.Clamp(_position.Y, bottomBarrier, topBarrier);
                */
            }
        }

        public Rectangle ViewportRectangle
        {
            get { return _viewport.Bounds; }
        }

        #endregion


        public Camera2D(Viewport viewport, Point worldSize, float initialZoom)
        {
            _initialZoom = initialZoom;
            _zoom = _initialZoom;
            _rotation = 0.0f;
            _position = Vector2.Zero;
            _positionLag = Vector2.Zero;
            _viewport = viewport;
            _worldWidth = worldSize.X;
            _worldHeight = worldSize.Y;
        }

        public void Update(Vector2 position)
        {
            var dt = (float)TimeSpan.FromTicks(GameConfiguration.DeltaTime).TotalSeconds;

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

            // Reset zoom
            if (InputHandler.KeyPressed(Keys.Home))
            {
                Zoom = _initialZoom;
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
            if (InputHandler.KeyPressed(Keys.Delete))
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

            // Reset rotation
            if (InputHandler.KeyPressed(Keys.End))
            {
                Rotation = 0;
            }

            /*
            // Center camera on the player
            Position = new Vector2(
                Position.X + Engine.Origin.X + Engine.TileWidth / 2f,
                Position.Y + Engine.Origin.Y + Engine.TileHeight / 2f
            );*/
        }

        public Matrix GetTransformation()
        {
            _transform =
               Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0)) *
               Matrix.CreateRotationZ(Rotation) *
               Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
               Matrix.CreateTranslation(new Vector3(_viewport.Width * 0.5f, _viewport.Height * 0.5f, 0)
               );

            return _transform;
        }
    }
}

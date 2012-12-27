using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Final_Bomber.TileEngine;

namespace Final_Bomber.Sprites
{
    public class AnimatedSprite
    {
        #region Field Region

        Dictionary<AnimationKey, Animation> animations;
        AnimationKey currentAnimation;
        bool isAnimating;

        Texture2D texture;
        Vector2 position;
        private Point cellPosition;
        Vector2 velocity;
        float speed = 2.5f;

        Animation animation;

        bool controlable;

        #endregion

        #region Property Region

        public AnimationKey CurrentAnimation
        {
            get { return currentAnimation; }
            set { currentAnimation = value; }
        }

        public Animation[] Animations
        {
            get 
            {
                Animation[] anim = new Animation[animations.Count]; 
                int i = 0;
                foreach (Animation a in animations.Values)
                {
                    anim[i] = a;
                    i++;
                }
                return anim;
            }
        }

        public Animation Animation
        {
            get { return animation; }
        }

        public bool IsAnimating
        {
            get { return isAnimating; }
            set { isAnimating = value; }
        }

        public int Width
        {
            get 
            {
                if (controlable)
                    return animations[currentAnimation].FrameWidth;
                else
                    return animation.FrameWidth;
            }
        }

        public int Height
        {
            get 
            {
                if (controlable)
                    return animations[currentAnimation].FrameHeight;
                else
                    return animation.FrameHeight;
            }
        }

        public Point Dimension
        {
            get { return new Point(Width, Height); }
        }

        public float Speed
        {
            get { return speed; }
            set { speed = MathHelper.Clamp(value, Config.PlayerSpeedIncrementeur, Config.MaxSpeed); }
        }

        public Vector2 Position
        {
            get { return position; }
            set
            { position = value; }
        }

        public float PositionX
        {
            set {  position.X = value; }
            get { return position.X; }
        }

        public float PositionY
        {
            set { position.Y = value; }
            get { return position.Y; }
        }

        public int CellPositionX
        {
            set { cellPosition.X = value; }
        }

        public int CellPositionY
        {
            set { cellPosition.Y = value; }
        }

        public Point CellPosition
        {
            get { return cellPosition; }
        }

        public Vector2 Velocity
        {
            get { return velocity; }
            set
            {
                velocity = value;
                if (velocity != Vector2.Zero)
                    velocity.Normalize();
            }
        }

        #endregion

        #region Constructor Region

        public AnimatedSprite(Texture2D sprite, Dictionary<AnimationKey, Animation> animation, Vector2 position)
        {
            texture = sprite;
            animations = new Dictionary<AnimationKey, Animation>();

            foreach (AnimationKey key in animation.Keys)
                animations.Add(key, (Animation)animation[key].Clone());

            controlable = true;
            this.position = position;
            cellPosition = Engine.VectorToCell(position, Dimension);
        }

        public AnimatedSprite(Texture2D sprite, Animation animation, Vector2 position)
        {
            texture = sprite;
            this.animation = animation;
            controlable = false;
            this.position = position;
            cellPosition = Engine.VectorToCell(position, Dimension);
        }

        #endregion

        #region XNA Method Region

        public void Update(GameTime gameTime)
        {
            if (controlable)
            {
                if (isAnimating)
                    animations[currentAnimation].Update(gameTime);
                else
                    animations[currentAnimation].Reset();
            }
            else
            {
                if (isAnimating)
                    animation.Update(gameTime);
                else
                    animation.Reset();
            }

            cellPosition = Engine.VectorToCell(position, Dimension);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Rectangle? sourceRectangle;
            if(controlable)
                sourceRectangle = animations[currentAnimation].CurrentFrameRect;
            else
                sourceRectangle = animation.CurrentFrameRect;

            if (Width != Engine.TileWidth || Height != Engine.TileHeight)
            {
                spriteBatch.Draw(
                    texture,
                    new Rectangle(
                        (int)(Engine.Origin.X + position.X - Engine.TileWidth / 4),
                        (int)(Engine.Origin.Y + position.Y - Engine.TileHeight / 2),
                        Engine.TileWidth + Engine.TileWidth / 2,
                        Engine.TileHeight + Engine.TileHeight / 2),
                    sourceRectangle,
                    Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
            }
            else
            {
                spriteBatch.Draw(
                texture,
                new Vector2(Engine.Origin.X + position.X, Engine.Origin.Y + position.Y),
                sourceRectangle,
                Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            }
        }
        #endregion

        #region Method Regions

        public void LockToMap()
        {
            position.X = MathHelper.Clamp(position.X, 0, TileMap.WidthInPixels - Width);
            position.Y = MathHelper.Clamp(position.Y, 0, TileMap.HeightInPixels - Height);
        }

        public void ChangeFramesPerSecond(int newValue)
        {
            if (controlable)
            {
                foreach (Animation a in animations.Values)
                    a.FramesPerSecond = newValue;
            }
            else
                animation.FramesPerSecond = newValue;
        }

        public void ChangePosition(Point p)
        {
            cellPosition = p;
            position = Engine.CellToVector(cellPosition);
        }

        public void ChangePosition(int x, int y)
        {
            cellPosition.X = x;
            cellPosition.Y = y;
            position = Engine.CellToVector(cellPosition);
        }

        public void ChangePosition(float x, float y)
        {
            position.X = x;
            position.Y = y;
            cellPosition = Engine.VectorToCell(position);
        }
        #endregion
    }
}

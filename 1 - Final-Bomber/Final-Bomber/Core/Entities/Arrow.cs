using System.Collections.Generic;
using FBClient.WorldEngine;
using FBLibrary.Core;
using FBClient.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FBClient.Core.Entities
{
    public class Arrow : DynamicEntity
    {
        #region Field Region

        private readonly LookDirection _lookDirection;
        private bool _isAlive;
        public AnimatedSprite Sprite { get; protected set; }

        #endregion

        #region Property Region

        #endregion

        #region Constructor Region

        public Arrow(Point position, LookDirection initialLookDirection)
        {
            _isAlive = true;
            _lookDirection = initialLookDirection;
        }

        #endregion

        #region XNA Method Region

        public void LoadContent()
        {
            const int animationFramesPerSecond = 10;
            var animations = new Dictionary<AnimationKey, Animation>();

            var animation = new Animation(1, 32, 32, 0, 0, animationFramesPerSecond);
            animations.Add(AnimationKey.Down, animation);

            animation = new Animation(1, 32, 32, 0, 32, animationFramesPerSecond);
            animations.Add(AnimationKey.Left, animation);

            animation = new Animation(1, 32, 32, 0, 64, animationFramesPerSecond);
            animations.Add(AnimationKey.Right, animation);

            animation = new Animation(1, 32, 32, 0, 96, animationFramesPerSecond);
            animations.Add(AnimationKey.Up, animation);

            var spriteTexture = FinalBomber.Instance.Content.Load<Texture2D>("Graphics/Sprites/arrow");

            Sprite = new AnimatedSprite(spriteTexture, animations)
            {
                IsAnimating = true,
                CurrentAnimation = LookDirectionToAnimationKey(_lookDirection)
            };
        }

        public void Update(GameTime gameTime)
        {
            Sprite.Update(gameTime);
        }

        public void Draw(GameTime gameTime, Camera2D camera)
        {
            if (camera.IsVisible(Position))
                Sprite.Draw(gameTime, FinalBomber.Instance.SpriteBatch, Position);
        }

        #endregion

        #region Private Method Region

        private AnimationKey LookDirectionToAnimationKey(LookDirection lookDirection)
        {
            var animationKey = AnimationKey.Up;
            switch (lookDirection)
            {
                case LookDirection.Up:
                    animationKey = AnimationKey.Up;
                    break;
                case LookDirection.Down:
                    animationKey = AnimationKey.Down;
                    break;
                case LookDirection.Right:
                    animationKey = AnimationKey.Right;
                    break;
                case LookDirection.Left:
                    animationKey = AnimationKey.Left;
                    break;
            }
            return animationKey;
        }

        #endregion

        #region Public Method Region

        public void ChangeDirection(Bomb bomb)
        {
            Point nextPosition = bomb.CellPosition;
            switch (_lookDirection)
            {
                case LookDirection.Up:
                    nextPosition.Y--;
                    break;
                case LookDirection.Down:
                    nextPosition.Y++;
                    break;
                case LookDirection.Left:
                    nextPosition.X--;
                    break;
                case LookDirection.Right:
                    nextPosition.X++;
                    break;
            }

            if (
                !FinalBomber.Instance.GamePlayScreen.World.Levels[FinalBomber.Instance.GamePlayScreen.World.CurrentLevel
                    ].
                    CollisionLayer[nextPosition.X, nextPosition.Y])
            {
                bomb.ChangeDirection(_lookDirection, -1);
                //bomb.ChangeSpeed(bomb.Speed + Config.BombSpeedIncrementeur);
                bomb.ResetTimer();
            }
        }

        #endregion

        #region Override Method Region

        public override void Destroy()
        {
        }

        public override void Remove()
        {
            _isAlive = false;
        }

        #endregion
    }
}
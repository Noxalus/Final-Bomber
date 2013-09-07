using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Final_Bomber.Sprites;

namespace Final_Bomber.Components
{
    public class Arrow : Entity
    {
        #region Field Region
        public override sealed Sprites.AnimatedSprite Sprite { get; protected set; }
        private readonly FinalBomber _gameRef;
        private bool _isAlive;
        private readonly LookDirection _lookDirection;
        #endregion

        #region Property Region

        public bool IsAlive
        {
            get { return _isAlive; }
        }

        #endregion

        #region Constructor Region
        public Arrow(FinalBomber game, Vector2 position, LookDirection initialLookDirection)
        {
            this._gameRef = game;

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

            var spriteTexture = _gameRef.Content.Load<Texture2D>("Graphics/Characters/arrow");
            Sprite = new Sprites.AnimatedSprite(spriteTexture, animations, position) {IsAnimating = true};

            _lookDirection = initialLookDirection;
            Sprite.CurrentAnimation = LookDirectionToAnimationKey(_lookDirection);

            _isAlive = true;
        }
        #endregion

        #region XNA Method Region

        public override void Update(GameTime gameTime)
        {
            Sprite.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Sprite.Draw(gameTime, _gameRef.SpriteBatch);
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
            Point nextPosition = bomb.Sprite.CellPosition;
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

            if (!_gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel].
                CollisionLayer[nextPosition.X, nextPosition.Y])
            {
                bomb.ChangeDirection(_lookDirection, -1);
                //bomb.ChangeSpeed(bomb.Sprite.Speed + Config.BombSpeedIncrementeur);
                bomb.ResetTimer();
            }
        }

        #endregion

        #region Override Method Region
        public override void Destroy()
        {
        }

        public override  void Remove()
        {
            this._isAlive = false;
        }
        #endregion
    }
}

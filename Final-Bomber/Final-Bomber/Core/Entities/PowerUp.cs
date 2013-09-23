using System.Collections.Generic;
using FBLibrary.Core;
using FBLibrary.Core.BaseEntities;
using Final_Bomber.Entities;
using Final_Bomber.Screens;
using Final_Bomber.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Final_Bomber.Core.Entities
{


    public class PowerUp : BasePowerUp
    {
        #region Field Region

        private AnimatedSprite _itemDestroyAnimation;
        public AnimatedSprite Sprite { get; protected set; }

        #endregion
        
        #region Constructor Region

        public PowerUp(Point cellPosition) : base(cellPosition)
        {
            Type = Config.ItemTypeAvaible[GamePlayScreen.Random.Next(Config.ItemTypeAvaible.Count)];
            Initialize();
        }

        public PowerUp(Point cellPosition, PowerUpType type) : base(cellPosition)
        {
            Type = type;
            Initialize();
        }

        #endregion

        private void Initialize()
        {
            var animations = new Dictionary<AnimationKey, Animation>();
            var animation = new Animation(2, 32, 32, 0, Config.ItemTypeIndex[Type] * 32, 5);

            var spriteTexture = FinalBomber.Instance.Content.Load<Texture2D>("Graphics/Characters/item");
            Sprite = new AnimatedSprite(spriteTexture, animation) { IsAnimating = true };

            var itemDestroyTexture = FinalBomber.Instance.Content.Load<Texture2D>("Graphics/Characters/itemDestroy");
            animation = new Animation(7, 31, 28, 0, 0, 8);
            _itemDestroyAnimation = new AnimatedSprite(itemDestroyTexture, animation)
            {
                IsAnimating = false
            };
        }

        #region XNA Method Region

        public void Update(GameTime gameTime)
        {
            Sprite.Update(gameTime);

            if (InDestruction)
            {
                _itemDestroyAnimation.Update(gameTime);
                if (_itemDestroyAnimation.Animation.CurrentFrame == _itemDestroyAnimation.Animation.FrameCount - 1)
                    Remove();
            }
        }

        public void Draw(GameTime gameTime)
        {
            Sprite.Draw(gameTime, FinalBomber.Instance.SpriteBatch, Position);

            if (_itemDestroyAnimation.IsAnimating)
                _itemDestroyAnimation.Draw(gameTime, FinalBomber.Instance.SpriteBatch, Position);
        }

        #endregion

        #region Method Region

        public override void Destroy()
        {
            if (!_itemDestroyAnimation.IsAnimating)
            {
                InDestruction = true;
                _itemDestroyAnimation.IsAnimating = true;
            }
        }

        public override void Remove()
        {
            IsAlive = false;
        }

        #endregion
    }
}
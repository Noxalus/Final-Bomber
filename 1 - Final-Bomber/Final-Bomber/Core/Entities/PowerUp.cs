using System.Collections.Generic;
using FBLibrary.Core;
using FBLibrary.Core.BaseEntities;
using FBClient.Entities;
using FBClient.Screens;
using FBClient.Screens.GameScreens;
using FBClient.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace FBClient.Core.Entities
{


    public class PowerUp : BasePowerUp
    {
        #region Field Region

        // Graphics
        private AnimatedSprite _itemDestroyAnimation;
        public AnimatedSprite Sprite { get; protected set; }

        // Sounds
        private SoundEffect _powerUpPickUpSound;

        #endregion
        
        #region Constructor Region

        public PowerUp(Point cellPosition)
            : base(cellPosition)
        {
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

            // Sounds
            _powerUpPickUpSound = FinalBomber.Instance.Content.Load<SoundEffect>("Audio/Sounds/item");
        }

        #region XNA Method Region

        public void Update(GameTime gameTime)
        {
            Sprite.Update(gameTime);

            if (InDestruction)
            {
                _itemDestroyAnimation.Update(gameTime);
            }

            base.Update();
        }

        public void Draw(GameTime gameTime)
        {
            Sprite.Draw(gameTime, FinalBomber.Instance.SpriteBatch, Position);

            if (_itemDestroyAnimation.IsAnimating)
                _itemDestroyAnimation.Draw(gameTime, FinalBomber.Instance.SpriteBatch, Position);
        }

        #endregion

        #region Method Region

        public override void PickUp()
        {
            _powerUpPickUpSound.Play();

            base.PickUp();
        }

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
            InDestruction = false;
        }

        #endregion
    }
}
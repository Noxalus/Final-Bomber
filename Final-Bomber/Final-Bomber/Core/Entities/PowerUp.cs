using System.Collections.Generic;
using FBLibrary.Core;
using Final_Bomber.Entities;
using Final_Bomber.Screens;
using Final_Bomber.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Final_Bomber.Core.Entities
{


    public class PowerUp : BaseEntity, IEntity
    {
        #region Field Region

        private readonly AnimatedSprite _itemDestroyAnimation;
        private readonly ItemType _type;
        private bool _inDestruction;
        private bool _isAlive;
        public AnimatedSprite Sprite { get; protected set; }

        #endregion

        #region Property Region

        public bool IsAlive
        {
            get { return _isAlive; }
        }

        public bool InDestruction
        {
            get { return _inDestruction; }
        }

        public ItemType Type
        {
            get { return _type; }
        }

        #endregion

        #region Constructor Region

        public PowerUp(Point position)
        {
            _type = Config.ItemTypeAvaible[GamePlayScreen.Random.Next(Config.ItemTypeAvaible.Count)];

            var animations = new Dictionary<AnimationKey, Animation>();
            var animation = new Animation(2, 32, 32, 0, Config.ItemTypeIndex[_type]*32, 5);

            var spriteTexture = FinalBomber.Instance.Content.Load<Texture2D>("Graphics/Characters/item");
            Sprite = new AnimatedSprite(spriteTexture, animation, position) {IsAnimating = true};

            var itemDestroyTexture = FinalBomber.Instance.Content.Load<Texture2D>("Graphics/Characters/itemDestroy");
            animation = new Animation(7, 31, 28, 0, 0, 8);
            _itemDestroyAnimation = new AnimatedSprite(itemDestroyTexture, animation, Sprite.Position)
            {
                IsAnimating = false
            };

            _inDestruction = false;
            _isAlive = true;
        }

        #endregion

        #region XNA Method Region

        public void Update(GameTime gameTime)
        {
            Sprite.Update(gameTime);

            if (_inDestruction)
            {
                _itemDestroyAnimation.Update(gameTime);
                if (_itemDestroyAnimation.Animation.CurrentFrame == _itemDestroyAnimation.Animation.FrameCount - 1)
                    Remove();
            }
        }

        public void Draw(GameTime gameTime)
        {
            Sprite.Draw(gameTime, FinalBomber.Instance.SpriteBatch);

            if (_itemDestroyAnimation.IsAnimating)
                _itemDestroyAnimation.Draw(gameTime, FinalBomber.Instance.SpriteBatch);
        }

        #endregion

        #region Method Region

        public void ApplyItem(Player p)
        {
            switch (_type)
            {
                    // More power
                case ItemType.Power:
                    p.IncreasePower(1);
                    break;
                    // More bombs
                case ItemType.Bomb:
                    p.IncreaseTotalBombNumber(1);
                    break;
                    // More speed
                case ItemType.Speed:
                    p.IncreaseSpeed(Config.PlayerSpeedIncrementeur);
                    break;
                    // Skeleton ! => Bad items
                case ItemType.BadItem:
                    int randomBadEffect = GamePlayScreen.Random.Next(Config.BadItemEffectList.Count);
                    p.ApplyBadItem(Config.BadItemEffectList[randomBadEffect]);
                    break;
                    // More points
                case ItemType.Point:
                    Config.PlayersScores[p.Id]++;
                    break;
            }
        }

        public void Destroy()
        {
            if (!_itemDestroyAnimation.IsAnimating)
            {
                _inDestruction = true;
                _itemDestroyAnimation.IsAnimating = true;
            }
        }

        public void Remove()
        {
            _isAlive = false;
        }

        #endregion
    }
}
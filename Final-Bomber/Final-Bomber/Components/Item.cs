using System.Collections.Generic;
using Final_Bomber.Screens;
using Microsoft.Xna.Framework;
using Final_Bomber.Sprites;
using Microsoft.Xna.Framework.Graphics;

namespace Final_Bomber.Components
{
    public enum ItemType { Power, Bomb, Speed, Point, BadItem }
    public enum BadItemEffect { NoBomb, BombDrop, BombTimerChanged, TooSpeed, TooSlow, KeysInversion }

    public class Item : Entity
    {
        #region Field Region

        public override sealed AnimatedSprite Sprite { get; protected set; }

        private readonly FinalBomber _gameRef;
        private readonly ItemType _type;
        private bool _inDestruction;
        private bool _isAlive;

        readonly AnimatedSprite _itemDestroyAnimation;
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
        public Item(FinalBomber game, Vector2 position)
        {
            _gameRef = game;

            _type = Config.ItemTypeAvaible[GamePlayScreen.Random.Next(Config.ItemTypeAvaible.Count)];

            var animations = new Dictionary<AnimationKey, Animation>();
            var animation = new Animation(2, 32, 32, 0, Config.ItemTypeIndex[_type] * 32, 5);

            var spriteTexture = _gameRef.Content.Load<Texture2D>("Graphics/Characters/item");
            Sprite = new AnimatedSprite(spriteTexture, animation, position) {IsAnimating = true};

            var itemDestroyTexture = _gameRef.Content.Load<Texture2D>("Graphics/Characters/itemDestroy");
            animation = new Animation(7, 31, 28, 0, 0, 8);
            _itemDestroyAnimation = new AnimatedSprite(itemDestroyTexture, animation, this.Sprite.Position)
                {
                    IsAnimating = false
                };

            _inDestruction = false;
            _isAlive = true;
        }
        #endregion

        #region XNA Method Region
        public override void Update(GameTime gameTime)
        {
            Sprite.Update(gameTime);

            if (_inDestruction)
            {
                _itemDestroyAnimation.Update(gameTime);
                if (_itemDestroyAnimation.Animation.CurrentFrame == _itemDestroyAnimation.Animation.FrameCount - 1)
                    Remove();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Sprite.Draw(gameTime, _gameRef.SpriteBatch);

            if (_itemDestroyAnimation.IsAnimating)
                _itemDestroyAnimation.Draw(gameTime, _gameRef.SpriteBatch);
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
                    Config.PlayersScores[p.Id - 1]++;
                    break;
            }
        }

        public override void Destroy()
        {
            if (!this._itemDestroyAnimation.IsAnimating)
            {
                this._inDestruction = true;
                this._itemDestroyAnimation.IsAnimating = true;
            }
        }

        public override void Remove()
        {
            this._isAlive = false;
        }
        #endregion
    }
}

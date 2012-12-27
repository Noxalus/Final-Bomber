using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Final_Bomber.Sprites;
using Microsoft.Xna.Framework.Graphics;
using Final_Bomber.TileEngine;

namespace Final_Bomber.Components
{
    public enum ItemType { Power, Bomb, Speed, Point, BadItem }
    public enum BadItemEffect { NoBomb, BombDrop, BombTimerChanged, TooSpeed, TooSlow, KeysInversion }

    public class Item : MapItem
    {
        #region Field Region

        public override AnimatedSprite Sprite { get; protected set; }

        private FinalBomber gameRef;
        private ItemType type;
        private bool inDestruction;
        private bool isAlive;

        AnimatedSprite itemDestroyAnimation;
        #endregion

        #region Property Region

        public bool IsAlive
        {
            get { return isAlive; }
        }

        public bool InDestruction
        {
            get { return inDestruction; }
        }

        public ItemType Type
        {
            get { return type; }
        }

        #endregion

        #region Constructor Region
        public Item(FinalBomber game, Vector2 position)
        {
            gameRef = game;

            type = Config.ItemTypeAvaible[gameRef.GamePlayScreen.Random.Next(Config.ItemTypeAvaible.Count)];

            Dictionary<AnimationKey, Animation> animations = new Dictionary<AnimationKey, Animation>();
            Animation animation = new Animation(2, 32, 32, 0, Config.ItemTypeIndex[type] * 32, 5);

            Texture2D spriteTexture = gameRef.Content.Load<Texture2D>("Graphics/Characters/item");
            Sprite = new AnimatedSprite(spriteTexture, animation, position);
            Sprite.IsAnimating = true;

            Texture2D itemDestroyTexture = gameRef.Content.Load<Texture2D>("Graphics/Characters/itemDestroy");
            animation = new Animation(7, 31, 28, 0, 0, 8);
            itemDestroyAnimation = new AnimatedSprite(itemDestroyTexture, animation, this.Sprite.Position);
            itemDestroyAnimation.IsAnimating = false;

            inDestruction = false;
            isAlive = true;
        }
        #endregion

        #region XNA Method Region
        public override void Update(GameTime gameTime)
        {
            Sprite.Update(gameTime);

            if (inDestruction)
            {
                itemDestroyAnimation.Update(gameTime);
                if (itemDestroyAnimation.Animation.CurrentFrame == itemDestroyAnimation.Animation.FrameCount - 1)
                    Remove();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Sprite.Draw(gameTime, gameRef.SpriteBatch);

            if (itemDestroyAnimation.IsAnimating)
                itemDestroyAnimation.Draw(gameTime, gameRef.SpriteBatch);
        }
        #endregion

        #region Method Region
        public void ApplyItem(Player p)
        {
            switch (type)
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
                    int randomBadEffect = gameRef.GamePlayScreen.Random.Next(Config.BadItemEffectList.Count);
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
            if (!this.itemDestroyAnimation.IsAnimating)
            {
                this.inDestruction = true;
                this.itemDestroyAnimation.IsAnimating = true;
            }
        }

        public override void Remove()
        {
            this.isAlive = false;
        }
        #endregion
    }
}

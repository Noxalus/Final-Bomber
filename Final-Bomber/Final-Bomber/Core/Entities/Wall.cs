using System.Collections.Generic;
using FBLibrary.Core;
using FBLibrary.Core.BaseEntities;
using Final_Bomber.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Final_Bomber.Core.Entities
{
    public class Wall : BaseWall
    {
        #region Field Region

        public AnimatedSprite Sprite { get; protected set; }

        #endregion

        #region Constructor Region

        public Wall(Point cellPosition)
            : base(cellPosition)
        {
            var animations = new Dictionary<AnimationKey, Animation>();

            var spriteTexture = FinalBomber.Instance.Content.Load<Texture2D>("Graphics/Characters/wall");
            var animation = new Animation(6, 32, 32, 0, 0, 20);

            Sprite = new AnimatedSprite(spriteTexture, animation);
        }

        #endregion

        #region XNA Method Region

        public void Update(GameTime gameTime)
        {
            Sprite.Update(gameTime);

            if (Sprite.Animation.CurrentFrame == Sprite.Animation.FrameCount - 1)
                Remove();

            if (InDestruction)
                Sprite.IsAnimating = true;
        }

        public void Draw(GameTime gameTime)
        {
            Sprite.Draw(gameTime, FinalBomber.Instance.SpriteBatch, Position);
        }

        #endregion

        #region Method Region

        public override void Destroy()
        {
            InDestruction = true;
        }

        public override void Remove()
        {
            IsAlive = false;
            InDestruction = false;
        }

        #endregion
    }
}
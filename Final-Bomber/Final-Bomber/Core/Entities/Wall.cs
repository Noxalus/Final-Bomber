using System.Collections.Generic;
using Final_Bomber.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Final_Bomber.Core.Entities
{
    public class Wall : StaticEntity
    {
        #region Field Region

        private bool _inDestruction;
        private bool _isAlive;
        public override sealed AnimatedSprite Sprite { get; protected set; }

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

        #endregion

        #region Constructor Region

        public Wall(Point position)
        {
            var animations = new Dictionary<AnimationKey, Animation>();

            var spriteTexture = FinalBomber.Instance.Content.Load<Texture2D>("Graphics/Characters/wall");
            var animation = new Animation(6, 32, 32, 0, 0, 20);

            Sprite = new AnimatedSprite(spriteTexture, animation, position);
            _inDestruction = false;
            _isAlive = true;
        }

        #endregion

        #region XNA Method Region

        public override void Update(GameTime gameTime)
        {
            Sprite.Update(gameTime);

            if (Sprite.Animation.CurrentFrame == Sprite.Animation.FrameCount - 1)
                Remove();

            if (_inDestruction)
                Sprite.IsAnimating = true;
        }

        public void Draw(GameTime gameTime)
        {
            Sprite.Draw(gameTime, FinalBomber.Instance.SpriteBatch);
        }

        #endregion

        #region Method Region

        public override void Destroy()
        {
            if (!_inDestruction)
                _inDestruction = true;
        }

        public override void Remove()
        {
            _isAlive = false;
            _inDestruction = false;
        }

        #endregion
    }
}
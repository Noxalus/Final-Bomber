using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using Final_Bomber.Sprites;
using Microsoft.Xna.Framework.Graphics;
using Final_Bomber.Controls;
using Microsoft.Xna.Framework.Input;

namespace Final_Bomber.Components
{
    public class Wall : MapItem
    {
        #region Field Region

        public override AnimatedSprite Sprite { get; protected set; }

        private FinalBomber gameRef;

        private bool inDestruction;
        private bool isAlive;

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

        #endregion

        #region Constructor Region
        public Wall(FinalBomber game, Vector2 position)
        {
            this.gameRef = game;

            Dictionary<AnimationKey, Animation> animations = new Dictionary<AnimationKey, Animation>();

            Texture2D spriteTexture = gameRef.Content.Load<Texture2D>("Graphics/Characters/wall");
            Animation animation = new Animation(6, 32, 32, 0, 0, 20);

            this.Sprite = new AnimatedSprite(spriteTexture, animation, position);
            this.inDestruction = false;
            this.isAlive = true;            
        }
        #endregion

        #region XNA Method Region
        public override void Update(GameTime gameTime)
        {
            Sprite.Update(gameTime);

            if (Sprite.Animation.CurrentFrame == Sprite.Animation.FrameCount - 1)
                Remove();

            if (inDestruction)
                this.Sprite.IsAnimating = true;
        }

        public override void Draw(GameTime gameTime)
        {
            Sprite.Draw(gameTime, gameRef.SpriteBatch);
        }
        #endregion

        #region Method Region

        public override void Destroy()
        {
            if(!this.inDestruction)
                this.inDestruction = true;
        }

        public override void Remove()
        {
            this.isAlive = false;
            this.inDestruction = false;
        }

        #endregion
    }
}

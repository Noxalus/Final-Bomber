using System.Collections.Generic;
using System.Diagnostics;
using FBClient.WorldEngine;
using FBLibrary;
using FBLibrary.Core;
using FBLibrary.Core.BaseEntities;
using FBClient.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FBClient.Core.Entities
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
            var spriteTexture = FinalBomber.Instance.Content.Load<Texture2D>("Graphics/Sprites/wall");
            var animation = new Animation(6, 32, 32, 0, 0, 20) {FramesPerSecond = 20};

            Sprite = new AnimatedSprite(spriteTexture, animation);
        }

        #endregion

        #region XNA Method Region

        public void Update(GameTime gameTime)
        {
            Sprite.Update(gameTime);

            if (InDestruction && !Sprite.IsAnimating)
                Sprite.IsAnimating = true;

            base.Update();
        }

        public void Draw(GameTime gameTime, Camera2D camera)
        {
            if (camera.IsVisible(Position))
                Sprite.Draw(gameTime, FinalBomber.Instance.SpriteBatch, Position);
        }

        #endregion

        #region Method Region

        public override void Destroy()
        {
            base.Destroy();
        }

        public override void Remove()
        {
            base.Remove();
        }

        #endregion
    }
}
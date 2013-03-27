using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Final_Bomber.Sprites;

namespace Final_Bomber.Components
{
    public class UnbreakableWall : MapItem
    {
        #region Field Region
        public override sealed Sprites.AnimatedSprite Sprite { get; protected set; }
        readonly FinalBomber _gameRef;
        #endregion

        #region Constructor Region
        public UnbreakableWall(FinalBomber game, Vector2 position)
        {
            this._gameRef = game;
            var spriteTexture = _gameRef.Content.Load<Texture2D>("Graphics/Characters/edgeWall");
            var animation = new Animation(0, 32, 32, 0, 0, 0);
            Sprite = new Sprites.AnimatedSprite(spriteTexture, animation, position);
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

        #region Override Method Region
        public override void Destroy()
        {
        }

        public override  void Remove()
        {
        }
        #endregion
    }
}

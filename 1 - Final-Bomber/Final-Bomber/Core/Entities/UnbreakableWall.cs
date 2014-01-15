using FBLibrary.Core;
using FBLibrary.Core.BaseEntities;
using FBClient.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FBClient.Core.Entities
{
    public class UnbreakableWall : BaseUnbreakableWall
    {
        #region Field Region

        public BaseSprite Sprite { get; protected set; }

        #endregion

        #region Constructor Region

        public UnbreakableWall(Point cellPosition) : base(cellPosition)
        {
            var spriteTexture = FinalBomber.Instance.Content.Load<Texture2D>("Graphics/Sprites/edgeWall");
            Sprite = new BaseSprite(spriteTexture, new Rectangle(0, 0, 32, 32));
        }

        #endregion

        #region XNA Method Region

        public void Update(GameTime gameTime)
        {
            Sprite.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            Sprite.Draw(gameTime, FinalBomber.Instance.SpriteBatch, Position);
        }

        #endregion

        #region Override Method Region

        public override void Destroy()
        {
        }

        public override void Remove()
        {
        }

        #endregion
    }
}
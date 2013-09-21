using FBLibrary.Core;
using Final_Bomber.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Final_Bomber.Core.Entities
{
    public class EdgeWall : BaseEntity, IEntity
    {
        #region Field Region

        public BaseSprite Sprite { get; protected set; }

        #endregion

        #region Constructor Region

        public EdgeWall(Point cellPosition) : base(cellPosition)
        {
            var spriteTexture = FinalBomber.Instance.Content.Load<Texture2D>("Graphics/Characters/edgeWall");
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
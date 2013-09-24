using Microsoft.Xna.Framework;

namespace FBLibrary.Core.BaseEntities
{
    public abstract class BaseWall : StaticEntity
    {
        protected BaseWall(Point cellPosition)
            : base(cellPosition)
        {
        }

        public void Update()
        {
        }
    }
}

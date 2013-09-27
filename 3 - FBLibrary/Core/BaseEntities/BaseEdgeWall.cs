using Microsoft.Xna.Framework;

namespace FBLibrary.Core.BaseEntities
{
    public abstract class BaseEdgeWall : StaticEntity
    {
        protected BaseEdgeWall(Point cellPosition)
            : base(cellPosition)
        {
        }
    }
}

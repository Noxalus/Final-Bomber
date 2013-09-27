using Microsoft.Xna.Framework;

namespace FBLibrary.Core.BaseEntities
{
    public abstract class BaseArrow : StaticEntity
    {
        protected LookDirection Direction;

        protected BaseArrow(Point cellPosition, LookDirection lookDirection)
            : base(cellPosition)
        {
            Direction = lookDirection;
        }
    }
}

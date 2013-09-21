using Microsoft.Xna.Framework;

namespace FBLibrary.Core
{
    public abstract class DynamicEntity : BaseEntity
    {
        protected Point PreviousCellPosition;

        protected DynamicEntity()
        {
            PreviousCellPosition = Point.Zero;
        }

        protected virtual void Update()
        {
            PreviousCellPosition = CellPosition;
        }

        protected bool IsChangingCell()
        {
            return (CellPosition != PreviousCellPosition);
        }
    }
}

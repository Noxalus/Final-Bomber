
using Microsoft.Xna.Framework;

namespace FBLibrary.Core.BaseEntities
{
    public abstract class BaseBomb : DynamicEntity
    {
        protected BaseBomb()
        {

        }

        protected BaseBomb(Point cellPosition)
            : base(cellPosition)
        {

        }
    }
}

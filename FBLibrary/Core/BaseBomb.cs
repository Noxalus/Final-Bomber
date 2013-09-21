
using Microsoft.Xna.Framework;

namespace FBLibrary.Core
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

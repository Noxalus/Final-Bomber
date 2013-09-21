using Microsoft.Xna.Framework;

namespace FBLibrary.Core.BaseEntities
{
    public abstract class BaseTeleporter : StaticEntity
    {
        protected BaseTeleporter(Point cellPosition)
            : base(cellPosition)
        {

        }
    }
}

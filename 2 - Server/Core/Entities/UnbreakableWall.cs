using FBLibrary.Core.BaseEntities;
using Microsoft.Xna.Framework;

namespace FBServer.Core.Entities
{
    class UnbreakableWall : BaseUnbreakableWall
    {
        public UnbreakableWall(Point cellPosition) : base(cellPosition)
        {
            
        }

        public override void Destroy()
        {
            throw new System.NotImplementedException();
        }

        public override void Remove()
        {
            throw new System.NotImplementedException();
        }
    }
}

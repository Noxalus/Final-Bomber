using FBLibrary.Core;
using FBLibrary.Core.BaseEntities;
using Microsoft.Xna.Framework;

namespace Final_BomberServer.Core.Entities
{
    class Arrow : BaseArrow
    {
        public Arrow(Point cellPosition, LookDirection lookDirection)
            : base(cellPosition, lookDirection)
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

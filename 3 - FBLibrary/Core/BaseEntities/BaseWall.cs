using System;
using Microsoft.Xna.Framework;

namespace FBLibrary.Core.BaseEntities
{
    public abstract class BaseWall : StaticEntity
    {
        protected BaseWall(Point cellPosition)
            : base(cellPosition)
        {
            DestructionTime = GameConfiguration.WallDestructionTime;
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Destroy()
        {
            InDestruction = true;
        }

        public override void Remove()
        {
            IsAlive = false;
            InDestruction = false;
        }
    }
}

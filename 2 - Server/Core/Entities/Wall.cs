using System;
using FBLibrary.Core.BaseEntities;
using Microsoft.Xna.Framework;

namespace FBServer.Core.Entities
{
    public class Wall : BaseWall
    {
        public Wall(Point cellPosition)
            : base(cellPosition)
        {
        }

        public override void Destroy()
        {
            Remove();
        }

        public override void Remove()
        {
            IsAlive = false;
        }
    }
}

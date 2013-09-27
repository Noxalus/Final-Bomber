using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FBLibrary.Core
{
    public abstract class StaticEntity : BaseEntity
    {
        protected StaticEntity()
        {
        }

        protected StaticEntity(Point cellPosition)
            : base(cellPosition)
        {
            
        }

        public override void Update()
        {
            base.Update();
        }
    }
}

using FBLibrary;
using FBLibrary.Core.BaseEntities;
using Microsoft.Xna.Framework;

namespace Final_BomberServer.Core.Entities
{
    public class PowerUp : BasePowerUp
    {
        public PowerUp(Point position) : base(position)
        {
        }

        public override void Destroy()
        {
            InDestruction = true;
        }

        public override void Remove()
        {
            IsAlive = false;
        }
    }
}

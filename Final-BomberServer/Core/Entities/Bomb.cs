using System;
using FBLibrary.Core;
using FBLibrary.Core.BaseEntities;
using Final_BomberServer.Host;
using Microsoft.Xna.Framework;

namespace Final_BomberServer.Core.Entities
{
    public class Bomb : BaseBomb
    {
        public Bomb(int playerId, Point cellPosition, int power, TimeSpan timer, float speed)
            : base(playerId, cellPosition, power, timer, speed)
        {
        }

        public override void Update()
        {

            base.Update();
        }

        public override void Destroy()
        {
            //GameSettings.gameServer.SendBombExploded(this);
            ComputeActionField(3);

            Remove();
        }

        public override void Remove()
        {
            IsAlive = false;
        }
    }
}

using System;
using FBLibrary.Core.BaseEntities;
using Microsoft.Xna.Framework;

namespace FBServer.Core.Entities
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
            base.Destroy();
        }

        public override void Remove()
        {
            base.Remove();
        }
    }
}

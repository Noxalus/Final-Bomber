using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FBLibrary.Core;
using FBLibrary.Core.BaseEntities;
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
            #region Timer

            if (Timer >= TimerLenght)
            {
                Timer = TimeSpan.FromSeconds(-1);
                Destroy();
            }
            else if (Timer >= TimeSpan.Zero)
            {
                Timer += new TimeSpan(0, 0, 0, 0, GameSettings.Speed);

                // The bomb will explode soon
                if (CurrentDirection == LookDirection.Idle &&
                    !WillExplode && TimerLenght.TotalSeconds - Timer.TotalSeconds < 1)
                {
                    ComputeActionField(2);
                    WillExplode = true;
                }
            }

            #endregion

            base.Update();
        }

        public override void Destroy()
        {
            throw new NotImplementedException();
        }

        public override void Remove()
        {
            throw new NotImplementedException();
        }
    }
}

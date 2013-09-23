using System;
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
                Timer += TimeSpan.FromMilliseconds(GameSettings.Speed);

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
            GameSettings.gameServer.SendBombExploded(this);
            ComputeActionField(3);

            Remove();
        }

        public override void Remove()
        {
            IsAlive = false;
        }
    }
}

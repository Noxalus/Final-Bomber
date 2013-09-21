using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final_BomberServer.Core
{
    public class Bomb
    {
        #region Exploded
        public delegate void IsExplodedEventHandler(Bomb sender);
        public event IsExplodedEventHandler IsExploded;
        protected virtual void OnIsExploded()
        {
            if (IsExploded != null)
                IsExploded(this);
        }
        #endregion
        public MapTile Position;
        public bool Exploded = false;
        public Player player;
        public ExplosionCollection Explosion = new ExplosionCollection();
        public bool Remove = false; //När bomben har gjort det den kan och ska tas bort
        public bool IsSuddenDeath = false;

        float tickCount = 5;

        public Bomb(MapTile pos, Player player)
        {
            Position = pos;
            this.player = player;
        }

        public void CheckTick() //Kollar när den sprängs resp när den tas bort (den brinner 1 sek efter den har sprängs ocksp)
        {
            if (!Exploded)
            {
                tickCount -= GetTickSpeed();
                if (tickCount <= 0)
                {
                    Exploded = true;
                    OnIsExploded();
                    //ExplosionTmr.Start();
                }
            }
            else
            {
                if (!IsSuddenDeath)
                {
                    if (true /*ExplosionTmr.Each(1000)*/)
                    {
                        Remove = true;
                    }
                }
            }
        }

        public void Explode()
        {
            Exploded = true;
            OnIsExploded();
            //ExplosionTmr.Start();
        }

        private float GetTickSpeed()
        {
            float rtn = 0f;//((float)player.BombTickPerSek * (float)GameSettings.speed) / 1000;
            return rtn;
        }
    }

    public class Explosion
    {
        public enum ExplosionType
        {
            Up,
            Right,
            Down,
            Left,
            UpEnd,
            RightEnd,
            DownEnd,
            LeftEnd,
            Mid
        }

        public Explosion(ExplosionType explosionType, MapTile position)
        {
            this.explosionType = explosionType;
            this.Position = position;
        }
        public ExplosionType explosionType;
        public MapTile Position;
    }


    public class ExplosionCollection : List<Explosion>
    {
        public Explosion GetExplosionByTile(MapTile tile)
        {
            foreach (Explosion ex in this)
            {
                if (ex.Position == tile)
                    return ex;
            }
            return null;
        }
    }
}

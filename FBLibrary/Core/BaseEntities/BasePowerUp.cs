using Microsoft.Xna.Framework;

namespace FBLibrary.Core.BaseEntities
{
    public abstract class BasePowerUp : StaticEntity
    {
        public PowerUpType Type;

        protected BasePowerUp(Point position) : base(position)
        {
        }

        public void ApplyItem(BasePlayer p)
        {
            switch (Type)
            {
                // More power
                case PowerUpType.Power:
                    p.IncreasePower(1);
                    break;
                // More bombs
                case PowerUpType.Bomb:
                    p.IncreaseTotalBombNumber(1);
                    break;
                // More speed
                case PowerUpType.Speed:
                    p.IncreaseSpeed(GameConfiguration.PlayerSpeedIncrementeur);
                    break;
                // Skeleton ! => Bad items
                case PowerUpType.BadItem:
                    //int randomBadEffect = GamePlayScreen.Random.Next(GameConfiguration.BadEffectList.Count);
                    int randomBadEffect = 0;
                    p.ApplyBadItem(GameConfiguration.BadEffectList[randomBadEffect]);
                    break;
                // More points
                case PowerUpType.Score:
                    p.Stats.PointPicked++;
                    break;
            }
        }
    }
}

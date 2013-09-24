using Microsoft.Xna.Framework;

namespace FBLibrary.Core.BaseEntities
{
    public abstract class BasePowerUp : StaticEntity
    {
        public PowerUpType Type;

        protected BasePowerUp(Point position) : base(position)
        {
            Type = GameConfiguration.PowerUpTypeAvailable[
                GameConfiguration.Random.Next(
                GameConfiguration.PowerUpTypeAvailable.Count)];
        }

        public void ApplyEffect(BasePlayer p)
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
                    p.IncreaseSpeed(GameConfiguration.BasePlayerSpeed * (GameConfiguration.PlayerSpeedIncrementeurPercentage/100));
                    break;
                // Skeleton ! => Bad items
                case PowerUpType.BadEffect:
                    int randomBadEffect = GameConfiguration.Random.Next(GameConfiguration.BadEffectList.Count);
                    p.ApplyBadItem(GameConfiguration.BadEffectList[randomBadEffect]);
                    break;
                // More points
                case PowerUpType.Score:
                    p.Stats.Score++;
                    break;
            }
        }
    }
}

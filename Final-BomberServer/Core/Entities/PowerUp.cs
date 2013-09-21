using FBLibrary.Core;
using FBLibrary.Core.BaseEntities;

namespace Final_BomberServer.Core.Entities
{
    public class PowerUp : BasePowerUp
    {
        public PowerUp(PowerUpType type)
        {
            Type = type;
        }
        public PowerUpType Type;

        public void GetPowerup(Player player)
        {
            player.Stats.PowerupsPicked++;
            switch (Type)
            {
                /*
                case PowerupType.ExtraLife:
                    player.lifes += (int)GetPowerupValue();
                    break;
                */
                case PowerUpType.Speed:
                    player.Speed += GetPowerupValue();
                    break;
                /*
                case PowerupType.TickRate:
                    player.BombTickPerSek += GetPowerupValue();
                    break;
                */
                case PowerUpType.Bomb:
                    player.TotalBombAmount += (int)GetPowerupValue();
                    break;
                case PowerUpType.Power:
                    player.BombPower += (int)GetPowerupValue();
                    break;
            }
        }

        public float GetPowerupValue()
        {
            switch (Type)
            {
                /*
                case PowerUpType.ExtraLife:
                    return GameSettings.GameValues.Powerup.Lifes;
                case PowerUpType.TickRate:
                    return GameSettings.GameValues.Powerup.Tickrate;
                */
                case PowerUpType.Speed:
                    return GameSettings.GameValues.Powerup.MovementSpeed;
                case PowerUpType.Bomb:
                    return GameSettings.GameValues.Powerup.BombAmount;
                case PowerUpType.Power:
                    return GameSettings.GameValues.Powerup.BombRange;
            }

            return 0;
        }

        public override void Destroy()
        {
            throw new System.NotImplementedException();
        }

        public override void Remove()
        {
            throw new System.NotImplementedException();
        }
    }
}

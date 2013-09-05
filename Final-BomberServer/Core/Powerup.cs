using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final_BomberServer.Core
{
    public class Powerup
    {
        public Powerup(PowerupType type)
        {
            Type = type;
        }
        public PowerupType Type;

        public enum PowerupType
        {
            ExplosionRange,
            ExtraLife,
            MaxBombs,
            MoveSpeed,
            TickRate,
        }

        public void GetPowerup(Player player)
        {
            player.Stats.PowerupsPicked++;
            switch (Type)
            {
                case PowerupType.ExtraLife:
                    player.lifes += (int)GetPowerupValue();
                    break;
                case PowerupType.MoveSpeed:
                    player.MoveSpeed += GetPowerupValue();
                    break;
                case PowerupType.TickRate:
                    player.BombTickPerSek += GetPowerupValue();
                    break;
                case PowerupType.MaxBombs:
                    player.maxBombAmount += (int)GetPowerupValue();
                    break;
                case PowerupType.ExplosionRange:
                    player.ExplodeRange += (int)GetPowerupValue();
                    break;
            }
        }

        public float GetPowerupValue()
        {
            switch (Type)
            {
                case PowerupType.ExtraLife:
                    return GameSettings.GameValues.Powerup.Lifes;
                case PowerupType.MoveSpeed:
                    return GameSettings.GameValues.Powerup.MovementSpeed;
                case PowerupType.TickRate:
                    return GameSettings.GameValues.Powerup.Tickrate;
                case PowerupType.MaxBombs:
                    return GameSettings.GameValues.Powerup.BombAmount;
                case PowerupType.ExplosionRange:
                    return GameSettings.GameValues.Powerup.BombRange;
            }
            return 0;
        }
    }
}

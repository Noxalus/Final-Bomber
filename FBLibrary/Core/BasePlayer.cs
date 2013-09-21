using System;
using Microsoft.Xna.Framework;

namespace FBLibrary.Core
{
    public abstract class BasePlayer
    {
        protected int Id;
        protected bool IsAlive;
        protected LookDirection CurrentDirection;
        protected TimeSpan InvincibleTime;
        protected int CurrentBombAmount;
        protected int TotalBombAmount;
        protected int BombPower;

        // TODO ?
        protected Vector2 Position;
        protected float Speed;

        protected BasePlayer()
        {
            Speed = GameConfiguration.PlayerBaseSpeed;
            // etc...
        }
    }
}

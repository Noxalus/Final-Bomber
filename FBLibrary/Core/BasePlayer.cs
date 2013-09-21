using System;
using Microsoft.Xna.Framework;

namespace FBLibrary.Core
{
    public abstract class BasePlayer : DynamicEntity
    {
        public int Id;
        public string Name;
        public bool IsAlive;
        public bool OnEdge;
        public LookDirection CurrentDirection;
        public TimeSpan InvincibleTime;
        public int CurrentBombAmount;
        public int TotalBombAmount;
        public int BombPower;
        public TimeSpan BombTimer;


        protected BasePlayer(int id)
        {
            Id = id;
            Name = "[UNKNOWN]";
            IsAlive = true;
            OnEdge = false;
            CurrentDirection = LookDirection.Idle;
            InvincibleTime = GameConfiguration.PlayerInvincibleTimer;
            TotalBombAmount = GameConfiguration.BasePlayerBombAmount;
            CurrentBombAmount = TotalBombAmount;
            BombPower = GameConfiguration.BasePlayerBombPower;
            BombTimer = GameConfiguration.BaseBombTimer;
            // etc...
        }
    }
}

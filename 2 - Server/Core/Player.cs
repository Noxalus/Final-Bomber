using FBLibrary.Core;
using FBLibrary.Core.BaseEntities;
using FBServer.Host;
using Microsoft.Xna.Framework;
using System;

namespace FBServer.Core
{
    public class Player : BasePlayer
    {
        private TimeSpan _destructionTimer;

        private readonly Timer _tmrSendPos = new Timer(true);

        public Player(int id)
            : base(id)
        {
            _destructionTimer = DestructionTime;
        }

        public Player(int id, PlayerStats stats)
            : base(id)
        {
            _destructionTimer = DestructionTime;
            Stats = stats;
        }

        public void SetMovement(LookDirection direction)
        {
            CurrentDirection = direction;
            if (PreviousDirection == LookDirection.Idle && direction != LookDirection.Idle)
            {
                PreviousDirection = direction;
                SendPosition();
            }
        }


        public void MovePlayer(BaseMap map)
        {
            if (IsAlive)
            {
                IsMoving = true;
                switch (CurrentDirection)
                {
                    case LookDirection.Down:
                        Position += new Vector2(0, GetMovementSpeed());
                        break;
                    case LookDirection.Up:
                        Position += new Vector2(0, -GetMovementSpeed());
                        break;
                    case LookDirection.Left:
                        Position += new Vector2(-GetMovementSpeed(), 0);
                        break;
                    case LookDirection.Right:
                        Position += new Vector2(GetMovementSpeed(), 0);
                        break;
                    default:
                        IsMoving = false;
                        break;
                }

                // If the player is moving
                if (IsMoving)
                {
                    ComputeWallCollision(map);
                }

                if (CurrentDirection != PreviousDirection)
                {
                    SendPosition();
                }
                else
                {
                    if (_tmrSendPos.Each(ServerSettings.SendPlayerPositionTime))
                        SendPosition();
                }
                
                if (InDestruction)
                {
                    _destructionTimer -= TimeSpan.FromMilliseconds(GameSettings.Speed);

                    if (_destructionTimer <= TimeSpan.Zero)
                    {
                        Remove();
                    }
                }

                // Call Update method of DynamicEntity class
                base.Update();

                PreviousDirection = CurrentDirection;
            }
        }

        private void SendPosition()
        {
            GameServer.Instance.SendPlayerPosition(this, false);
        }

        protected override float GetMovementSpeed()
        {
            float rtn = (Speed * GameSettings.Speed) / 1000f;
            return rtn;
        }

        protected override int GetTime()
        {
            return GameSettings.Speed;
        }

        public override void Destroy()
        {
            InDestruction = true;
        }

        public override void Remove()
        {
            base.Remove();
        }
    }
}

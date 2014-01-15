using FBLibrary;
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
                // TODO: Send position to all players except him
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
                    // TODO: Same
                    SendPosition();
                }

                // Call Update method of DynamicEntity class
                base.Update();

                PreviousDirection = CurrentDirection;
            }
        }

        private void SendPosition()
        {
            var client = GameServer.Instance.Clients.GetClientFromPlayer(this);
            GameServer.Instance.SendPlayerPosition(client, false, true);
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

using System;
using System.Collections.Generic;
using FBLibrary;
using FBLibrary.Core;
using FBLibrary.Core.BaseEntities;
using FBServer.Host;
using Lidgren.Network;
using Microsoft.Xna.Framework;

namespace FBServer.Core
{
    public class Player : BasePlayer
    {
        private readonly List<Vector2> _previousPositions;
        private readonly Timer _storePositionTimer;
        private TimeSpan _timer;

        public Player(int id)
            : base(id)
        {
            _previousPositions = new List<Vector2>();
            _storePositionTimer = new Timer(true);
            _timer = TimeSpan.Zero;
        }

        public Player(int id, PlayerStats stats)
            : base(id)
        {
            _previousPositions = new List<Vector2>();
            _storePositionTimer = new Timer(true);
            _timer = TimeSpan.Zero;

            Stats = stats;
        }

        public override void Update()
        {
            base.Update();

            _timer -= TimeSpan.FromTicks(GameConfiguration.DeltaTime);

            // If the player is moving
            if (IsMoving)
            {
                // Add current position to previous positions list
                if (_timer <= TimeSpan.Zero)
                {
                    _timer = TimeSpan.FromMilliseconds(ServerSettings.StorePreviousPlayerPositionTime);

                    //Program.Log.Debug("COUCOU [" + Position + "][" + _storePositionTimer.ElapsedMilliseconds + "]");
                    if (_previousPositions.Count >= ServerSettings.MaxLatency / ServerSettings.StorePreviousPlayerPositionTime)
                    {
                        _previousPositions.RemoveAt(0);
                    }

                    _previousPositions.Add(Position);

                    //Program.Log.Debug("Previous position: " + Position);
                }
            }
        }

        public void SetLookDirection(LookDirection direction, double? ping = null)
        {
            CurrentDirection = direction;
            if (PreviousDirection == LookDirection.Idle && direction != LookDirection.Idle)
            {
                PreviousDirection = direction;

                // TODO: Send position to all players except him
                SendPosition();
            }
            // If ping is not null (player want to stay here)
            else if (ping != null)
            {
                // => we set position to a previous position according to message ping
                double messageLatency = Math.Abs((float)ping - NetTime.Now) * 1000;
                //Program.Log.Debug("Message latency: " + messagePing + "|NetTime.Now: " + NetTime.Now);

                var previousPositionIndex = (int)Math.Round(messageLatency / ServerSettings.StorePreviousPlayerPositionTime) - 1;
                if (previousPositionIndex >= _previousPositions.Count)
                    previousPositionIndex = _previousPositions.Count - 1;

                //Position = _previousPositions[(_previousPositions.Count - 1) - previousPositionIndex];

                _previousPositions.Clear();

                //RealMovePlayer((float)messageLatency);

                SendPosition(false);
            }
        }


        public void MovePlayer(BaseMap map, float? customDeltaTime = null)
        {
            if (IsAlive)
            {
                IsMoving = RealMovePlayer(customDeltaTime);


                // If the player is moving
                if (IsMoving)
                {
                    ComputeWallCollision(map);
                }

                if (CurrentDirection != PreviousDirection)
                {
                    SendPosition();
                }

                // Call Update method of DynamicEntity class
                base.Update();

                PreviousDirection = CurrentDirection;
            }
        }

        private bool RealMovePlayer(float? customDeltaTime = null)
        {
            switch (CurrentDirection)
            {
                case LookDirection.Down:
                    Position += new Vector2(0, GetMovementSpeed(customDeltaTime));
                    break;
                case LookDirection.Up:
                    Position += new Vector2(0, -GetMovementSpeed(customDeltaTime));
                    break;
                case LookDirection.Left:
                    Position += new Vector2(-GetMovementSpeed(customDeltaTime), 0);
                    break;
                case LookDirection.Right:
                    Position += new Vector2(GetMovementSpeed(customDeltaTime), 0);
                    break;
                default:
                    return false;
            }

            return true;
        }

        private void SendPosition(bool exceptMe = true)
        {
            var client = GameServer.Instance.Clients.GetClientFromPlayer(this);
            GameServer.Instance.SendPlayerPosition(client, false, exceptMe);
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

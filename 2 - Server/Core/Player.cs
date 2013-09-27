using System.Diagnostics;
using FBLibrary.Core;
using FBLibrary.Core.BaseEntities;
using Final_BomberServer.Core.Entities;
using Final_BomberServer.Core.WorldEngine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final_BomberServer.Core
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
                        //Program.Log.Info("[Client #" + this.PlayerId + "]Position: " + Position.ToString());
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

                //Program.Log.Info("[Client #" + Id + "]Position: + " + Position.ToString());

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
            GameSettings.GameServer.SendPlayerPosition(this, false);
        }

        /*

        public Vector2 GetCenterPosition()
        {
            return new Vector2(Position.X + SizeOffset.X + Dimension.X / 2f, Position.Y + SizeOffset.Y + Dimension.Y / 2f);
        }

        public void PositionOnTile(MapTile tile)
        {
            CurrentTile = tile;
            NextTile = tile;
            Vector2 pos = GetCenteredTilePos(tile);
            Position.X = pos.X;
            Position.Y = pos.Y;
        }

        public Vector2 GetCenteredTilePos(MapTile tile)
        {
            Vector2 rtn = new Vector2(0);
            Vector2 pos = Vector2.Zero; //tile.GetMapPos();
            pos += new Vector2(MapTile.SIZE / 2);
            rtn.X = pos.X - (SizeOffset.X + Dimension.X / 2f);
            rtn.Y = pos.Y - (SizeOffset.Y + Dimension.Y / 2f);
            return rtn;
        }

        public MapTile GetPlayerPosition()
        {
            Vector2 pos = GetCenterPosition();
            return GameSettings.GetCurrentMap().GetTileByPosition(pos.X, pos.Y);
        }

        public void Burn(OldBomb bomb)
        {
            if (true) //invurnable.Each(INVURNABLETIME))
            {
                if (bomb != null)
                {
                    if (bomb.player == this)
                    {
                        Stats.SelfExplodeHits++;
                    }
                    else
                    {
                        if (bomb.player != null)
                            bomb.player.Stats.ExplodeHits++;
                    }
                }
                Stats.Burns++;
                //invurnable.Reset();
                GameSettings.gameServer.SendPlayerGotBurned(this);
                Kill(bomb);
            }
        }

        public void Kill(OldBomb bomb)
        {
            if (IsAlive)
            {
                if (bomb.player == this)
                {
                    Stats.SelfKills++;
                }
                else
                {
                    if (bomb.player != null)
                        bomb.player.Stats.Kills++;
                }

                IsAlive = false;
                GameSettings.gameServer.SendRemovePlayer(this);
            }
        }
        */
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
            IsAlive = false;
        }
    }
}

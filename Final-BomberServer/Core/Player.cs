using System.Diagnostics;
using FBLibrary.Core;
using FBLibrary.Core.BaseEntities;
using Final_BomberServer.Core.Entities;
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
        public LookDirection nextDirection;
        public MapTile AfterNextTile;
        public MapTile NextTile;
        public MapTile CurrentTile;
        public Vector2 SizeOffset = new Vector2(15, 30);
        public PlayerStats Stats = new PlayerStats();

        private readonly Timer _tmrSendPos = new Timer(true);

        public Player(int id)
            : base(id)
        {
            nextDirection = LookDirection.Idle;
        }

        public void SetMovement(LookDirection direction)
        {
            MapTile nextTile = null;
            if (NextTile != null) // Got an error that this fixes
            {
                nextTile = GameSettings.GetCurrentMap().GetTileByPlayerDirection(this, direction);
            }

            if (true /*nextTile != null*/)
            {
                nextDirection = direction;
                if (CurrentDirection == LookDirection.Idle && direction != LookDirection.Idle)
                {
                    CurrentDirection = direction;
                    SendPosition();
                }
            }
            else
            {
                nextDirection = LookDirection.Idle;
            }
        }


        public void MovePlayer()
        {
            if (IsAlive)
            {
                switch (CurrentDirection)
                {
                    case LookDirection.Down:
                        Position += new Vector2(0, GetMovementSpeed());
                        if (true /*Position.Y > GetCenteredTilePos(NextTile).Y*/)
                        {
                            //Position.Y = GetCenteredTilePos(NextTile).Y;

                            if (CurrentDirection != nextDirection)
                            {
                                CurrentDirection = nextDirection;
                                SendPosition();
                            }
                            else
                            {
                                if (_tmrSendPos.Each(5000))
                                    SendPosition();
                            }

                            CurrentTile = NextTile;
                            SetMovement(CurrentDirection);
                        }
                        break;
                    case LookDirection.Up:
                        Position += new Vector2(0, -GetMovementSpeed());
                        if (true /*Position.Y < GetCenteredTilePos(NextTile).Y*/)
                        {
                            //Position.Y = GetCenteredTilePos(NextTile).Y;

                            if (CurrentDirection != nextDirection)
                            {
                                CurrentDirection = nextDirection;
                                SendPosition();
                            }
                            else
                            {
                                if (_tmrSendPos.Each(5000))
                                    SendPosition();
                            }

                            CurrentTile = NextTile;
                            SetMovement(CurrentDirection);
                        }
                        break;
                    case LookDirection.Left:
                        Position += new Vector2(-GetMovementSpeed(), 0);
                        if (true /*Position.X < GetCenteredTilePos(NextTile).X*/)
                        {
                            //Position.X = GetCenteredTilePos(NextTile).X;

                            if (CurrentDirection != nextDirection)
                            {
                                CurrentDirection = nextDirection;
                                SendPosition();
                            }
                            else
                            {
                                if (_tmrSendPos.Each(5000))
                                    SendPosition();
                            }

                            CurrentTile = NextTile;
                            SetMovement(CurrentDirection);
                        }
                        break;
                    case LookDirection.Right:
                        Position += new Vector2(GetMovementSpeed(), 0);
                        //Program.Log.Info("[Client #" + this.PlayerId + "]Position: " + Position.ToString());
                        if (true /*Position.X > GetCenteredTilePos(NextTile).X*/)
                        {
                            //Position.X = GetCenteredTilePos(NextTile).X;

                            if (CurrentDirection != nextDirection)
                            {
                                CurrentDirection = nextDirection;
                                SendPosition();
                            }
                            else
                            {
                                if (_tmrSendPos.Each(5000))
                                {
                                    SendPosition();
                                }
                            }

                            CurrentTile = NextTile;
                            SetMovement(CurrentDirection);
                        }
                        break;
                }

                //Program.Log.Info("[Client #" + this.PlayerId + "]Position: + " + Position.ToString());
            }
        }

        public void SendPosition()
        {
            GameSettings.gameServer.SendPlayerPosition(this, false);
        }

        public Vector2 GetCenterPosition()
        {
            return new Vector2(Position.X + SizeOffset.X + Dimension.X / 2f, Position.Y + SizeOffset.Y + Dimension.Y / 2f);
        }

        public void PositionOnTile(MapTile tile)
        {
            CurrentTile = tile;
            NextTile = tile;
            Vector2 pos = GetCenteredTilePos(tile);
            /*
            Position.X = pos.X;
            Position.Y = pos.Y;
            */
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

        public void Burn(Bomb bomb)
        {
            if (true /*invurnable.Each(INVURNABLETIME)*/)
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

        public void Kill(Bomb bomb)
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

        private float GetMovementSpeed()
        {
            float rtn = (Speed * GameSettings.speed) / 1000f;
            return rtn;
        }

        public override void Destroy()
        {
            throw new NotImplementedException();
        }

        public override void Remove()
        {
            throw new NotImplementedException();
        }
    }

    public class PlayerStats
    {
        public int Kills, ExplodeHits, Burns, SelfExplodeHits, SelfKills, TilesBlowned, PowerupsPicked, TileWalkDistance;
        //Fixad - PowerupsPicked, TileWalkDistance, Selfkills, kills, selfexplodehits, explodehits, death
    }
}

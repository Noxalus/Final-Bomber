using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final_BomberServer.Core
{
    public class Player
    {
        const int INVURNABLETIME = 5000;

        public ActionEnum Direction;
        public ActionEnum nextDirection;
        public Vector2 Position = new Vector2(0);
        public MapTile AfterNextTile;
        public MapTile NextTile;
        public MapTile CurrentTile;
        public int PlayerId;
        public Vector2 Size = new Vector2(25, 20);
        public Vector2 SizeOffset = new Vector2(15, 30);
        public PlayerStats Stats = new PlayerStats();

        public Timer tmrSendPos = new Timer(true);


        public int maxBombAmount;
        public int currentBombAmount;
        public float MoveSpeed;
        public int lifes;
        public bool IsDead;
        public int ExplodeRange;
        public float BombTickPerSek; //Bomb ticks per second

        public Player(int playerId)
        {
            this.PlayerId = playerId;
            maxBombAmount = 1;
            currentBombAmount = 0;
            MoveSpeed = 130;
            lifes = 2;
            IsDead = false;
            Direction = ActionEnum.Standing;
            nextDirection = ActionEnum.Standing;
            ExplodeRange = 2;
            BombTickPerSek = 1;
        }

        public enum ActionEnum
        {
            Standing,
            WalkingUp,
            WalkingRight,
            WalkingDown,
            WalkingLeft
        }

        public void SetMovement(ActionEnum direction)
        {
            MapTile nextTile = null;
            if (NextTile != null) // Got an error that this fixes
            {
                nextTile = GameSettings.GetCurrentMap().GetTileByPlayerDirection(this, direction);
            }

            if (true /*nextTile != null*/)
            {
                nextDirection = direction;
                if (Direction == ActionEnum.Standing && direction != ActionEnum.Standing)
                {
                    Direction = direction;
                    SendPosition();
                }
            }
            else
            {
                nextDirection = ActionEnum.Standing;
            }
        }


        public void MovePlayer()
        {
            if (!IsDead)
            {
                switch (Direction)
                {
                    case ActionEnum.WalkingDown:
                        Position += new Vector2(0, GetMovementSpeed());
                        if (Position.Y > GetCenteredTilePos(NextTile).Y)
                        {
                            Position.Y = GetCenteredTilePos(NextTile).Y;
                            if (Direction != nextDirection)
                            {
                                Direction = nextDirection;
                                SendPosition();
                            }
                            else
                            {
                                if (tmrSendPos.Each(200))
                                    SendPosition();
                            }
                            CurrentTile = NextTile;
                            SetMovement(Direction);
                        }
                        break;
                    case ActionEnum.WalkingUp:
                        Position += new Vector2(0, -GetMovementSpeed());
                        if (Position.Y < GetCenteredTilePos(NextTile).Y)
                        {
                            Position.Y = GetCenteredTilePos(NextTile).Y;
                            if (Direction != nextDirection)
                            {
                                Direction = nextDirection;
                                SendPosition();
                            }
                            else
                            {
                                if (tmrSendPos.Each(200))
                                    SendPosition();
                            }
                            CurrentTile = NextTile;
                            SetMovement(Direction);
                        }
                        break;
                    case ActionEnum.WalkingLeft:
                        Position += new Vector2(-GetMovementSpeed(), 0);
                        if (Position.X < GetCenteredTilePos(NextTile).X)
                        {
                            Position.X = GetCenteredTilePos(NextTile).X;
                            if (Direction != nextDirection)
                            {
                                Direction = nextDirection;
                                SendPosition();
                            }
                            else
                            {
                                if (tmrSendPos.Each(200))
                                    SendPosition();
                            }
                            CurrentTile = NextTile;
                            SetMovement(Direction);
                        }
                        break;
                    case ActionEnum.WalkingRight:
                        float test = GetMovementSpeed();
                        Position.X += 10f*GameSettings.speed;
                        Console.WriteLine("[Client #" + this.PlayerId + "]Position: " + Position.ToString());
                        if (true /*Position.X > GetCenteredTilePos(NextTile).X*/)
                        {
                            //Position.X = GetCenteredTilePos(NextTile).X;
                            if (Direction != nextDirection)
                            {
                                Direction = nextDirection;
                                SendPosition();
                            }
                            else
                            {
                                if (tmrSendPos.Each(200))
                                    SendPosition();
                            }
                            CurrentTile = NextTile;
                            SetMovement(Direction);
                        }
                        break;
                }

                //Console.WriteLine("[Client #" + this.PlayerId + "]Position: + " + Position.ToString());
            }
        }

        public void SendPosition()
        {
            GameSettings.gameServer.SendPlayerPosition(this, false);
        }

        public Vector2 GetCenterPosition()
        {
            return new Vector2(Position.X + SizeOffset.X + Size.X / 2, Position.Y + SizeOffset.Y + Size.Y / 2);
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
            rtn.X = pos.X - (SizeOffset.X + Size.X / 2);
            rtn.Y = pos.Y - (SizeOffset.Y + Size.Y / 2);
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
                lifes--;
                //invurnable.Reset();
                GameSettings.gameServer.SendPlayerGotBurned(this);
                if (lifes == 0)
                {
                    Kill(bomb);
                }
            }
        }

        public void Kill(Bomb bomb)
        {
            if (!IsDead)
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
                IsDead = true;
                GameSettings.gameServer.SendRemovePlayer(this);
            }
        }

        private float GetMovementSpeed()
        {
            float rtn = ((float)MoveSpeed * (float)GameSettings.speed) / 1000;
            return rtn;
        }
    }

    public class PlayerStats
    {
        public int Kills, ExplodeHits, Burns, SelfExplodeHits, SelfKills, TilesBlowned, PowerupsPicked, TileWalkDistance;
        //Fixad - PowerupsPicked, TileWalkDistance, Selfkills, kills, selfexplodehits, explodehits, death
    }
}

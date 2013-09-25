using System;
using System.Data;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace FBLibrary.Core.BaseEntities
{
    public abstract class BasePlayer : DynamicEntity
    {
        public int Id;
        public string Name;
        public bool OnEdge;
        public TimeSpan InvincibleTime;
        public int CurrentBombAmount;
        public int TotalBombAmount;
        public int BombPower;
        public TimeSpan BombTimer;
        public PlayerStats Stats;
        public bool IsInvincible;
        public bool HasBadEffect;
        public BadEffect BadEffect;
        public TimeSpan BadEffectTimer;
        public TimeSpan BadEffectTimerLenght;

        protected TimeSpan InvincibleTimer;

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
            Speed = GameConfiguration.BasePlayerSpeed;

            IsInvincible = true;

            Stats = new PlayerStats();

            // Bad power up
            HasBadEffect = false;
            BadEffectTimer = TimeSpan.Zero;
            BadEffectTimerLenght = TimeSpan.Zero;

            DestructionTime = GameConfiguration.PlayerDestructionTime;

            // Protected
            InvincibleTimer = GameConfiguration.PlayerInvincibleTimer;
        }

        public override void Update()
        {
            #region Item

            // Have caught a bad item
            if (HasBadEffect)
            {
                BadEffectTimer += TimeSpan.FromMilliseconds(GetTime());
                if (BadEffectTimer >= BadEffectTimerLenght)
                {
                    RemoveBadItem();
                }
            }

            #endregion

            #region Invincibility

            if (!GameConfiguration.Invincible && IsInvincible)
            {
                if (InvincibleTimer >= TimeSpan.Zero)
                {
                    InvincibleTimer -= TimeSpan.FromMilliseconds(GameConfiguration.DeltaTime);
                    Console.WriteLine(InvincibleTimer);
                }
                else
                {
                    InvincibleTimer = GameConfiguration.PlayerInvincibleTimer;
                    IsInvincible = false;
                }
            }

            #endregion

            base.Update();
        }

        #region Wall collision

        protected void ComputeWallCollision(BaseMap map)
        {
            #region Smooth movement
            // If the player want to go to top...
            if (CurrentDirection == LookDirection.Up)
            {
                // ...  and that there is a wall
                if (WallAt(new Point(CellPosition.X, CellPosition.Y - 1), map))
                {
                    // If he is more on the left side, we lag him to the left
                    if (IsMoreLeftSide() && !WallAt(new Point(CellPosition.X - 1, CellPosition.Y - 1), map))
                    {
                        if (!WallAt(new Point(CellPosition.X - 1, CellPosition.Y), map))
                            Position = new Vector2(Position.X - GetMovementSpeed(), Position.Y);
                    }
                    else if (IsMoreRightSide() &&
                             !WallAt(new Point(CellPosition.X + 1, CellPosition.Y - 1), map))
                    {
                        if (!WallAt(new Point(CellPosition.X + 1, CellPosition.Y), map))
                            Position = new Vector2(Position.X + GetMovementSpeed(), Position.Y);
                    }
                }
                // ... and that there is no wall
                else
                {
                    // If he is more on the left side
                    if (IsMoreLeftSide())
                    {
                        Position = new Vector2(Position.X + GetMovementSpeed(), Position.Y);
                    }
                    // If he is more on the right side
                    else if (IsMoreRightSide())
                    {
                        Position = new Vector2(Position.X - GetMovementSpeed(), Position.Y);
                    }
                }
            }
            // If the player want to go to bottom and that there is a wall
            else if (CurrentDirection == LookDirection.Down)
            {
                // Wall at the bottom ?
                if (WallAt(new Point(CellPosition.X, CellPosition.Y + 1), map))
                {
                    // If he is more on the left side, we lag him to the left
                    if (IsMoreLeftSide() && !WallAt(new Point(CellPosition.X - 1, CellPosition.Y + 1), map))
                    {
                        if (!WallAt(new Point(CellPosition.X - 1, CellPosition.Y), map))
                            Position = new Vector2(Position.X - GetMovementSpeed(), Position.Y);
                    }
                    else if (IsMoreRightSide() &&
                             !WallAt(new Point(CellPosition.X + 1, CellPosition.Y + 1), map))
                    {
                        if (!WallAt(new Point(CellPosition.X + 1, CellPosition.Y), map))
                            Position = new Vector2(Position.X + GetMovementSpeed(), Position.Y);
                    }
                }
                else
                {
                    // If he is more on the left side
                    if (IsMoreLeftSide())
                    {
                        Position = new Vector2(Position.X + GetMovementSpeed(), Position.Y);
                    }
                    // If he is more on the right side
                    else if (IsMoreRightSide())
                    {
                        Position = new Vector2(Position.X - GetMovementSpeed(), Position.Y);
                    }
                }
            }
            // If the player want to go to left and that there is a wall
            else if (CurrentDirection == LookDirection.Left)
            {
                if (WallAt(new Point(CellPosition.X - 1, CellPosition.Y), map))
                {
                    // If he is more on the top side, we lag him to the top
                    if (IsMoreTopSide() && !WallAt(new Point(CellPosition.X - 1, CellPosition.Y - 1), map))
                    {
                        if (!WallAt(new Point(CellPosition.X, CellPosition.Y - 1), map))
                            Position = new Vector2(Position.X, Position.Y - GetMovementSpeed());
                    }
                    else if (IsMoreBottomSide() && !WallAt(new Point(CellPosition.X - 1, CellPosition.Y + 1), map))
                    {
                        if (!WallAt(new Point(CellPosition.X, CellPosition.Y + 1), map))
                            Position = new Vector2(Position.X, Position.Y + GetMovementSpeed());
                    }
                }
                else
                {
                    // If he is more on the top side, we lag him to the bottom
                    if (IsMoreTopSide())
                    {
                        Position = new Vector2(Position.X, Position.Y + GetMovementSpeed());
                    }
                    else if (IsMoreBottomSide())
                    {
                        Position = new Vector2(Position.X, Position.Y - GetMovementSpeed());
                    }
                }
            }
            // If the player want to go to right and that there is a wall
            else if (CurrentDirection == LookDirection.Right)
            {
                if (WallAt(new Point(CellPosition.X + 1, CellPosition.Y), map))
                {
                    // If he is more on the top side, we lag him to the top
                    if (IsMoreTopSide() && !WallAt(new Point(CellPosition.X + 1, CellPosition.Y - 1), map))
                    {
                        if (!WallAt(new Point(CellPosition.X, CellPosition.Y - 1), map))
                            Position = new Vector2(Position.X, Position.Y - GetMovementSpeed());
                    }
                    else if (IsMoreBottomSide() &&
                             !WallAt(new Point(CellPosition.X + 1, CellPosition.Y + 1), map))
                    {
                        if (!WallAt(new Point(CellPosition.X, CellPosition.Y + 1), map))
                            Position = new Vector2(Position.X, Position.Y + GetMovementSpeed());
                    }
                }
                else
                {
                    // If he is more on the top side, we lag him to the top
                    if (IsMoreTopSide())
                    {
                        Position = new Vector2(Position.X, Position.Y + GetMovementSpeed());
                    }
                    else if (IsMoreBottomSide())
                    {
                        Position = new Vector2(Position.X, Position.Y - GetMovementSpeed());
                    }
                }
            }
            #endregion

            #region Wall collision
            // -- Vertical check -- //
            // Is there a wall on the top ?
            if (WallAt(new Point(CellPosition.X, CellPosition.Y - 1), map))
            {
                // Is there a wall on the bottom ?
                if (WallAt(new Point(CellPosition.X, CellPosition.Y + 1), map))
                {
                    // Top collision and Bottom collision
                    if ((CurrentDirection == LookDirection.Up && IsMoreTopSide()) ||
                        (CurrentDirection == LookDirection.Down && IsMoreBottomSide()))
                        PositionY = CellPosition.Y * Engine.TileHeight;
                }
                // No wall at the bottom
                else
                {
                    // Top collision
                    if (CurrentDirection == LookDirection.Up && IsMoreTopSide())
                        PositionY = CellPosition.Y * Engine.TileHeight;
                }
            }
            // Wall only at the bottom
            else if (WallAt(new Point(CellPosition.X, CellPosition.Y + 1), map))
            {
                // Bottom collision
                if (CurrentDirection == LookDirection.Down && IsMoreBottomSide())
                    PositionY = CellPosition.Y * Engine.TileHeight;
                // To lag him
                else if (CurrentDirection == LookDirection.Down)
                {
                    if (IsMoreLeftSide())
                        PositionX += GetMovementSpeed();
                    else if (IsMoreRightSide())
                        PositionX -= GetMovementSpeed();
                }
            }

            // -- Horizontal check -- //
            // Is there a wall on the left ?
            if (WallAt(new Point(CellPosition.X - 1, CellPosition.Y), map))
            {
                // Is there a wall on the right ?
                if (WallAt(new Point(CellPosition.X + 1, CellPosition.Y), map))
                {
                    // Left and right collisions
                    if ((CurrentDirection == LookDirection.Left && IsMoreLeftSide()) ||
                        (CurrentDirection == LookDirection.Right && IsMoreRightSide()))
                        PositionX = CellPosition.X * Engine.TileWidth - Engine.TileWidth / 2 +
                                           Engine.TileWidth / 2;
                }
                // Wall only at the left
                else
                {
                    // Left collision
                    if (CurrentDirection == LookDirection.Left && IsMoreLeftSide())
                        PositionX = CellPosition.X * Engine.TileWidth - Engine.TileWidth / 2 +
                                           Engine.TileWidth / 2;
                }
            }
            // Wall only at the right
            else if (WallAt(new Point(CellPosition.X + 1, CellPosition.Y), map))
            {
                // Right collision
                if (CurrentDirection == LookDirection.Right && IsMoreRightSide())
                    PositionX = CellPosition.X * Engine.TileWidth - Engine.TileWidth / 2 + Engine.TileWidth / 2;
            }
            #endregion

            // The player must stay in the map
            PositionX = MathHelper.Clamp(Position.X, Engine.TileWidth, (map.Size.X * Engine.TileWidth) - 2 * Engine.TileWidth);
            PositionY = MathHelper.Clamp(Position.Y, Engine.TileHeight, (map.Size.Y * Engine.TileHeight) - 2 * Engine.TileWidth);
        }

        private bool WallAt(Point cell, BaseMap map)
        {
            if (cell.X >= 0 && cell.Y >= 0 && cell.X < map.Size.X && cell.Y < map.Size.Y)
                return (map.CollisionLayer[cell.X, cell.Y]);
            else
                return false;
        }

        private bool IsMoreTopSide()
        {
            return Position.Y < ((CellPosition.Y * Engine.TileHeight) - (GetMovementSpeed() / 2));
        }

        private bool IsMoreBottomSide()
        {
            return Position.Y > ((CellPosition.Y * Engine.TileHeight) + (GetMovementSpeed() / 2));
        }

        private bool IsMoreLeftSide()
        {
            return Position.X < ((CellPosition.X * Engine.TileWidth) - (GetMovementSpeed() / 2));
        }

        private bool IsMoreRightSide()
        {
            return Position.X > ((CellPosition.X * Engine.TileWidth) + (GetMovementSpeed() / 2));
        }

        #endregion

        protected abstract float GetMovementSpeed();

        #region Public Method Region

        public void IncreaseTotalBombNumber(int incr)
        {
            if (TotalBombAmount + incr > GameConfiguration.MaxBombAmount)
            {
                TotalBombAmount = GameConfiguration.MaxBombAmount;
                CurrentBombAmount = TotalBombAmount;
            }
            else if (TotalBombAmount + incr < GameConfiguration.MinBombAmount)
            {
                TotalBombAmount = GameConfiguration.MinBombAmount;
                CurrentBombAmount = TotalBombAmount;
            }
            else
            {
                TotalBombAmount += incr;
                CurrentBombAmount += incr;
            }
        }

        public void IncreasePower(int incr)
        {
            if (BombPower + incr > GameConfiguration.MaxBombPower)
                BombPower = GameConfiguration.MaxBombPower;
            else if (BombPower + incr < GameConfiguration.MinBombPower)
                BombPower = GameConfiguration.MinBombPower;
            else
                BombPower += incr;
        }

        public void IncreaseSpeed(float incr)
        {
            Speed += incr;
        }

        public virtual void ApplyBadItem(BadEffect effect)
        {
            HasBadEffect = true;
            BadEffect = effect;
            BadEffectTimerLenght = TimeSpan.FromSeconds(15);
            //BadEffectTimerLenght = TimeSpan.FromSeconds(GamePlayScreen.Random.Next(Config.BadItemTimerMin, Config.BadItemTimerMax));
        }

        protected virtual void RemoveBadItem()
        {
            HasBadEffect = false;
            BadEffectTimer = TimeSpan.Zero;
            BadEffectTimerLenght = TimeSpan.Zero;
        }


        #endregion

        protected abstract int GetTime();

    }
}

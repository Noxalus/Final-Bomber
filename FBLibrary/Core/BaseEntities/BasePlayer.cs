using System;
using Microsoft.Xna.Framework;

namespace FBLibrary.Core.BaseEntities
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
            Speed = GameConfiguration.BasePlayerSpeed;
            // etc...

            // TO DELETE
            PositionY = Engine.TileWidth;
            PositionX = Engine.TileHeight;
        }

        protected void ComputeWallCollision(BaseMap map)
        {
            #region Wall collisions
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
                        PositionX += Speed;
                    else if (IsMoreRightSide())
                        PositionX -= Speed;
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

            // The player must stay in the map
            PositionX = MathHelper.Clamp(Position.X, Engine.TileWidth, (map.Size.X * Engine.TileWidth) - 2 * Engine.TileWidth);
            PositionY = MathHelper.Clamp(Position.Y, Engine.TileHeight, (map.Size.Y * Engine.TileHeight) - 2 * Engine.TileWidth);

            #endregion
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
            return Position.Y < ((CellPosition.Y * Engine.TileHeight) - (Speed / 2));
        }

        private bool IsMoreBottomSide()
        {
            return Position.Y > ((CellPosition.Y * Engine.TileHeight) + (Speed / 2));
        }

        private bool IsMoreLeftSide()
        {
            return Position.X < ((CellPosition.X * Engine.TileWidth) - (Speed / 2));
        }

        private bool IsMoreRightSide()
        {
            return Position.X > ((CellPosition.X * Engine.TileWidth) + (Speed / 2));
        }
    }
}

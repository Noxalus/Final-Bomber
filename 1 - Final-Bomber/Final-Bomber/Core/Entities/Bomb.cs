using System;
using System.Collections.Generic;
using System.Diagnostics;
using FBLibrary;
using FBLibrary.Core;
using FBLibrary.Core.BaseEntities;
using FBClient.Controls;
using FBClient.Entities;
using FBClient.Screens;
using FBClient.Screens.GameScreens;
using FBClient.Sprites;
using FBClient.WorldEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FBClient.Core.Entities
{
    public class Bomb : BaseBomb
    {
        #region Field Region

        private readonly Animation[] _explosionAnimations;
        private readonly Dictionary<Point, ExplosionDirection> _explosionAnimationsDirection;
        private readonly Texture2D _explosionSpriteTexture;

        public AnimatedSprite Sprite { get; protected set; }

        // TODO: Move to base bomb
        private bool _cellTeleporting;
        private int _lastPlayerThatPushIt;

        private enum ExplosionDirection
        {
            Down,
            Left,
            Right,
            Up,
            Middle,
            Horizontal,
            Vertical
        };

        #endregion
        
        #region Constructor Region

        public Bomb(int playerId, Point cellPosition, int pow, TimeSpan timerLenght, float playerSpeed)
            : base(playerId, cellPosition, pow, timerLenght, playerSpeed)
        {
            // TODO: Move all loading of content into a LoadContent XNA method like
            // Bomb Sprite
            var spriteTexture = FinalBomber.Instance.Content.Load<Texture2D>("Graphics/Characters/bomb");
            var animation = new Animation(3, 32, 32, 0, 0, 3);
            Sprite = new AnimatedSprite(spriteTexture, animation)
            {
                IsAnimating = true
            };

            // Bomb's explosion animations
            _explosionSpriteTexture = FinalBomber.Instance.Content.Load<Texture2D>("Graphics/Characters/explosion");
            var explosionAnimationsFramesPerSecond = 10;
            _explosionAnimations = new[]
            {
                new Animation(4, 32, 32, 0, 0, explosionAnimationsFramesPerSecond),
                new Animation(4, 32, 32, 0, 32, explosionAnimationsFramesPerSecond),
                new Animation(4, 32, 32, 0, 64, explosionAnimationsFramesPerSecond),
                new Animation(4, 32, 32, 0, 96, explosionAnimationsFramesPerSecond),
                new Animation(4, 32, 32, 0, 128, explosionAnimationsFramesPerSecond),
                new Animation(4, 32, 32, 0, 160, explosionAnimationsFramesPerSecond),
                new Animation(4, 32, 32, 0, 192, explosionAnimationsFramesPerSecond)
            };

            _explosionAnimationsDirection = new Dictionary<Point, ExplosionDirection>();

            // Bomb's states
            _cellTeleporting = false;
            _lastPlayerThatPushIt = -1;
        }

        #endregion

        #region XNA Method

        public void Update(GameTime gameTime, Map map, int[,] hazardMap)
        {
            Sprite.Update(gameTime);

            #region Is moving ?
            /*
            if (IsChangingCell())
            {
                if (Map.Board[PreviousCellPosition.X, PreviousCellPosition.Y] == this)
                {
                    Map.Board[PreviousCellPosition.X, PreviousCellPosition.Y] = null;
                }

                if (Map.Board[CellPosition.X, CellPosition.Y] == null)
                {
                    Map.CollisionLayer[CellPosition.X, CellPosition.Y] = true;
                    Map.Board[CellPosition.X, CellPosition.Y] = this;
                }

                Map.CollisionLayer[PreviousCellPosition.X, PreviousCellPosition.Y] = false;

                if (_cellTeleporting)
                    _cellTeleporting = false;

                Position = Engine.CellToVector(CellPosition);
            }
            */
            #endregion

            #region Destruction

            if (InDestruction)
            {
                foreach (Animation a in _explosionAnimations)
                    a.Update(gameTime);
            }

            Debug.Print(Timer.ToString());

            #endregion

            #region When the bomb moves
            /*
            // Control
            if (InputHandler.KeyDown(Keys.NumPad8))
                CurrentDirection = LookDirection.Up;
            else if (InputHandler.KeyDown(Keys.NumPad5))
                CurrentDirection = LookDirection.Down;
            else if (InputHandler.KeyDown(Keys.NumPad4))
                CurrentDirection = LookDirection.Left;
            else if (InputHandler.KeyDown(Keys.NumPad6))
                CurrentDirection = LookDirection.Right;

            if (CurrentDirection != LookDirection.Idle)
            {
                switch (CurrentDirection)
                {
                    case LookDirection.Up:
                        if (Position.Y > Engine.TileHeight)
                            PositionY = Position.Y - Speed;
                        else
                            CurrentDirection = LookDirection.Idle;

                        var upCell = new Point(CellPosition.X, CellPosition.Y - 1);
                        if (Map.Board[upCell.X, upCell.Y] is Player || WallAt(upCell))
                        {
                            // Top collision
                            if (CurrentDirection == LookDirection.Up && MoreTopSide())
                            {
                                PositionY = CellPosition.Y*Engine.TileHeight;
                                CurrentDirection = LookDirection.Idle;
                            }
                        }

                        break;
                    case LookDirection.Down:
                        if (Position.Y < (Map.Size.Y - 2)*Engine.TileHeight)
                            PositionY = Position.Y + Speed;
                        else
                            CurrentDirection = LookDirection.Idle;

                        var bottomCell = new Point(CellPosition.X, CellPosition.Y + 1);
                        if (Map.Board[bottomCell.X, bottomCell.Y] is Player || WallAt(bottomCell))
                        {
                            // Bottom collision
                            if (CurrentDirection == LookDirection.Down && MoreBottomSide())
                            {
                                PositionY = CellPosition.Y*Engine.TileHeight;
                                CurrentDirection = LookDirection.Idle;
                            }
                        }

                        break;
                    case LookDirection.Left:
                        if (Position.X > Engine.TileWidth)
                            PositionX = Position.X - Speed;
                        else
                            CurrentDirection = LookDirection.Idle;

                        var leftCell = new Point(CellPosition.X - 1, CellPosition.Y);
                        if (Map.Board[leftCell.X, leftCell.Y] is Player || WallAt(leftCell))
                        {
                            // Left collision
                            if (CurrentDirection == LookDirection.Left && MoreLeftSide())
                            {
                                PositionX = CellPosition.X*Engine.TileWidth - Engine.TileWidth/2 +
                                                   Engine.TileWidth/2;
                                CurrentDirection = LookDirection.Idle;
                            }
                        }
                        break;
                    case LookDirection.Right:
                        if (Position.X < (Map.Size.X - 2)*Engine.TileWidth)
                            PositionX = Position.X + Speed;
                        else
                            CurrentDirection = LookDirection.Idle;

                        var rightCell = new Point(CellPosition.X + 1, CellPosition.Y);
                        if (Map.Board[rightCell.X, rightCell.Y] is Player || WallAt(rightCell))
                        {
                            // Right collision
                            if (CurrentDirection == LookDirection.Right && MoreRightSide())
                            {
                                PositionX = CellPosition.X*Engine.TileWidth - Engine.TileWidth/2 +
                                                   Engine.TileWidth/2;
                                CurrentDirection = LookDirection.Idle;
                            }
                        }
                        break;
                }
                if (CurrentDirection == LookDirection.Idle)
                    Position = Engine.CellToVector(CellPosition);
            }
            */
            #endregion

            #region Teleporter
            /*
            if (!_cellTeleporting && Map.
                Board[CellPosition.X, CellPosition.Y] is Teleporter)
            {
                var teleporter = (Teleporter) (Map.
                    Board[CellPosition.X, CellPosition.Y]);

                teleporter.ChangeEntityPosition(this, Map);
                _cellTeleporting = true;
            }
            */
            #endregion

            #region Arrow
            /*
            if (!_cellTeleporting && Map.
                Board[CellPosition.X, CellPosition.Y] is Arrow)
            {
                var arrow = (Arrow) (Map.Board[CellPosition.X, CellPosition.Y]);

                arrow.ChangeDirection(this);
            }
            */
            #endregion

            // Call Update method of DynamicEntity class
            base.Update();
        }

        public void Draw(GameTime gameTime)
        {
            if (InDestruction)
            {
                if (_explosionAnimationsDirection.Count == 0)
                {
                    foreach (Point p in ActionField)
                    {
                        // Is this a wall ? => we don't like wall !
                        if (!Map.CollisionLayer[p.X, p.Y] || p == CellPosition)
                        {
                            // We choose the sprite of explosion for each cell
                            _explosionAnimationsDirection[p] = ComputeExplosionSpriteDirections(p, Map, HazardMap);
                        }
                    }
                }
                else
                {
                    foreach (Point p in _explosionAnimationsDirection.Keys)
                    {
                        FinalBomber.Instance.SpriteBatch.Draw(_explosionSpriteTexture,
                            new Vector2(Engine.Origin.X + p.X*Engine.TileWidth, Engine.Origin.Y + p.Y*Engine.TileHeight),
                            _explosionAnimations[(int) _explosionAnimationsDirection[p]].CurrentFrameRect, Color.White);
                    }
                }
            }
            else
                Sprite.Draw(gameTime, FinalBomber.Instance.SpriteBatch, Position);
        }

        #endregion

        #region Method Region

        #region Private Method Region

        #region Moving utils methods

        private bool WallAt(Point cell)
        {
            if (cell.X >= 0 && cell.Y >= 0 && cell.X < Map.Size.X &&
                cell.Y < Map.Size.Y)
                return (Map.CollisionLayer[cell.X, cell.Y]);
            return false;
        }

        private bool MoreTopSide()
        {
            return Position.Y < ((CellPosition.Y * Engine.TileHeight) - (Speed / 2));
        }

        private bool MoreBottomSide()
        {
            return Position.Y > ((CellPosition.Y * Engine.TileHeight) + (Speed / 2));
        }

        private bool MoreLeftSide()
        {
            return Position.X < ((CellPosition.X * Engine.TileWidth) - (Speed / 2));
        }

        private bool MoreRightSide()
        {
            return Position.X > ((CellPosition.X * Engine.TileWidth) + (Speed / 2));
        }

        #endregion

        // 0 => Down, 1 => Left, 2 => Right, 3 => Up, 4 => Middle, 5 => Horizontal, 6 => Vertical 
        private ExplosionDirection ComputeExplosionSpriteDirections(Point cell, BaseMap map, int[,] hazardMap)
        {
            int downCell = cell.Y + 1, leftCell = cell.X - 1, rightCell = cell.X + 1, upCell = cell.Y - 1;

            // ~ The middle ~ //
            if (cell.X == CellPosition.X && cell.Y == CellPosition.Y)
                return ExplosionDirection.Middle;

            // ~ Vertical axis ~ //

            // Top extremity
            if (hazardMap[cell.X, upCell] == 0 &&
                (ActionField.Find(c => c.X == cell.X && c.Y == downCell) != Point.Zero))
                return ExplosionDirection.Up;
                // Vertical
            if ((ActionField.Find(c => c.X == cell.X && c.Y == downCell) != Point.Zero) &&
                (ActionField.Find(c => c.X == cell.X && c.Y == upCell) != Point.Zero))
                return ExplosionDirection.Vertical;
                // Bottom extremity
            if (hazardMap[cell.X, downCell] == 0 &&
                (ActionField.Find(c => c.X == cell.X && c.Y == upCell) != Point.Zero))
                return ExplosionDirection.Down;

                // ~ Horizontal axis ~ //

                // Left extremity
            if (hazardMap[leftCell, cell.Y] == 0 &&
                (ActionField.Find(c => c.X == rightCell && c.Y == cell.Y) != Point.Zero))
                return ExplosionDirection.Left;
                // Left - Right
            if ((ActionField.Find(c => c.X == rightCell && c.Y == cell.Y) != Point.Zero) &&
                (ActionField.Find(c => c.X == leftCell && c.Y == cell.Y) != Point.Zero))
                return ExplosionDirection.Horizontal;
                // Right extremity
            if (hazardMap[rightCell, cell.Y] == 0 &&
                (ActionField.Find(c => c.X == leftCell && c.Y == cell.Y) != Point.Zero))
                return ExplosionDirection.Right;

                // ~ Corners ~ //

                // Corner Top - Left
            if (cell.Y == 1 && cell.X == 1)
            {
                // Left extremity
                if (ActionField.Find(c => c.X == rightCell && c.Y == cell.Y) != Point.Zero)
                    return ExplosionDirection.Left;
                    // Top extremity
                return ExplosionDirection.Up;
            }
                // Corner Bottom - Left
            if (cell.Y == map.Size.Y - 2 && cell.X == 1)
            {
                // Left extremity
                return ActionField.Find(c => c.X == rightCell && c.Y == cell.Y) != Point.Zero
                    ? ExplosionDirection.Left
                    : ExplosionDirection.Down;
            }

                // Corner Top - Right
            if (cell.X == map.Size.X - 2 && cell.Y == 1)
            {
                // Right extremity
                return ActionField.Find(c => c.X == leftCell && c.Y == cell.Y) != Point.Zero
                    ? ExplosionDirection.Right
                    : ExplosionDirection.Up;
            }
                // Corner Bottom - Right
            if (cell.Y == map.Size.Y - 2 && cell.X == map.Size.X - 2)
            {
                // Right extremity
                return ActionField.Find(c => c.X == leftCell && c.Y == cell.Y) != Point.Zero
                    ? ExplosionDirection.Right
                    : ExplosionDirection.Down;
            }
                // Error case
            return ExplosionDirection.Middle;
        }

        #endregion

        #region Public Method Region

        public void ChangeDirection(LookDirection lD, int playerId)
        {
            Point pos = Point.Zero;
            switch (lD)
            {
                case LookDirection.Up:
                    pos = new Point(CellPosition.X, CellPosition.Y - 1);
                    break;
                case LookDirection.Down:
                    pos = new Point(CellPosition.X, CellPosition.Y + 1);
                    break;
                case LookDirection.Left:
                    pos = new Point(CellPosition.X - 1, CellPosition.Y);
                    break;
                case LookDirection.Right:
                    pos = new Point(CellPosition.X + 1, CellPosition.Y);
                    break;
            }

            if (!Map.CollisionLayer[pos.X, pos.Y])
            {
                CurrentDirection = lD;
                _lastPlayerThatPushIt = playerId;
                foreach (Point p in ActionField)
                {
                    HazardMap[p.X, p.Y] = 0;
                }
            }
            else
                CurrentDirection = LookDirection.Idle;
        }

        public override void Destroy()
        {
            NetworkTestScreen.GameManager.BombExplosionSound.Play();

            base.Destroy();
        }

        public override void Remove()
        {
            IsAlive = false;
        }

        #endregion

        #endregion
    }
}
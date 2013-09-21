using System;
using System.Collections.Generic;
using FBLibrary.Core;
using Final_Bomber.Controls;
using Final_Bomber.Entities;
using Final_Bomber.Sprites;
using Final_Bomber.WorldEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Final_Bomber.Core.Entities
{
    public class Bomb : BaseBomb, IEntity
    {
        #region Field Region

        private readonly Animation[] _explosionAnimations;
        private readonly Dictionary<Point, ExplosionDirection> _explosionAnimationsDirection;
        private readonly SoundEffect _explosionSound;
        private readonly Texture2D _explosionSpriteTexture;

        private readonly int _power;

        private bool _cellTeleporting;
        private int[,] _hazardMap;
        private int _id;

        private int _lastPlayerThatPushIt;
        private LookDirection _lookDirection;

        // Private reference of _map
        private Map _map;
        private TimeSpan _timer;
        private TimeSpan _timerLenght;
        private bool _willExplode;
        public AnimatedSprite Sprite { get; protected set; }

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

        // Private reference of hazard _map

        #endregion

        #region Propery Region

        public bool IsAlive { get; private set; }
        public bool InDestruction { get; private set; }
        public List<Point> ActionField { get; private set; }

        #endregion

        #region Constructor Region

        public Bomb(int id, Point cellPosition, int pow, TimeSpan timerLenght, float playerSpeed)
            : base(cellPosition)
        {
            // ID of the player that planted the bomb
            _id = id;

            // Bomb Sprite
            var spriteTexture = FinalBomber.Instance.Content.Load<Texture2D>("Graphics/Characters/bomb");
            var animation = new Animation(3, 32, 32, 0, 0, 3);
            Sprite = new AnimatedSprite(spriteTexture, animation)
            {
                IsAnimating = true
            };

            // Bomb's explosion animations
            _explosionSpriteTexture = FinalBomber.Instance.Content.Load<Texture2D>("Graphics/Characters/explosion");
            const int explosionAnimationsFramesPerSecond = Config.BombLatency;
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

            // Sound Effect
            _explosionSound = FinalBomber.Instance.GamePlayScreen.BombExplosionSound;

            // Bomb's timer
            _timer = TimeSpan.Zero;
            _timerLenght = timerLenght;

            // Bomb's states
            InDestruction = false;
            IsAlive = true;
            _willExplode = false;
            _power = pow;
            _lookDirection = LookDirection.Idle;
            _cellTeleporting = false;

            _lastPlayerThatPushIt = -1;

            // Action field
            ActionField = new List<Point>();
        }

        #endregion

        private void Initialize(Map map, int[,] hazardMap)
        {
            _map = map;
            _hazardMap = hazardMap;

            ComputeActionField(1);
        }

        #region XNA Method

        public void Update(GameTime gameTime, Map map, int[,] hazardMap)
        {
            Sprite.Update(gameTime);

            #region Is moving ?

            if (IsChangingCell())
            {
                if (_map.Board[PreviousCellPosition.X, PreviousCellPosition.Y] == this)
                {
                    _map.Board[PreviousCellPosition.X, PreviousCellPosition.Y] = null;
                }

                if (_map.Board[CellPosition.X, CellPosition.Y] == null)
                {
                    _map.CollisionLayer[CellPosition.X, CellPosition.Y] = true;
                    _map.Board[CellPosition.X, CellPosition.Y] = this;
                }

                _map.CollisionLayer[PreviousCellPosition.X, PreviousCellPosition.Y] = false;

                if (_cellTeleporting)
                    _cellTeleporting = false;

                Position = Engine.CellToVector(CellPosition);
            }

            #endregion

            #region Timer

            if (_timer >= _timerLenght)
            {
                _timer = TimeSpan.FromSeconds(-1);
                Destroy();
            }
            else if (_timer >= TimeSpan.Zero)
            {
                _timer += gameTime.ElapsedGameTime;

                // The bomb will explode soon
                if (_lookDirection == LookDirection.Idle &&
                    !_willExplode && _timerLenght.TotalSeconds - _timer.TotalSeconds < 1)
                {
                    ComputeActionField(2);
                    _willExplode = true;
                }
            }

            #endregion

            #region Destruction

            if (InDestruction)
            {
                foreach (Animation a in _explosionAnimations)
                    a.Update(gameTime);
                if (_explosionAnimations[4].CurrentFrame == _explosionAnimations[4].FrameCount - 1)
                    Remove();
            }

            #endregion

            #region When the bomb moves

            // Control
            if (InputHandler.KeyDown(Keys.NumPad8))
                _lookDirection = LookDirection.Up;
            else if (InputHandler.KeyDown(Keys.NumPad5))
                _lookDirection = LookDirection.Down;
            else if (InputHandler.KeyDown(Keys.NumPad4))
                _lookDirection = LookDirection.Left;
            else if (InputHandler.KeyDown(Keys.NumPad6))
                _lookDirection = LookDirection.Right;

            if (_lookDirection != LookDirection.Idle)
            {
                switch (_lookDirection)
                {
                    case LookDirection.Up:
                        if (Position.Y > Engine.TileHeight)
                            PositionY = Position.Y - Speed;
                        else
                            _lookDirection = LookDirection.Idle;

                        var upCell = new Point(CellPosition.X, CellPosition.Y - 1);
                        if (_map.Board[upCell.X, upCell.Y] is Player || WallAt(upCell))
                        {
                            // Top collision
                            if (_lookDirection == LookDirection.Up && MoreTopSide())
                            {
                                PositionY = CellPosition.Y*Engine.TileHeight;
                                _lookDirection = LookDirection.Idle;
                            }
                        }

                        break;
                    case LookDirection.Down:
                        if (Position.Y < (_map.Size.Y - 2)*Engine.TileHeight)
                            PositionY = Position.Y + Speed;
                        else
                            _lookDirection = LookDirection.Idle;

                        var bottomCell = new Point(CellPosition.X, CellPosition.Y + 1);
                        if (_map.Board[bottomCell.X, bottomCell.Y] is Player || WallAt(bottomCell))
                        {
                            // Bottom collision
                            if (_lookDirection == LookDirection.Down && MoreBottomSide())
                            {
                                PositionY = CellPosition.Y*Engine.TileHeight;
                                _lookDirection = LookDirection.Idle;
                            }
                        }

                        break;
                    case LookDirection.Left:
                        if (Position.X > Engine.TileWidth)
                            PositionX = Position.X - Speed;
                        else
                            _lookDirection = LookDirection.Idle;

                        var leftCell = new Point(CellPosition.X - 1, CellPosition.Y);
                        if (_map.Board[leftCell.X, leftCell.Y] is Player || WallAt(leftCell))
                        {
                            // Left collision
                            if (_lookDirection == LookDirection.Left && MoreLeftSide())
                            {
                                PositionX = CellPosition.X*Engine.TileWidth - Engine.TileWidth/2 +
                                                   Engine.TileWidth/2;
                                _lookDirection = LookDirection.Idle;
                            }
                        }
                        break;
                    case LookDirection.Right:
                        if (Position.X < (_map.Size.X - 2)*Engine.TileWidth)
                            PositionX = Position.X + Speed;
                        else
                            _lookDirection = LookDirection.Idle;

                        var rightCell = new Point(CellPosition.X + 1, CellPosition.Y);
                        if (_map.Board[rightCell.X, rightCell.Y] is Player || WallAt(rightCell))
                        {
                            // Right collision
                            if (_lookDirection == LookDirection.Right && MoreRightSide())
                            {
                                PositionX = CellPosition.X*Engine.TileWidth - Engine.TileWidth/2 +
                                                   Engine.TileWidth/2;
                                _lookDirection = LookDirection.Idle;
                            }
                        }
                        break;
                }
                if (_lookDirection == LookDirection.Idle)
                    Position = Engine.CellToVector(CellPosition);
            }

            #endregion

            #region Teleporter

            if (!_cellTeleporting && _map.
                Board[CellPosition.X, CellPosition.Y] is Teleporter)
            {
                var teleporter = (Teleporter) (_map.
                    Board[CellPosition.X, CellPosition.Y]);

                teleporter.ChangeEntityPosition(this, _map);
                _cellTeleporting = true;
            }

            #endregion

            #region Arrow

            if (!_cellTeleporting && _map.
                Board[CellPosition.X, CellPosition.Y] is Arrow)
            {
                var arrow = (Arrow) (_map.
                    Board[CellPosition.X, CellPosition.Y]);

                arrow.ChangeDirection(this);
            }

            #endregion

            // Call Update method of DynamicEntity class
            Update();
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
                        if (!_map.CollisionLayer[p.X, p.Y] || p == CellPosition)
                        {
                            // We choose the sprite of explosion for each cell
                            _explosionAnimationsDirection[p] = ComputeExplosionSpriteDirections(p, _map, _hazardMap);
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

        private bool WallAt(Point cell)
        {
            if (cell.X >= 0 && cell.Y >= 0 && cell.X < _map.Size.X &&
                cell.Y < _map.Size.Y)
                return (_map.CollisionLayer[cell.X, cell.Y]);
            return false;
        }

        private bool MoreTopSide()
        {
            return Position.Y < ((CellPosition.Y*Engine.TileHeight) - (Speed/2));
        }

        private bool MoreBottomSide()
        {
            return Position.Y > ((CellPosition.Y*Engine.TileHeight) + (Speed/2));
        }

        private bool MoreLeftSide()
        {
            return Position.X < ((CellPosition.X*Engine.TileWidth) - (Speed/2));
        }

        private bool MoreRightSide()
        {
            return Position.X > ((CellPosition.X*Engine.TileWidth) + (Speed/2));
        }

        // Compute bomb's effect field: 1 => just planted, 2 => will explode soon, 3 => deathly
        private void ComputeActionField(int dangerType)
        {
            // Reset the actionField list
            // We put the bomb in its action field
            ActionField = new List<Point> {new Point(CellPosition.X, CellPosition.Y)};

            if (_hazardMap[CellPosition.X, CellPosition.Y] < dangerType)
            {
                _hazardMap[CellPosition.X, CellPosition.Y] = dangerType;
            }

            // 0 => Top, 1 => Bottom, 2 => Left, 3 => Right
            var obstacles = new Dictionary<String, bool>
            {
                {"up", false},
                {"down", false},
                {"right", false},
                {"left", false}
            };

            int tempPower = _power - 1;
            while (tempPower >= 0)
            {
                // Directions
                int up = CellPosition.Y - (_power - tempPower);
                int down = CellPosition.Y + (_power - tempPower);
                int right = CellPosition.X + (_power - tempPower);
                int left = CellPosition.X - (_power - tempPower);

                // Up
                Point addPosition;
                if (up >= 0 && !obstacles["up"])
                {
                    if (_map.CollisionLayer[CellPosition.X, up])
                        obstacles["up"] = true;
                    // We don't count the outside walls
                    if (!(_map.Board[CellPosition.X, up] is EdgeWall))
                    {
                        addPosition = new Point(CellPosition.X, up);
                        ActionField.Add(addPosition);
                        if (_hazardMap[addPosition.X, addPosition.Y] < dangerType)
                        {
                            _hazardMap[addPosition.X, addPosition.Y] = dangerType;
                        }
                    }
                }

                // Down
                if (down < _map.Size.Y - 1 && !obstacles["down"])
                {
                    if (_map.CollisionLayer[CellPosition.X, down])
                        obstacles["down"] = true;
                    // We don't count the outside walls
                    if (!(_map.Board[CellPosition.X, down] is EdgeWall))
                    {
                        addPosition = new Point(CellPosition.X, down);
                        ActionField.Add(addPosition);
                        if (_hazardMap[addPosition.X, addPosition.Y] < dangerType)
                        {
                            _hazardMap[addPosition.X, addPosition.Y] = dangerType;
                        }
                    }
                }

                // Right
                if (right < _map.Size.X - 1 && !obstacles["right"])
                {
                    if (_map.CollisionLayer[right, CellPosition.Y])
                        obstacles["right"] = true;
                    // We don't count the outside walls
                    if (!(_map.Board[right, CellPosition.Y] is EdgeWall))
                    {
                        addPosition = new Point(right, CellPosition.Y);
                        ActionField.Add(addPosition);
                        if (_hazardMap[addPosition.X, addPosition.Y] < dangerType)
                        {
                            _hazardMap[addPosition.X, addPosition.Y] = dangerType;
                        }
                    }
                }

                // Left
                if (left >= 0 && !obstacles["left"])
                {
                    if (_map.CollisionLayer[left, CellPosition.Y])
                        obstacles["left"] = true;
                    // We don't count the outside walls
                    if (!(_map.Board[left, CellPosition.Y] is EdgeWall))
                    {
                        addPosition = new Point(left, CellPosition.Y);
                        ActionField.Add(addPosition);
                        if (_hazardMap[addPosition.X, addPosition.Y] < dangerType)
                        {
                            _hazardMap[addPosition.X, addPosition.Y] = dangerType;
                        }
                    }
                }

                tempPower--;
            }
        }

        // 0 => Down, 1 => Left, 2 => Right, 3 => Up, 4 => Middle, 5 => Horizontal, 6 => Vertical 
        private ExplosionDirection ComputeExplosionSpriteDirections(Point cell, Map _map, int[,] _hazardMap)
        {
            int downCell = cell.Y + 1, leftCell = cell.X - 1, rightCell = cell.X + 1, upCell = cell.Y - 1;

            // ~ The middle ~ //
            if (cell.X == CellPosition.X && cell.Y == CellPosition.Y)
                return ExplosionDirection.Middle;

            // ~ Vertical axis ~ //

            // Top extremity
            if (_hazardMap[cell.X, upCell] == 0 &&
                (ActionField.Find(c => c.X == cell.X && c.Y == downCell) != Point.Zero))
                return ExplosionDirection.Up;
                // Vertical
            if ((ActionField.Find(c => c.X == cell.X && c.Y == downCell) != Point.Zero) &&
                (ActionField.Find(c => c.X == cell.X && c.Y == upCell) != Point.Zero))
                return ExplosionDirection.Vertical;
                // Bottom extremity
            if (_hazardMap[cell.X, downCell] == 0 &&
                (ActionField.Find(c => c.X == cell.X && c.Y == upCell) != Point.Zero))
                return ExplosionDirection.Down;

                // ~ Horizontal axis ~ //

                // Left extremity
            if (_hazardMap[leftCell, cell.Y] == 0 &&
                (ActionField.Find(c => c.X == rightCell && c.Y == cell.Y) != Point.Zero))
                return ExplosionDirection.Left;
                // Left - Right
            if ((ActionField.Find(c => c.X == rightCell && c.Y == cell.Y) != Point.Zero) &&
                (ActionField.Find(c => c.X == leftCell && c.Y == cell.Y) != Point.Zero))
                return ExplosionDirection.Horizontal;
                // Right extremity
            if (_hazardMap[rightCell, cell.Y] == 0 &&
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
            if (cell.Y == _map.Size.Y - 2 && cell.X == 1)
            {
                // Left extremity
                return ActionField.Find(c => c.X == rightCell && c.Y == cell.Y) != Point.Zero
                    ? ExplosionDirection.Left
                    : ExplosionDirection.Down;
            }

                // Corner Top - Right
            if (cell.X == _map.Size.X - 2 && cell.Y == 1)
            {
                // Right extremity
                return ActionField.Find(c => c.X == leftCell && c.Y == cell.Y) != Point.Zero
                    ? ExplosionDirection.Right
                    : ExplosionDirection.Up;
            }
                // Corner Bottom - Right
            if (cell.Y == _map.Size.Y - 2 && cell.X == _map.Size.X - 2)
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

            if (!_map.CollisionLayer[pos.X, pos.Y])
            {
                _lookDirection = lD;
                _lastPlayerThatPushIt = playerId;
                foreach (Point p in ActionField)
                {
                    _hazardMap[p.X, p.Y] = 0;
                }
            }
            else
                _lookDirection = LookDirection.Idle;
        }

        public void ChangeSpeed(float changing)
        {
            Speed = changing;
        }

        public void ResetTimer()
        {
            _timer = TimeSpan.Zero;
        }

        public void Destroy()
        {
            if (InDestruction) return;

            _explosionSound.Play();
            InDestruction = true;
            ComputeActionField(3);
        }

        public void Remove()
        {
            IsAlive = false;
        }

        #endregion

        #endregion
    }
}
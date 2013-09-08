using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Final_Bomber.Sprites;
using Final_Bomber.WorldEngine;
using Final_Bomber.Controls;
using Microsoft.Xna.Framework.Input;
using Final_Bomber.TileEngine;
using Microsoft.Xna.Framework.Media;

namespace Final_Bomber.Entities
{
    public class Bomb : Entity
    {
        #region Field Region
        private enum ExplosionDirection { Down, Left, Right, Up, Middle, Horizontal, Vertical };

        private readonly FinalBomber _gameRef;

        public override sealed AnimatedSprite Sprite { get; protected set; }

        private LookDirection _lookDirection;

        private readonly Animation[] _explosionAnimations;
        private readonly Dictionary<Point, ExplosionDirection> _explosionAnimationsDirection;
        private readonly Texture2D _explosionSpriteTexture;
        private readonly SoundEffect _explosionSound;
        private bool _willExplode;

        private readonly int _power;

        TimeSpan _timer;
        TimeSpan _timerLenght;

        private Point _previousCellPosition;
        private bool _cellChanging;
        private bool _cellTeleporting;

        private int _lastPlayerThatPushIt;

        #endregion

        #region Propery Region

        public int Id { get; private set; }

        public bool IsAlive { get; private set; }

        public bool InDestruction { get; private set; }

        public List<Point> ActionField { get; private set; }

        #endregion

        #region Constructor Region
        public Bomb(FinalBomber game, int id, Point position, int pow, TimeSpan timerLenght, float playerSpeed)
        {
            // The game => you lose :D
            this._gameRef = game;

            // ID => to know whom it is
            this.Id = id;

            // Bomb Sprite
            var spriteTexture = _gameRef.Content.Load<Texture2D>("Graphics/Characters/bomb");
            var animation = new Animation(3, 32, 32, 0, 0, 3);
            this.Sprite = new AnimatedSprite(spriteTexture, animation, Engine.CellToVector(position))
                {
                    IsAnimating = true
                };

            // Bomb's explosion animations
            _explosionSpriteTexture = _gameRef.Content.Load<Texture2D>("Graphics/Characters/explosion");
            const int explosionAnimationsFramesPerSecond = Config.BombLatency;
            _explosionAnimations = new Animation[]
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
            this._explosionSound = _gameRef.GamePlayScreen.BombExplosionSound;

            // Bomb's timer
            _timer = TimeSpan.Zero;
            this._timerLenght = timerLenght;

            // Bomb's states
            this.InDestruction = false;
            this.IsAlive = true;
            this._willExplode = false;
            this._power = pow;
            this._lookDirection = LookDirection.Idle;
            this._previousCellPosition = Sprite.CellPosition;
            this._cellChanging = false;
            this._cellTeleporting = false;

            this._lastPlayerThatPushIt = -1;

            // Action field
            this.ActionField = new List<Point>();
            ComputeActionField(1);
        }
        #endregion

        #region XNA Method

        public override void Update(GameTime gameTime)
        {
            Sprite.Update(gameTime);

            #region Is moving ?
            if (Sprite.CellPosition != _previousCellPosition)
            {
                _cellChanging = true;

                if (_gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel].
                    Map[_previousCellPosition.X, _previousCellPosition.Y] == this)
                {
                    _gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel].
                        Map[_previousCellPosition.X, _previousCellPosition.Y] = null;
                }

                if (_gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel].
                    Map[Sprite.CellPosition.X, Sprite.CellPosition.Y] == null)
                {
                    _gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel].
                    CollisionLayer[Sprite.CellPosition.X, Sprite.CellPosition.Y] = true;
                    
                    _gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel].
                    Map[Sprite.CellPosition.X, Sprite.CellPosition.Y] = this;
                }

                _gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel].
                    CollisionLayer[_previousCellPosition.X, _previousCellPosition.Y] = false;

                if (_cellTeleporting)
                    _cellTeleporting = false;

                this.Sprite.Position = Engine.CellToVector(this.Sprite.CellPosition);
            }
            else
                _cellChanging = false;
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
                Level level = _gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel];
                switch (_lookDirection)
                {
                    case LookDirection.Up:
                        if (Sprite.Position.Y > Engine.TileHeight)
                            Sprite.PositionY = Sprite.Position.Y - Sprite.Speed;
                        else
                            _lookDirection = LookDirection.Idle;
                        
                        var upCell = new Point(this.Sprite.CellPosition.X, this.Sprite.CellPosition.Y - 1);
                        if (level.Map[upCell.X, upCell.Y] is Player || WallAt(upCell))
                        {
                            // Top collision
                            if (_lookDirection == LookDirection.Up && MoreTopSide())
                            {
                                this.Sprite.PositionY = this.Sprite.CellPosition.Y * Engine.TileHeight;
                                _lookDirection = LookDirection.Idle;
                            }
                        }
                        
                        break;
                    case LookDirection.Down:
                        if (Sprite.Position.Y < (level.Size.Y - 2) * Engine.TileHeight)
                            Sprite.PositionY = Sprite.Position.Y + Sprite.Speed;
                        else
                            _lookDirection = LookDirection.Idle;
                        
                        var bottomCell = new Point(this.Sprite.CellPosition.X, this.Sprite.CellPosition.Y + 1);
                        if (level.Map[bottomCell.X, bottomCell.Y] is Player || WallAt(bottomCell))
                        {
                            // Bottom collision
                            if (_lookDirection == LookDirection.Down && MoreBottomSide())
                            {
                                this.Sprite.PositionY = this.Sprite.CellPosition.Y * Engine.TileHeight;
                                _lookDirection = LookDirection.Idle;
                            }
                        }
                        
                        break;
                    case LookDirection.Left:
                        if (Sprite.Position.X > Engine.TileWidth)
                            Sprite.PositionX = Sprite.Position.X - Sprite.Speed;
                        else
                            _lookDirection = LookDirection.Idle;
                        
                        var leftCell = new Point(this.Sprite.CellPosition.X - 1, this.Sprite.CellPosition.Y);
                        if (level.Map[leftCell.X, leftCell.Y] is Player || WallAt(leftCell))
                        {
                            // Left collision
                            if (this._lookDirection == LookDirection.Left && MoreLeftSide())
                            {
                                this.Sprite.PositionX = this.Sprite.CellPosition.X * Engine.TileWidth - Engine.TileWidth / 2 + Engine.TileWidth / 2;
                                _lookDirection = LookDirection.Idle;
                            }
                         }
                        break;
                    case LookDirection.Right:
                        if (Sprite.Position.X < (level.Size.X - 2) * Engine.TileWidth)
                            Sprite.PositionX = Sprite.Position.X + Sprite.Speed;
                        else
                            _lookDirection = LookDirection.Idle;
                        
                        var rightCell = new Point(this.Sprite.CellPosition.X + 1, this.Sprite.CellPosition.Y);
                        if (level.Map[rightCell.X, rightCell.Y] is Player || WallAt(rightCell))
                        {
                            // Right collision
                            if (this._lookDirection == LookDirection.Right && MoreRightSide())
                            {
                                this.Sprite.PositionX = this.Sprite.CellPosition.X * Engine.TileWidth - Engine.TileWidth / 2 + Engine.TileWidth / 2;
                                _lookDirection = LookDirection.Idle;
                            }
                         }
                        break;
                }
                if (_lookDirection == LookDirection.Idle)
                    this.Sprite.Position = Engine.CellToVector(this.Sprite.CellPosition);
            }
            #endregion

            #region Teleporter

            if (!_cellTeleporting && _gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel].
                Map[Sprite.CellPosition.X, Sprite.CellPosition.Y] is Teleporter)
            {
                var teleporter = (Teleporter)(_gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel].
                    Map[Sprite.CellPosition.X, Sprite.CellPosition.Y]);

                teleporter.ChangePosition(this);
                _cellTeleporting = true;
            }

            #endregion

            #region Arrow

            if (!_cellTeleporting && _gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel].
                Map[Sprite.CellPosition.X, Sprite.CellPosition.Y] is Arrow)
            {
                var arrow = (Arrow)(_gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel].
                    Map[Sprite.CellPosition.X, Sprite.CellPosition.Y]);

                arrow.ChangeDirection(this);
            }

            _previousCellPosition = Sprite.CellPosition;
            #endregion

        }

        public override void Draw(GameTime gameTime)
        {
            if (InDestruction)
            {
                Level level = _gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel];

                if (_explosionAnimationsDirection.Count == 0)
                {
                    foreach (Point p in ActionField)
                    {
                        // Is this a wall ? => we don't like wall !
                        if (!level.CollisionLayer[p.X, p.Y] || p == this.Sprite.CellPosition)
                        {
                            // We choose the sprite of explosion for each cell
                            _explosionAnimationsDirection[p] = ComputeExplosionSpriteDirections(p, level);
                        }
                    }
                }
                else
                {
                    foreach (Point p in _explosionAnimationsDirection.Keys)
                    {
                        _gameRef.SpriteBatch.Draw(_explosionSpriteTexture, new Vector2(Engine.Origin.X + p.X * Engine.TileWidth, Engine.Origin.Y + p.Y * Engine.TileHeight),
                            _explosionAnimations[(int) _explosionAnimationsDirection[p]].CurrentFrameRect, Color.White);
                    }
                }
            }
            else
                Sprite.Draw(gameTime, _gameRef.SpriteBatch);
        }

        #endregion

        #region Method Region

        #region Private Method Region

        private bool WallAt(Point cell)
        {
            if (cell.X >= 0 && cell.Y >= 0 && cell.X < _gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel].Size.X &&
                cell.Y < _gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel].Size.Y)
                return (this._gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel].CollisionLayer[cell.X, cell.Y]);
            else
                return false;
        }

        private bool MoreTopSide()
        {
            return this.Sprite.Position.Y < ((this.Sprite.CellPosition.Y * Engine.TileHeight) - (this.Sprite.Speed / 2));
        }

        private bool MoreBottomSide()
        {
            return this.Sprite.Position.Y > ((Sprite.CellPosition.Y * Engine.TileHeight) + (Sprite.Speed / 2));
        }

        private bool MoreLeftSide()
        {
            return this.Sprite.Position.X < ((this.Sprite.CellPosition.X * Engine.TileWidth) - (this.Sprite.Speed / 2));
        }

        private bool MoreRightSide()
        {
            return this.Sprite.Position.X > ((this.Sprite.CellPosition.X * Engine.TileWidth) + (this.Sprite.Speed / 2));
        }

        // Compute bomb's effect field
        private void ComputeActionField(int dangerType)
        {
            // Reset the actionField list
            // We put the bomb in its action field
            this.ActionField = new List<Point> {new Point(this.Sprite.CellPosition.X, this.Sprite.CellPosition.Y)};

            if (_gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel].
                        HazardMap[this.Sprite.CellPosition.X, this.Sprite.CellPosition.Y] < dangerType)
            {
                _gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel].
                HazardMap[this.Sprite.CellPosition.X, this.Sprite.CellPosition.Y] = dangerType;
            }

            // 0 => Top, 1 => Bottom, 2 => Left, 3 => Right
            var obstacles = new Dictionary<String, bool> 
            { 
                {"up", false}, 
                {"down", false}, 
                {"right",false}, 
                {"left", false}
            };

            int tempPower = this._power - 1;
            Level level = _gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel];
            while (tempPower >= 0)
            {
                // Directions
                int up = this.Sprite.CellPosition.Y - (this._power - tempPower);
                int down = this.Sprite.CellPosition.Y + (this._power - tempPower);
                int right = this.Sprite.CellPosition.X + (this._power - tempPower);
                int left = this.Sprite.CellPosition.X - (this._power - tempPower);

                // Up
                Point addPosition;
                if (up >= 0 && !obstacles["up"])
                {
                    if (level.CollisionLayer[this.Sprite.CellPosition.X, up])
                        obstacles["up"] = true;
                    // We don't count the outside walls
                    if (!(level.Map[this.Sprite.CellPosition.X, up] is EdgeWall))
                    {
                        addPosition = new Point(this.Sprite.CellPosition.X, up);
                        this.ActionField.Add(addPosition);
                        if (_gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel].
                        HazardMap[addPosition.X, addPosition.Y] < dangerType)
                        {
                            _gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel].
                            HazardMap[addPosition.X, addPosition.Y] = dangerType;
                        }
                    }
                }

                // Down
                if (down < level.Size.Y - 1 && !obstacles["down"])
                {
                    if (level.CollisionLayer[this.Sprite.CellPosition.X, down])
                        obstacles["down"] = true;
                    // We don't count the outside walls
                    if (!(level.Map[this.Sprite.CellPosition.X, down] is EdgeWall))
                    {
                        addPosition = new Point(this.Sprite.CellPosition.X, down);
                        this.ActionField.Add(addPosition);
                        if (_gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel].
                        HazardMap[addPosition.X, addPosition.Y] < dangerType)
                        {
                            _gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel].
                            HazardMap[addPosition.X, addPosition.Y] = dangerType;
                        }
                    }
                }

                // Right
                if (right < level.Size.X - 1 && !obstacles["right"])
                {
                    if (level.CollisionLayer[right, this.Sprite.CellPosition.Y])
                        obstacles["right"] = true;
                    // We don't count the outside walls
                    if (!(level.Map[right, this.Sprite.CellPosition.Y] is EdgeWall))
                    {
                        addPosition = new Point(right, this.Sprite.CellPosition.Y);
                        this.ActionField.Add(addPosition);
                        if (_gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel].
                        HazardMap[addPosition.X, addPosition.Y] < dangerType)
                        {
                            _gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel].
                            HazardMap[addPosition.X, addPosition.Y] = dangerType;
                        }
                    }
                }

                // Left
                if (left >= 0 && !obstacles["left"])
                {
                    if (level.CollisionLayer[left, this.Sprite.CellPosition.Y])
                        obstacles["left"] = true;
                    // We don't count the outside walls
                    if (!(level.Map[left, this.Sprite.CellPosition.Y] is EdgeWall))
                    {
                        addPosition = new Point(left, this.Sprite.CellPosition.Y);
                        this.ActionField.Add(addPosition);
                        if (_gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel].
                        HazardMap[addPosition.X, addPosition.Y] < dangerType)
                        {
                            _gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel].
                            HazardMap[addPosition.X, addPosition.Y] = dangerType;
                        }
                    }
                }

                tempPower--;
            }
        }

        // 0 => Down, 1 => Left, 2 => Right, 3 => Up, 4 => Middle, 5 => Horizontal, 6 => Vertical 
        private ExplosionDirection ComputeExplosionSpriteDirections(Point cell, Level level)
        {
            int downCell = cell.Y + 1, leftCell = cell.X - 1, rightCell = cell.X + 1, upCell = cell.Y - 1;

            // ~ The middle ~ //
            if (cell.X == this.Sprite.CellPosition.X && cell.Y == this.Sprite.CellPosition.Y)
                return ExplosionDirection.Middle;
 
            // ~ Vertical axis ~ //

            // Top extremity
            if (level.HazardMap[cell.X, upCell] == 0 &&
                (this.ActionField.Find(c => c.X == cell.X && c.Y == downCell) != Point.Zero))
                return ExplosionDirection.Up;
            // Vertical
            else if ((this.ActionField.Find(c => c.X == cell.X && c.Y == downCell) != Point.Zero) &&
                (this.ActionField.Find(c => c.X == cell.X && c.Y == upCell) != Point.Zero))
                return ExplosionDirection.Vertical;
            // Bottom extremity
            else if (level.HazardMap[cell.X, downCell] == 0 &&
                (this.ActionField.Find(c => c.X == cell.X && c.Y == upCell) != Point.Zero))
                return ExplosionDirection.Down;

            // ~ Horizontal axis ~ //

            // Left extremity
            else if (level.HazardMap[leftCell, cell.Y] == 0 &&
                (this.ActionField.Find(c => c.X == rightCell && c.Y == cell.Y) != Point.Zero))
                return ExplosionDirection.Left;
            // Left - Right
            else if ((this.ActionField.Find(c => c.X == rightCell && c.Y == cell.Y) != Point.Zero) &&
                (this.ActionField.Find(c => c.X == leftCell && c.Y == cell.Y) != Point.Zero))
                return ExplosionDirection.Horizontal;
            // Right extremity
            else if (level.HazardMap[rightCell, cell.Y] == 0 &&
                (this.ActionField.Find(c => c.X == leftCell && c.Y == cell.Y) != Point.Zero))
                return ExplosionDirection.Right;

            // ~ Corners ~ //

            // Corner Top - Left
            else if (cell.Y == 1 && cell.X == 1)
            {
                // Left extremity
                if (this.ActionField.Find(c => c.X == rightCell && c.Y == cell.Y) != Point.Zero)
                    return ExplosionDirection.Left;
                // Top extremity
                else
                    return ExplosionDirection.Up;
            }
            // Corner Bottom - Left
            else if (cell.Y == level.Size.Y - 2 && cell.X == 1)
            {
                // Left extremity
                return this.ActionField.Find(c => c.X == rightCell && c.Y == cell.Y) != Point.Zero ? ExplosionDirection.Left : ExplosionDirection.Down;
            }

            // Corner Top - Right
            else if (cell.X == level.Size.X - 2 && cell.Y == 1)
            {
                // Right extremity
                return this.ActionField.Find(c => c.X == leftCell && c.Y == cell.Y) != Point.Zero ? ExplosionDirection.Right : ExplosionDirection.Up;
            }
            // Corner Bottom - Right
            else if (cell.Y == level.Size.Y - 2 && cell.X == level.Size.X - 2)
            {
                // Right extremity
                return this.ActionField.Find(c => c.X == leftCell && c.Y == cell.Y) != Point.Zero ? ExplosionDirection.Right : ExplosionDirection.Down;
            }
            // Error case
            else
                return ExplosionDirection.Middle;

        }

        #endregion

        #region Public Method Region

        public void ChangeDirection(LookDirection lD, int playerId)
        {
            var pos = Point.Zero;
            switch(lD)
            {
                case LookDirection.Up:
                    pos = new Point(Sprite.CellPosition.X, Sprite.CellPosition.Y - 1);
                    break;
                case LookDirection.Down:
                    pos = new Point(Sprite.CellPosition.X, Sprite.CellPosition.Y + 1);
                    break;
                case LookDirection.Left:
                    pos = new Point(Sprite.CellPosition.X - 1, Sprite.CellPosition.Y);
                    break;
                case LookDirection.Right:
                    pos = new Point(Sprite.CellPosition.X + 1, Sprite.CellPosition.Y);
                    break;
            }

            if (!_gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel].
                CollisionLayer[pos.X, pos.Y])
            {
                this._lookDirection = lD;
                this._lastPlayerThatPushIt = playerId;
                foreach (Point p in this.ActionField)
                {
                    _gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel].
                        HazardMap[p.X, p.Y] = 0;
                }
            }
            else
                _lookDirection = LookDirection.Idle;
        }

        public void ChangeSpeed(float changing)
        {
            this.Sprite.Speed = changing;
        }

        public void ResetTimer()
        {
            this._timer = TimeSpan.Zero;
        }

        public override void Destroy()
        {
            if (this.InDestruction) return;

            this._explosionSound.Play();
            this.InDestruction = true;
            ComputeActionField(3);
        }

        public override void Remove()
        {
            this.IsAlive = false;
        }
        #endregion

        #endregion
    }
}

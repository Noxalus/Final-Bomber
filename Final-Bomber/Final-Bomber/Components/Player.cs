using System;
using System.Collections.Generic;
using Final_Bomber.Components.ArtificialIntelligence;
using Final_Bomber.Screens;
using Final_Bomber.Sprites;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Final_Bomber.Controls;
using Final_Bomber.TileEngine;
using Final_Bomber.WorldEngine;
using Microsoft.Xna.Framework.Input;

namespace Final_Bomber.Components
{
    public enum LookDirection { Down, Left, Right, Up, Idle }

    public class Player : MapItem
    {
        #region Field Region

        public override sealed AnimatedSprite Sprite { get; protected set; }

        private readonly AnimatedSprite _playerDeathAnimation;

        private bool _isMoving;

        private TimeSpan _invincibleTimer;
        private TimeSpan _invincibleBlinkTimer;
        private float _invincibleBlinkFrequency;
        
        private Point _previousCellPosition;
        private bool _cellChanging;
        private bool _cellTeleporting;

        private Keys[] _keys;

        private LookDirection _lookDirection;

        private readonly FinalBomber _gameRef;
        private Point _mapSize;

        // Characteristics

        // Bad item
        private TimeSpan _bombTimerSaved;
        private float _speedSaved;
        private Keys[] _keysSaved;

        // Artificial Intelligence
        Vector2 _aiNextPosition;
        public List<Point> AIPath;

        #endregion

        #region Property Region

        public int Id { get; private set; }

        public Camera Camera { get; private set; }

        public bool IsAlive { get; private set; }

        public bool InDestruction { get; private set; }

        public bool IsInvincible { get; private set; }

        public int Power { get; private set; }

        public int CurrentBombNumber { get; set; }

        public int TotalBombNumber { get; private set; }

        public bool OnEdge { get; set; }

        public bool HasBadItemEffect { get; private set; }

        public BadItemEffect BadItemEffect { get; private set; }

        public TimeSpan BadItemTimer { get; private set; }

        public TimeSpan BadItemTimerLenght { get; private set; }

        public TimeSpan BombTimer { get; private set; }

        #endregion

        #region Constructor Region
        public Player(int id, FinalBomber game, Vector2 position)
        {
            this.Id = id;
            this._gameRef = game;

            this._mapSize = game.GamePlayScreen.World.Levels[game.GamePlayScreen.World.CurrentLevel].Size;

            this.Camera = new Camera(_gameRef.ScreenRectangle);

            const int animationFramesPerSecond = 10;
            var animations = new Dictionary<AnimationKey, Animation>();

            var animation = new Animation(4, 23, 23, 0, 0, animationFramesPerSecond);
            animations.Add(AnimationKey.Down, animation);

            animation = new Animation(4, 23, 23, 0, 23, animationFramesPerSecond);
            animations.Add(AnimationKey.Left, animation);

            animation = new Animation(4, 23, 23, 0, 46, animationFramesPerSecond);
            animations.Add(AnimationKey.Right, animation);

            animation = new Animation(4, 23, 23, 0, 69, animationFramesPerSecond);
            animations.Add(AnimationKey.Up, animation);

            var spriteTexture = _gameRef.Content.Load<Texture2D>("Graphics/Characters/player1");
            
            this.Sprite = new AnimatedSprite(spriteTexture, animations, position);
            this.Sprite.ChangeFramesPerSecond(10);
            this.Sprite.Speed = Config.BasePlayerSpeed;

            this._previousCellPosition = this.Sprite.CellPosition; 

            var playerDeathTexture = _gameRef.Content.Load<Texture2D>("Graphics/Characters/player1Death");
            animation = new Animation(8, 23, 23, 0, 0, 4);
            _playerDeathAnimation = new AnimatedSprite(playerDeathTexture, animation, Sprite.Position)
                {
                    IsAnimating = false
                };

            this._isMoving = false;
            this.IsAlive = true;
            this.InDestruction = false;
            this.OnEdge = false;
            this.IsInvincible = true;

            this._invincibleTimer = Config.PlayerInvincibleTimer;
            this._invincibleBlinkFrequency = Config.InvincibleBlinkFrequency;
            this._invincibleBlinkTimer = TimeSpan.FromSeconds(this._invincibleBlinkFrequency);

            Power = Config.BasePlayerBombPower;
            TotalBombNumber = Config.BasePlayerBombNumber;
            CurrentBombNumber = TotalBombNumber;

            _keys = Config.PlayersKeys[id - 1];
            BombTimer = Config.BombTimer;

            _lookDirection = LookDirection.Down;

            // Bad item
            HasBadItemEffect = false;
            BadItemTimer = TimeSpan.Zero;
            BadItemTimerLenght = TimeSpan.Zero;

            // AI
            _aiNextPosition = new Vector2(-1, -1);
            AIPath = new List<Point>();
        }
        #endregion

        #region XNA Method Region
        public override void Update(GameTime gameTime)
        {
            if (IsAlive && !InDestruction)
            {
                Sprite.Update(gameTime);

                #region Invincibility

                if (!Config.Invincible && IsInvincible)
                {
                    if (_invincibleTimer >= TimeSpan.Zero)
                    {
                        _invincibleTimer -= gameTime.ElapsedGameTime;
                        if (_invincibleBlinkTimer >= TimeSpan.Zero)
                            _invincibleBlinkTimer -= gameTime.ElapsedGameTime;
                        else
                            _invincibleBlinkTimer = TimeSpan.FromSeconds(_invincibleBlinkFrequency);
                    }
                    else
                    {
                        _invincibleTimer = Config.PlayerInvincibleTimer;
                        IsInvincible = false;
                    }
                }

                #endregion

                #region Movement

                _cellChanging = _previousCellPosition != Sprite.CellPosition;

                #region Human's player part
                var motion = new Vector2();
                if (!Config.AIPlayers[Id - 1])
                {
                    // Up
                    if (InputHandler.KeyDown(_keys[0]))
                    {
                        Sprite.CurrentAnimation = AnimationKey.Up;
                        _lookDirection = LookDirection.Up;
                        motion.Y = -1;
                    }
                    // Down
                    else if (InputHandler.KeyDown(_keys[1]))
                    {
                        Sprite.CurrentAnimation = AnimationKey.Down;
                        _lookDirection = LookDirection.Down;
                        motion.Y = 1;
                    }
                    // Left
                    else if (InputHandler.KeyDown(_keys[2]))
                    {
                        Sprite.CurrentAnimation = AnimationKey.Left;
                        _lookDirection = LookDirection.Left;
                        motion.X = -1;
                    }
                    // Right
                    else if (InputHandler.KeyDown(_keys[3]))
                    {
                        Sprite.CurrentAnimation = AnimationKey.Right;
                        _lookDirection = LookDirection.Right;
                        motion.X = 1;
                    }
                    else
                        _lookDirection = LookDirection.Idle;
                }
                #endregion

                if (motion != Vector2.Zero || Config.AIPlayers[Id - 1])
                {
                    if (!Config.AIPlayers[Id - 1])
                    {
                        #region Human's player part
                        this._isMoving = true;
                        Sprite.IsAnimating = true;
                        motion.Normalize();

                        Vector2 nextPosition = Sprite.Position + motion * Sprite.Speed;
                        Point nextPositionCell = Engine.VectorToCell(nextPosition, Sprite.Dimension);

                        #region Moving of the player
                        // We move the player
                        Sprite.Position += motion * Sprite.Speed;

                        // If the player want to go to top...
                        if (motion.Y == -1)
                        {
                            // ...  and that there is a wall
                            if (WallAt(new Point(Sprite.CellPosition.X, Sprite.CellPosition.Y - 1)))
                            {
                                // If he is more on the left side, we lag him to the left
                                if (MoreLeftSide() && !WallAt(new Point(Sprite.CellPosition.X - 1, Sprite.CellPosition.Y - 1)))
                                {
                                    if (!WallAt(new Point(Sprite.CellPosition.X - 1, Sprite.CellPosition.Y)))
                                        Sprite.Position = new Vector2(Sprite.Position.X - Sprite.Speed, Sprite.Position.Y);
                                }
                                else if (MoreRightSide() && !WallAt(new Point(Sprite.CellPosition.X + 1, Sprite.CellPosition.Y - 1)))
                                {
                                    if (!WallAt(new Point(Sprite.CellPosition.X + 1, Sprite.CellPosition.Y)))
                                        Sprite.Position = new Vector2(Sprite.Position.X + Sprite.Speed, Sprite.Position.Y);
                                }
                            }
                            // ... and that there is no wall
                            else
                            {
                                // If he is more on the left side
                                if (MoreLeftSide())
                                {
                                    Sprite.Position = new Vector2(Sprite.Position.X + Sprite.Speed, Sprite.Position.Y);
                                }
                                // If he is more on the right side
                                else if (MoreRightSide())
                                {
                                    Sprite.Position = new Vector2(Sprite.Position.X - Sprite.Speed, Sprite.Position.Y);
                                }
                            }
                        }
                        // If the player want to go to bottom and that there is a wall
                        else if (motion.Y == 1)
                        {
                            // Wall at the bottom ?
                            if (WallAt(new Point(Sprite.CellPosition.X, Sprite.CellPosition.Y + 1)))
                            {
                                // If he is more on the left side, we lag him to the left
                                if (MoreLeftSide() && !WallAt(new Point(Sprite.CellPosition.X - 1, Sprite.CellPosition.Y + 1)))
                                {
                                    if (!WallAt(new Point(Sprite.CellPosition.X - 1, Sprite.CellPosition.Y)))
                                        Sprite.Position = new Vector2(Sprite.Position.X - Sprite.Speed, Sprite.Position.Y);
                                }
                                else if (MoreRightSide() && !WallAt(new Point(Sprite.CellPosition.X + 1, Sprite.CellPosition.Y + 1)))
                                {
                                    if (!WallAt(new Point(Sprite.CellPosition.X + 1, Sprite.CellPosition.Y)))
                                        Sprite.Position = new Vector2(Sprite.Position.X + Sprite.Speed, Sprite.Position.Y);
                                }
                            }
                            else
                            {
                                // If he is more on the left side
                                if (MoreLeftSide())
                                {
                                    Sprite.Position = new Vector2(Sprite.Position.X + Sprite.Speed, Sprite.Position.Y);
                                }
                                // If he is more on the right side
                                else if (MoreRightSide())
                                {
                                    Sprite.Position = new Vector2(Sprite.Position.X - Sprite.Speed, Sprite.Position.Y);
                                }
                            }
                        }
                        // If the player want to go to left and that there is a wall
                        else if (motion.X == -1)
                        {
                            if (WallAt(new Point(Sprite.CellPosition.X - 1, Sprite.CellPosition.Y)))
                            {
                                // If he is more on the top side, we lag him to the top
                                if (MoreTopSide() && !WallAt(new Point(Sprite.CellPosition.X - 1, Sprite.CellPosition.Y - 1)))
                                {
                                    if (!WallAt(new Point(Sprite.CellPosition.X, Sprite.CellPosition.Y - 1)))
                                        Sprite.Position = new Vector2(Sprite.Position.X, Sprite.Position.Y - Sprite.Speed);
                                }
                                else if (MoreBottomSide() && !WallAt(new Point(Sprite.CellPosition.X - 1, Sprite.CellPosition.Y + 1)))
                                {
                                    if (!WallAt(new Point(Sprite.CellPosition.X, Sprite.CellPosition.Y + 1)))
                                        Sprite.Position = new Vector2(Sprite.Position.X, Sprite.Position.Y + Sprite.Speed);
                                }
                            }
                            else
                            {
                                // If he is more on the top side, we lag him to the bottom
                                if (MoreTopSide())
                                {
                                    Sprite.Position = new Vector2(Sprite.Position.X, Sprite.Position.Y + Sprite.Speed);
                                }
                                else if (MoreBottomSide())
                                {
                                    Sprite.Position = new Vector2(Sprite.Position.X, Sprite.Position.Y - Sprite.Speed);
                                }
                            }
                        }
                        // If the player want to go to right and that there is a wall
                        else if (motion.X == 1)
                        {
                            if (WallAt(new Point(Sprite.CellPosition.X + 1, Sprite.CellPosition.Y)))
                            {
                                // If he is more on the top side, we lag him to the top
                                if (MoreTopSide() && !WallAt(new Point(Sprite.CellPosition.X + 1, Sprite.CellPosition.Y - 1)))
                                {
                                    if (!WallAt(new Point(Sprite.CellPosition.X, Sprite.CellPosition.Y - 1)))
                                        Sprite.Position = new Vector2(Sprite.Position.X, Sprite.Position.Y - Sprite.Speed);
                                }
                                else if (MoreBottomSide() && !WallAt(new Point(Sprite.CellPosition.X + 1, Sprite.CellPosition.Y + 1)))
                                {
                                    if (!WallAt(new Point(Sprite.CellPosition.X, Sprite.CellPosition.Y + 1)))
                                        Sprite.Position = new Vector2(Sprite.Position.X, Sprite.Position.Y + Sprite.Speed);
                                }
                            }
                            else
                            {
                                // If he is more on the top side, we lag him to the top
                                if (MoreTopSide())
                                {
                                    Sprite.Position = new Vector2(Sprite.Position.X, Sprite.Position.Y + Sprite.Speed);
                                }
                                else if (MoreBottomSide())
                                {
                                    Sprite.Position = new Vector2(Sprite.Position.X, Sprite.Position.Y - Sprite.Speed);
                                }
                            }
                        }
                        #endregion

                        #endregion
                    }
                    else
                    {
                        #region IA part

                        #region Walk
                        // If he hasn't reach his goal => we walk to this goal
                        if ((_aiNextPosition.X != -1 && _aiNextPosition.Y != -1) && !AI.HasReachNextPosition(Sprite.Position, Sprite.Speed, _aiNextPosition))
                        {
                            this._isMoving = true;
                            Sprite.IsAnimating = true;

                            // If the AI is blocked
                            Level level = _gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel];
                            if (level.CollisionLayer[Engine.VectorToCell(_aiNextPosition).X, Engine.VectorToCell(_aiNextPosition).Y] ||
                                level.HazardMap[Engine.VectorToCell(_aiNextPosition).X, Engine.VectorToCell(_aiNextPosition).Y] >= 2)
                            {
                                Sprite.IsAnimating = false;
                                this._isMoving = false;
                                // We define a new goal
                                bool[,] collisionLayer = level.CollisionLayer;
                                int[,] hazardMap = level.HazardMap;
                                MapItem[,] map = level.Map;
                                AIPath = AI.MakeAWay(
                                    Sprite.CellPosition,
                                    AI.SetNewGoal(Sprite.CellPosition, map, collisionLayer, hazardMap, _mapSize),
                                    collisionLayer, hazardMap, _mapSize);
                            }
                            
                            // Up
                            if (Sprite.Position.Y > _aiNextPosition.Y)
                            {
                                Sprite.Position = new Vector2(Sprite.Position.X, Sprite.Position.Y - Sprite.Speed);
                                Sprite.CurrentAnimation = AnimationKey.Up;
                                _lookDirection = LookDirection.Up;
                            }
                            // Down
                            else if (Sprite.Position.Y < _aiNextPosition.Y)
                            {
                                Sprite.Position = new Vector2(Sprite.Position.X, Sprite.Position.Y + Sprite.Speed);
                                Sprite.CurrentAnimation = AnimationKey.Down;
                                _lookDirection = LookDirection.Down;
                            }
                            // Right
                            else if (Sprite.Position.X < _aiNextPosition.X)
                            {
                                Sprite.Position = new Vector2(Sprite.Position.X + Sprite.Speed, Sprite.Position.Y);
                                Sprite.CurrentAnimation = AnimationKey.Right;
                                _lookDirection = LookDirection.Right;
                            }
                            // Left
                            else if (Sprite.Position.X > _aiNextPosition.X)
                            {
                                Sprite.Position = new Vector2(Sprite.Position.X - Sprite.Speed, Sprite.Position.Y);
                                Sprite.CurrentAnimation = AnimationKey.Left;
                                _lookDirection = LookDirection.Left;
                            }
                        }
                        #endregion

                        #region Search a goal
                        // Otherwise => we find another goal
                        else
                        {
                            // We place the player at the center of its cell
                            Sprite.Position = Engine.CellToVector(Sprite.CellPosition);

                            Level level = _gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel];

                            #region Bomb => AI
                            // Try to put a bomb
                                // Put a bomb
                            if (!HasBadItemEffect || (HasBadItemEffect && BadItemEffect != BadItemEffect.NoBomb))
                            {
                                if (AI.TryToPutBomb(Sprite.CellPosition, Power, level.Map, level.CollisionLayer, level.HazardMap, _mapSize))
                                {
                                    if (this.CurrentBombNumber > 0)
                                    {
                                        var bo = _gameRef.GamePlayScreen.BombList.Find(b => b.Sprite.CellPosition == this.Sprite.CellPosition);
                                        if (bo == null)
                                        {
                                            this.CurrentBombNumber--;
                                            var bomb = new Bomb(_gameRef, this.Id, Sprite.CellPosition, this.Power, this.BombTimer, this.Sprite.Speed);

                                            if (_gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel].
                                                Map[Sprite.CellPosition.X, Sprite.CellPosition.Y] is Player)
                                            {
                                                _gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel].
                                                    Map[Sprite.CellPosition.X, Sprite.CellPosition.Y] = bomb;
                                                _gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel].
                                                CollisionLayer[bomb.Sprite.CellPosition.X, bomb.Sprite.CellPosition.Y] = true;

                                                // We define a new way (to escape the bomb)
                                                AIPath = AI.MakeAWay(
                                                    Sprite.CellPosition,
                                                    AI.SetNewDefenseGoal(Sprite.CellPosition, level.CollisionLayer, level.HazardMap, _mapSize),
                                                    level.CollisionLayer, level.HazardMap, _mapSize);
                                            }
                                            _gameRef.GamePlayScreen.BombList.Add(bomb);
                                        }
                                    }
                                }
                            }
                            #endregion

                            if (AIPath == null || AIPath.Count == 0)
                            {
                                Sprite.IsAnimating = false;
                                this._isMoving = false;
                                // We define a new goal
                                AIPath = AI.MakeAWay(
                                    Sprite.CellPosition,
                                    AI.SetNewGoal(Sprite.CellPosition, level.Map, level.CollisionLayer, level.HazardMap, _mapSize),
                                    level.CollisionLayer, level.HazardMap, _mapSize);

                                if (AIPath != null)
                                {
                                    _aiNextPosition = Engine.CellToVector(AIPath[AIPath.Count - 1]);
                                    AIPath.Remove(AIPath[AIPath.Count - 1]);

                                    // If the AI is blocked
                                    if (level.CollisionLayer[Engine.VectorToCell(_aiNextPosition).X, Engine.VectorToCell(_aiNextPosition).Y] ||
                                        level.HazardMap[Engine.VectorToCell(_aiNextPosition).X, Engine.VectorToCell(_aiNextPosition).Y] >= 2)
                                    {
                                        Sprite.IsAnimating = false;
                                        this._isMoving = false;
                                        // We define a new goal
                                        bool[,] collisionLayer = level.CollisionLayer;
                                        int[,] hazardMap = level.HazardMap;
                                        MapItem[,] map = level.Map;
                                        AIPath = AI.MakeAWay(
                                            Sprite.CellPosition,
                                            AI.SetNewGoal(Sprite.CellPosition, map, collisionLayer, hazardMap, _mapSize),
                                            collisionLayer, hazardMap, _mapSize);
                                    }
                                }
                            }
                            else
                            {
                                // We finish the current way
                                _aiNextPosition = Engine.CellToVector(AIPath[AIPath.Count - 1]);
                                AIPath.Remove(AIPath[AIPath.Count - 1]);
                                /*
                                // Update the way of the AI each time it changes of cell => usefull to battle against players (little bug here)
                                aiWay = AI.MakeAWay(
                                    Sprite.CellPosition,
                                    AI.SetNewGoal(Sprite.CellPosition, level.Map, level.CollisionLayer, level.HazardMap), 
                                    level.CollisionLayer, level.HazardMap);
                                */
                            }
                        }
                        #endregion

                        #endregion
                    }

                    Sprite.LockToMap();
                    
                    #region Wall collisions
                    // -- Vertical check -- //
                    // Is there a wall on the top ?
                    if (WallAt(new Point(this.Sprite.CellPosition.X, this.Sprite.CellPosition.Y - 1)))
                    {
                        // Is there a wall on the bottom ?
                        if (WallAt(new Point(this.Sprite.CellPosition.X, this.Sprite.CellPosition.Y + 1)))
                        {
                            // Top collision and Bottom collision
                            if ((_lookDirection == LookDirection.Up && MoreTopSide()) || (_lookDirection == LookDirection.Down && MoreBottomSide()))
                                this.Sprite.PositionY = this.Sprite.CellPosition.Y * Engine.TileHeight;
                        }
                        // No wall at the bottom
                        else
                        {
                            // Top collision
                            if (_lookDirection == LookDirection.Up && MoreTopSide())
                                this.Sprite.PositionY = this.Sprite.CellPosition.Y * Engine.TileHeight;
                        }
                    }
                    // Wall only at the bottom
                    else if (WallAt(new Point(this.Sprite.CellPosition.X, this.Sprite.CellPosition.Y + 1)))
                    {
                        // Bottom collision
                        if (_lookDirection == LookDirection.Down && MoreBottomSide())
                            this.Sprite.PositionY = this.Sprite.CellPosition.Y * Engine.TileHeight;
                        // To lag him
                        else if (_lookDirection == LookDirection.Down)
                        {
                            if(MoreLeftSide())
                                this.Sprite.PositionX += this.Sprite.Speed;
                            else if(MoreRightSide())
                                this.Sprite.PositionX -= this.Sprite.Speed;
                        }
                    }

                    // -- Horizontal check -- //
                    // Is there a wall on the left ?
                    if (WallAt(new Point(Sprite.CellPosition.X - 1, Sprite.CellPosition.Y)))
                    {
                        // Is there a wall on the right ?
                        if (WallAt(new Point(Sprite.CellPosition.X + 1, Sprite.CellPosition.Y)))
                        {
                            // Left and right collisions
                            if ((this._lookDirection == LookDirection.Left && MoreLeftSide()) || (this._lookDirection == LookDirection.Right && MoreRightSide()))
                                this.Sprite.PositionX = this.Sprite.CellPosition.X * Engine.TileWidth - Engine.TileWidth / 2 + Engine.TileWidth / 2;
                        }
                        // Wall only at the left
                        else
                        {
                            // Left collision
                            if (this._lookDirection == LookDirection.Left && MoreLeftSide())
                                this.Sprite.PositionX = this.Sprite.CellPosition.X * Engine.TileWidth - Engine.TileWidth / 2 + Engine.TileWidth / 2;
                        }
                    }
                    // Wall only at the right
                    else if (WallAt(new Point(this.Sprite.CellPosition.X + 1, this.Sprite.CellPosition.Y)))
                    {
                        // Right collision
                        if (this._lookDirection == LookDirection.Right && MoreRightSide())
                            this.Sprite.PositionX = this.Sprite.CellPosition.X * Engine.TileWidth - Engine.TileWidth / 2 + Engine.TileWidth / 2;
                    }

                    
                    // The player must stay in the map
                    this.Sprite.PositionX = MathHelper.Clamp(this.Sprite.Position.X, Engine.TileWidth,
                        (_gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel].Size.X * Engine.TileWidth) - 2 * Engine.TileWidth);
                    this.Sprite.PositionY = MathHelper.Clamp(this.Sprite.Position.Y, Engine.TileHeight,
                        (_gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel].Size.Y * Engine.TileHeight) - 2 * Engine.TileHeight);
                    
                    #endregion
                    
                }
                else
                {
                    this._isMoving = false;
                    Sprite.IsAnimating = false;
                }

                #region Mise à jour de la position du joueur
                if (_cellChanging)
                {
                    if (_gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel].
                       Map[_previousCellPosition.X, _previousCellPosition.Y] == this)
                    {
                        _gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel].
                            Map[_previousCellPosition.X, _previousCellPosition.Y] = null;
                    }

                    if (_cellTeleporting)
                        _cellTeleporting = false;
                }
                else
                {
                    if (_gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel].
                        Map[Sprite.CellPosition.X, Sprite.CellPosition.Y] == null)
                    {
                        _gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel].
                            Map[Sprite.CellPosition.X, Sprite.CellPosition.Y] = this;
                    }
                }
                #endregion

                #endregion

                #region Bomb

                #region Human player's part
                if (!Config.AIPlayers[Id - 1])
                {
                    if ((HasBadItemEffect && BadItemEffect == BadItemEffect.BombDrop) || (InputHandler.KeyPressed(_keys[4]) &&
                        (!HasBadItemEffect || (HasBadItemEffect && BadItemEffect != BadItemEffect.NoBomb))))
                    {
                        if (this.CurrentBombNumber > 0)
                        {
                            var bo = _gameRef.GamePlayScreen.BombList.Find(b => b.Sprite.CellPosition == this.Sprite.CellPosition);
                            if (bo == null)
                            {
                                this.CurrentBombNumber--;
                                var bomb = new Bomb(_gameRef, this.Id, Sprite.CellPosition, this.Power, this.BombTimer, this.Sprite.Speed);

                                if (_gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel].
                                    Map[Sprite.CellPosition.X, Sprite.CellPosition.Y] is Player)
                                {
                                    _gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel].
                                        Map[Sprite.CellPosition.X, Sprite.CellPosition.Y] = bomb;
                                    _gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel].
                                    CollisionLayer[bomb.Sprite.CellPosition.X, bomb.Sprite.CellPosition.Y] = true;
                                }

                                _gameRef.GamePlayScreen.BombList.Add(bomb);
                            }
                        }
                    }
                }
                #endregion

                #region Push a bomb

                if (Config.PlayerCanPush)
                {
                    if (_lookDirection != LookDirection.Idle)
                    {
                        Point direction = Point.Zero;
                        switch (_lookDirection)
                        {
                            case LookDirection.Up:
                                direction = new Point(Sprite.CellPosition.X, Sprite.CellPosition.Y - 1);
                                break;
                            case LookDirection.Down:
                                direction = new Point(Sprite.CellPosition.X, Sprite.CellPosition.Y + 1);
                                break;
                            case LookDirection.Left:
                                direction = new Point(Sprite.CellPosition.X - 1, Sprite.CellPosition.Y);
                                break;
                            case LookDirection.Right:
                                direction = new Point(Sprite.CellPosition.X + 1, Sprite.CellPosition.Y);
                                break;
                        }
                        Bomb bomb = BombAt(direction);
                        if (bomb != null)
                            bomb.ChangeDirection(_lookDirection, this.Id);
                    }
                }

                #endregion

                #endregion

                #region Item

                if (_gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel].
                    Map[Sprite.CellPosition.X, Sprite.CellPosition.Y] is Item)
                {
                    var item = (Item)(_gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel].
                        Map[Sprite.CellPosition.X, Sprite.CellPosition.Y]);
                    if (!item.InDestruction)
                    {
                        if (!HasBadItemEffect || (HasBadItemEffect && item.Type != ItemType.BadItem))
                        {
                            item.ApplyItem(this);
                            _gameRef.GamePlayScreen.ItemPickUpSound.Play();
                            item.Remove();
                        }
                    }
                }

                // Have caught a bad item
                if (HasBadItemEffect)
                {
                    BadItemTimer += gameTime.ElapsedGameTime;
                    if (BadItemTimer >= BadItemTimerLenght)
                    {
                        switch (BadItemEffect)
                        {
                            case BadItemEffect.TooSlow:
                                Sprite.Speed = _speedSaved;
                                break;
                            case BadItemEffect.TooSpeed:
                                Sprite.Speed = _speedSaved;
                                break;
                            case BadItemEffect.KeysInversion:
                                _keys = _keysSaved;
                                break;
                            case BadItemEffect.BombTimerChanged:
                                BombTimer = _bombTimerSaved;
                                break;
                        }
                        HasBadItemEffect = false;
                        BadItemTimer = TimeSpan.Zero;
                        BadItemTimerLenght = TimeSpan.Zero;
                    }
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
            }
            #region Death
            else if(_playerDeathAnimation.IsAnimating)
            {
                _playerDeathAnimation.Update(gameTime);

                if (_playerDeathAnimation.Animation.CurrentFrame == _playerDeathAnimation.Animation.FrameCount - 1)
                    Remove();
            }
            #endregion
            #region Edge wall gameplay
            else if (OnEdge && (!Config.ActiveSuddenDeath || (Config.ActiveSuddenDeath && !_gameRef.GamePlayScreen.SuddenDeath.HasStarted)))
            {
                Sprite.Update(gameTime);

                // The player is either at the top either at the bottom
                // => he can only move on the right or on the left
                if (Sprite.Position.Y <= 0 || Sprite.Position.Y >= (_mapSize.Y - 1) * Engine.TileHeight)
                {
                    // If he wants to go to the left
                    if (Sprite.Position.X > 0 && InputHandler.KeyDown(_keys[2]))
                        Sprite.Position = new Vector2(Sprite.Position.X - Sprite.Speed, Sprite.Position.Y);
                    // If he wants to go to the right
                    else if (Sprite.Position.X < (_mapSize.X * Engine.TileWidth) - Engine.TileWidth &&
                        InputHandler.KeyDown(_keys[3]))
                        Sprite.Position = new Vector2(Sprite.Position.X + Sprite.Speed, Sprite.Position.Y);
                }
                // The player is either on the left either on the right
                if (Sprite.Position.X <= 0 || Sprite.Position.X >= (_mapSize.X - 1) * Engine.TileWidth)
                {
                    // If he wants to go to the top
                    if (Sprite.Position.Y > 0 && InputHandler.KeyDown(_keys[0]))
                        Sprite.Position = new Vector2(Sprite.Position.X, Sprite.Position.Y - Sprite.Speed);
                    // If he wants to go to the bottom
                    else if (Sprite.Position.Y < (_mapSize.Y * Engine.TileHeight) - Engine.TileHeight &&
                        InputHandler.KeyDown(_keys[1]))
                        Sprite.Position = new Vector2(Sprite.Position.X, Sprite.Position.Y + Sprite.Speed);
                }

                if (Sprite.Position.Y <= 0)
                    Sprite.CurrentAnimation = AnimationKey.Down;
                else if (Sprite.Position.Y >= (_mapSize.Y - 1) * Engine.TileHeight)
                    Sprite.CurrentAnimation = AnimationKey.Up;
                else if (Sprite.Position.X <= 0)
                        Sprite.CurrentAnimation = AnimationKey.Right;
                else if (Sprite.Position.X >= (_mapSize.X - 1) * Engine.TileWidth)
                        Sprite.CurrentAnimation = AnimationKey.Left;

                #region Bombs => Edge gameplay

                if (InputHandler.KeyDown(_keys[4]) && this.CurrentBombNumber > 0)
                {
                    // He can't put a bomb when he is on a corner
                    if (!((Sprite.CellPosition.Y == 0 && (Sprite.CellPosition.X == 0 || Sprite.CellPosition.X == _mapSize.X - 1)) ||
                        (Sprite.CellPosition.Y == _mapSize.Y - 1 && (Sprite.CellPosition.X == 0 || (Sprite.CellPosition.X == _mapSize.X - 1)))))
                    {
                        Level level = _gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel];
                        int lag = 0;
                        Point bombPosition = Sprite.CellPosition;
                        // Up
                        if (Sprite.CellPosition.Y == 0)
                        {
                            while (Sprite.CellPosition.Y + lag + 3 < _mapSize.Y &&
                                    level.CollisionLayer[Sprite.CellPosition.X, Sprite.CellPosition.Y + lag + 3])
                            {
                                lag++;
                            }
                            bombPosition.Y = Sprite.CellPosition.Y + lag + 3;
                            if (bombPosition.Y < _mapSize.Y)
                            {
                                var bomb = new Bomb(_gameRef, Id, bombPosition, Power, BombTimer, Config.BaseBombSpeed + Sprite.Speed);
                                level.CollisionLayer[bombPosition.X, bombPosition.Y] = true;
                                _gameRef.GamePlayScreen.BombList.Add(bomb);
                                level.Map[bombPosition.X, bombPosition.Y] = bomb;
                                this.CurrentBombNumber--;
                            }
                        }
                        // Down
                        if (Sprite.CellPosition.Y == _mapSize.Y - 1)
                        {
                            while (Sprite.CellPosition.Y - lag - 3 >= 0 &&
                                    level.CollisionLayer[Sprite.CellPosition.X, Sprite.CellPosition.Y - lag - 3])
                            {
                                lag++;
                            }
                            bombPosition.Y = Sprite.CellPosition.Y - lag - 3;
                            if (bombPosition.Y >= 0)
                            {
                                var bomb = new Bomb(_gameRef, Id, bombPosition, Power, BombTimer, Config.BaseBombSpeed + Sprite.Speed);
                                level.CollisionLayer[bombPosition.X, bombPosition.Y] = true;
                                _gameRef.GamePlayScreen.BombList.Add(bomb);
                                level.Map[bombPosition.X, bombPosition.Y] = bomb;
                                this.CurrentBombNumber--;
                            }
                        }
                        // Left
                        if (Sprite.CellPosition.X == 0)
                        {
                            while (Sprite.CellPosition.X + lag + 3 < _mapSize.X &&
                                    level.CollisionLayer[Sprite.CellPosition.X + lag + 3, Sprite.CellPosition.Y])
                            {
                                lag++;
                            }
                            bombPosition.X = Sprite.CellPosition.X + lag + 3;
                            if (bombPosition.X < _mapSize.X)
                            {
                                var bomb = new Bomb(_gameRef, Id, bombPosition, Power, BombTimer, Config.BaseBombSpeed + Sprite.Speed);
                                level.CollisionLayer[bombPosition.X, bombPosition.Y] = true;
                                _gameRef.GamePlayScreen.BombList.Add(bomb);
                                level.Map[bombPosition.X, bombPosition.Y] = bomb;
                                this.CurrentBombNumber--;
                            }
                        }
                        // Right
                        if (Sprite.CellPosition.X == _mapSize.X - 1)
                        {
                            while (Sprite.CellPosition.X - lag - 3 >= 0 &&
                                    level.CollisionLayer[Sprite.CellPosition.X - lag - 3, Sprite.CellPosition.Y])
                            {
                                lag++;
                            }
                            bombPosition.X = Sprite.CellPosition.X - lag - 3;
                            if (bombPosition.X >= 0)
                            {
                                var bomb = new Bomb(_gameRef, Id, bombPosition, Power, BombTimer, Config.BaseBombSpeed + Sprite.Speed);
                                level.CollisionLayer[bombPosition.X, bombPosition.Y] = true;
                                _gameRef.GamePlayScreen.BombList.Add(bomb);
                                level.Map[bombPosition.X, bombPosition.Y] = bomb;
                                this.CurrentBombNumber--;
                            }
                        }
                    }
                }

                #endregion
            }
            #endregion

            #region Camera
            Camera.Update(gameTime);

            if (Config.Debug)
            {
                if (InputHandler.KeyDown(Keys.PageUp) ||
                    InputHandler.ButtonReleased(Buttons.LeftShoulder, PlayerIndex.One))
                {
                    Camera.ZoomIn();
                    if (Camera.CameraMode == CameraMode.Follow)
                        Camera.LockToSprite(Sprite);
                }
                else if (InputHandler.KeyDown(Keys.PageDown))
                {
                    Camera.ZoomOut();
                    if (Camera.CameraMode == CameraMode.Follow)
                        Camera.LockToSprite(Sprite);
                }
                else if (InputHandler.KeyDown(Keys.End))
                {
                    Camera.ZoomReset();
                }

                if (InputHandler.KeyReleased(Keys.F))
                {
                    Camera.ToggleCameraMode();
                    if (Camera.CameraMode == CameraMode.Follow)
                        Camera.LockToSprite(Sprite);
                }

                if (Camera.CameraMode != CameraMode.Follow)
                {
                    if (InputHandler.KeyReleased(Keys.L))
                    {
                        Camera.LockToSprite(Sprite);
                    }
                }
            }

            if (_isMoving && Camera.CameraMode == CameraMode.Follow)
                Camera.LockToSprite(Sprite);
            #endregion

            _previousCellPosition = Sprite.CellPosition;
        }

        public override void Draw(GameTime gameTime)
        {
            var playerNamePosition = new Vector2(
                Sprite.Position.X + Engine.Origin.X + Sprite.Width/2 - 
                ControlManager.SpriteFont.MeasureString(Config.PlayersName[Id - 1]).X / 2 + 5,
                Sprite.Position.Y + Engine.Origin.Y - 25 - 
                ControlManager.SpriteFont.MeasureString(Config.PlayersName[Id - 1]).Y / 2);

            if ((IsAlive && !InDestruction) || OnEdge)
            {
                if (IsInvincible)
                {
                    if (_invincibleBlinkTimer > TimeSpan.FromSeconds(_invincibleBlinkFrequency * 0.5f))
                    {
                        Sprite.Draw(gameTime, _gameRef.SpriteBatch);
                        _gameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, Config.PlayersName[Id - 1], playerNamePosition, Color.Black);
                    }
                }
                else
                {
                    Sprite.Draw(gameTime, _gameRef.SpriteBatch);
                    _gameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, Config.PlayersName[Id - 1], playerNamePosition, Color.Black);
                }
            }
            else
            {
                _playerDeathAnimation.Draw(gameTime, _gameRef.SpriteBatch);
                _gameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, Config.PlayersName[Id - 1], playerNamePosition, Color.Black);
            }
        }
        #endregion

        #region Method Region

        #region Private Method Region

        private Bomb BombAt(Point cell)
        {
            if (cell.X >= 0 && cell.Y >= 0 && cell.X < _gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel].Size.X &&
                cell.Y < _gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel].Size.Y)
            {
                Bomb bomb = _gameRef.GamePlayScreen.BombList.Find(b => b.Sprite.CellPosition == cell);
                return (bomb);
            }
            else
                return null;
        }

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

        private void Invincibility()
        {
            this.IsInvincible = true;
            this._invincibleTimer = Config.PlayerInvincibleTimer;
            this._invincibleBlinkFrequency = Config.InvincibleBlinkFrequency;
            this._invincibleBlinkTimer = TimeSpan.FromSeconds(this._invincibleBlinkFrequency);
        }

        #endregion

        #region Public Method Region

        public void IncreaseTotalBombNumber(int incr)
        {
            if (this.TotalBombNumber + incr > Config.MaxBombNumber)
            {
                this.TotalBombNumber = Config.MaxBombNumber;
                this.CurrentBombNumber = TotalBombNumber;
            }
            else if (this.TotalBombNumber + incr < Config.MinBombNumber)
            {
                this.TotalBombNumber = Config.MinBombNumber;
                this.CurrentBombNumber = TotalBombNumber;
            }
            else
            {
                this.TotalBombNumber += incr;
                this.CurrentBombNumber += incr;
            }
        }

        public void IncreasePower(int incr)
        {
            if (this.Power + incr > Config.MaxBombPower)
                this.Power = Config.MaxBombPower;
            else if (this.Power + incr < Config.MinBombPower)
                this.Power = Config.MinBombPower;
            else
                this.Power += incr;
        }

        public void IncreaseSpeed(float incr)
        {
            this.Sprite.Speed += incr;
        }

        public void ApplyBadItem(BadItemEffect effect)
        {
            this.HasBadItemEffect = true;
            this.BadItemEffect = effect;
            this.BadItemTimerLenght = TimeSpan.FromSeconds(GamePlayScreen.Random.Next(Config.BadItemTimerMin, Config.BadItemTimerMax));
            switch (effect)
            {
                case BadItemEffect.TooSlow:
                    this._speedSaved = this.Sprite.Speed;
                    this.Sprite.Speed = Config.MinSpeed;
                    break;
                case BadItemEffect.TooSpeed:
                    _speedSaved = this.Sprite.Speed;
                    this.Sprite.Speed = Config.MaxSpeed;
                    break;
                case BadItemEffect.KeysInversion:
                    this._keysSaved = (Keys[])this._keys.Clone();
                    var inversedKeysArray = new int[] { 1, 0, 3, 2 };
                    for (int i = 0; i < inversedKeysArray.Length; i++)
                        this._keys[i] = this._keysSaved[inversedKeysArray[i]];
                    break;
                case BadItemEffect.BombTimerChanged:
                    this._bombTimerSaved = this.BombTimer;
                    int randomBombTimer = GamePlayScreen.Random.Next(Config.BadItemTimerChangedMin, Config.BadItemTimerChangedMax);
                    this.BombTimer = TimeSpan.FromSeconds(randomBombTimer);
                    break;
            }
        }

        public void Rebirth(Vector2 position)
        {
            this.IsAlive = true;
            this.Sprite.IsAnimating = true;
            this.InDestruction = false;
            this.Sprite.Position = position;
            this.Sprite.CurrentAnimation = AnimationKey.Down;
            this._playerDeathAnimation.IsAnimating = false;
            
            Invincibility();
        }
        
        #endregion

        #region Override Method Region

        public override void Destroy()
        {
            if (!this.InDestruction)
            {
                this.Sprite.IsAnimating = false;
                this.InDestruction = true;
                _gameRef.GamePlayScreen.PlayerDeathSound.Play();
                this._playerDeathAnimation.Position = this.Sprite.Position;
                this._playerDeathAnimation.IsAnimating = true;
            }
        }

        public override void Remove()
        {
            this._playerDeathAnimation.IsAnimating = false;
            this.InDestruction = false;
            this.IsAlive = false;

            // Replacing for the gameplay on the edges
            // Right side
            if (_mapSize.X - this.Sprite.CellPosition.X < _mapSize.X / 2)
            {
                this.Sprite.CurrentAnimation = AnimationKey.Left;
                this.Sprite.Position = new Vector2((_mapSize.X * Engine.TileWidth) - Engine.TileWidth, this.Sprite.Position.Y);
            }
            // Left side
            else
            {
                this.Sprite.CurrentAnimation = AnimationKey.Right;
                this.Sprite.Position = new Vector2(0, this.Sprite.Position.Y);
            }
        }

        #endregion

        #endregion
    }
}

using System;
using System.Collections.Generic;
using Final_Bomber.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Final_Bomber.TileEngine;
using Microsoft.Xna.Framework.Media;

namespace Final_Bomber.Entities
{
    public class SuddenDeath
    {
        #region Field Region
        private readonly FinalBomber _gameRef;
        private Point _mapSize;
        private bool _hasStarted;
        private readonly Texture2D _suddenDeathShadow;
        private readonly Texture2D _suddenDeathWall;
        private Point _currentPosition;
        private Point _previousPosition;
        private TimeSpan _timer;
        private TimeSpan _moveTime;
        private LookDirection _lookDirection;
        private readonly bool[,] _visited;

        // Map moving
        private Vector2 _nextMapPosition;
        #endregion

        #region Propery Region

        public bool HasStarted
        {
            get { return _hasStarted; }
        }

        public bool[,] Visited
        {
            get { return _visited; }
        }
        #endregion

        #region Constructor Region
        public SuddenDeath(FinalBomber game, Point pos)
        {
            _gameRef = game;
            _hasStarted = false;
            _suddenDeathShadow = _gameRef.Content.Load<Texture2D>("Graphics/Characters/suddenDeathShadow");
            _suddenDeathWall = _gameRef.Content.Load<Texture2D>("Graphics/Characters/edgeWall");
            _currentPosition = pos;
            _previousPosition = Point.Zero;

            _timer = TimeSpan.Zero;
            _moveTime = TimeSpan.FromSeconds(Config.SuddenDeathWallSpeed);
            _lookDirection = LookDirection.Right;

            _mapSize = Config.MapSize;
            //_mapSize = game.GamePlayScreen.World.Levels[game.GamePlayScreen.World.CurrentLevel].Size;            

            _visited = new bool[_mapSize.X, _mapSize.Y];
            for (int i = 0; i < _mapSize.X; i++)
            {
                _visited[i, 0] = true;
                _visited[i, _mapSize.Y - 1] = true;
            }
            for (int i = 0; i < _mapSize.Y; i++)
            {
                _visited[0, i] = true;
                _visited[_mapSize.X - 1, i] = true;
            }

            // Map moving
            _nextMapPosition = Vector2.Zero;
        }
        #endregion

        #region XNA Method Region
        public void Update(GameTime gameTime)
        {
            _timer += gameTime.ElapsedGameTime;

            if (!_hasStarted && _timer >= Config.SuddenDeathTimer)
            {
                MediaPlayer.Play(_gameRef.GamePlayScreen.MapSongHurry);
                _hasStarted = true;
                _timer = TimeSpan.Zero;
            }

            if (_hasStarted)
            {
                #region Walls
                // We change sudden death's wall position
                if (_timer >= _moveTime)
                {
                    if (!AllVisited())
                    {
                        _visited[_currentPosition.X, _currentPosition.Y] = true;
                        Entity mapItem = _gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel].
                            Map[_currentPosition.X, _currentPosition.Y];
                        if (mapItem != null)
                        {
                            if (mapItem is Player)
                            {
                                mapItem.Destroy();
                                List<Player> pl = _gameRef.GamePlayScreen.PlayerList.FindAll(p => p.Sprite.CellPosition == _currentPosition);
                                foreach (Player p in pl)
                                    p.Destroy();
                            }
                            else if (mapItem is Teleporter || mapItem is Arrow || mapItem is Bomb)
                            {
                                List<Player> pl = _gameRef.GamePlayScreen.PlayerList.FindAll(p => p.Sprite.CellPosition == _currentPosition);
                                foreach(Player p in pl)
                                    p.Destroy();
                                List<Bomb> bl = _gameRef.GamePlayScreen.BombList.FindAll(b => b.Sprite.CellPosition == _currentPosition);
                                foreach(Bomb b in bl)
                                    b.Remove();
                                mapItem.Remove();
                            }
                            else
                                mapItem.Remove();
                        }

                        _gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel].CollisionLayer[_currentPosition.X, _currentPosition.Y] = true;
                        var u = new UnbreakableWall(_gameRef, Engine.CellToVector(_currentPosition));
                        _gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel].Map[_currentPosition.X, _currentPosition.Y] = u;
                        _gameRef.GamePlayScreen.UnbreakableWallList.Add(u);
                        _timer = TimeSpan.Zero;
                        _previousPosition = _currentPosition;

                        switch (_lookDirection)
                        {
                            case LookDirection.Up:
                                _currentPosition.Y--;
                                if (!_visited[_currentPosition.X, _currentPosition.Y] &&
                                    _gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel].
                                    Map[_currentPosition.X, _currentPosition.Y] is UnbreakableWall)
                                {
                                    _visited[_currentPosition.X, _currentPosition.Y] = true;
                                    if (_visited[_currentPosition.X, _currentPosition.Y - 1])
                                        _lookDirection = LookDirection.Right;
                                    else
                                        _currentPosition.Y--;
                                }
                                if (_visited[_currentPosition.X, _currentPosition.Y - 1])
                                    _lookDirection = LookDirection.Right;
                                break;
                            case LookDirection.Down:
                                _currentPosition.Y++;
                                if (!_visited[_currentPosition.X, _currentPosition.Y] &&
                                    _gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel].
                                    Map[_currentPosition.X, _currentPosition.Y] is UnbreakableWall)
                                {
                                    _visited[_currentPosition.X, _currentPosition.Y] = true;
                                    if (_visited[_currentPosition.X, _currentPosition.Y + 1])
                                        _lookDirection = LookDirection.Left;
                                    else
                                        _currentPosition.Y++;
                                }
                                if (_visited[_currentPosition.X, _currentPosition.Y + 1])
                                    _lookDirection = LookDirection.Left;
                                break;
                            case LookDirection.Left:
                                _currentPosition.X--;
                                if (!_visited[_currentPosition.X, _currentPosition.Y] &&
                                    _gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel].
                                    Map[_currentPosition.X, _currentPosition.Y] is UnbreakableWall)
                                {
                                    _visited[_currentPosition.X, _currentPosition.Y] = true;
                                    if (_visited[_currentPosition.X - 1, _currentPosition.Y])
                                        _lookDirection = LookDirection.Up;
                                    else
                                        _currentPosition.X--;
                                }
                                if (_visited[_currentPosition.X - 1, _currentPosition.Y])
                                    _lookDirection = LookDirection.Up;
                                break;
                            case LookDirection.Right:
                                _currentPosition.X++;
                                if (!_visited[_currentPosition.X, _currentPosition.Y] &&
                                    _gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel].
                                    Map[_currentPosition.X, _currentPosition.Y] is UnbreakableWall)
                                {
                                    _visited[_currentPosition.X, _currentPosition.Y] = true;
                                    if (_visited[_currentPosition.X + 1, _currentPosition.Y])
                                        _lookDirection = LookDirection.Down;
                                    else
                                        _currentPosition.X++;
                                }
                                if (_visited[_currentPosition.X + 1, _currentPosition.Y])
                                    _lookDirection = LookDirection.Down;
                                break;
                        }
                    }

                    // Move the map
                    //Engine.Origin = NextMapPosition();
                }
                #endregion  
            }
        }

        public void Draw(GameTime gameTime)
        {
            if (_hasStarted && !AllVisited())
            {
                // Shadow
                if (_gameRef.GamePlayScreen.World.Levels[_gameRef.GamePlayScreen.World.CurrentLevel].
                    Map[_currentPosition.X, _currentPosition.Y] == null)
                {
                    _gameRef.SpriteBatch.Draw(_suddenDeathShadow,
                        new Rectangle(_currentPosition.X * Engine.TileWidth + (int)Engine.Origin.X, _currentPosition.Y * Engine.TileHeight + (int)Engine.Origin.Y,
                            Engine.TileWidth, Engine.TileHeight),
                        new Rectangle(0, 0, Engine.TileWidth, Engine.TileHeight), Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
                }

                // Previous wall
                _gameRef.SpriteBatch.Draw(_suddenDeathWall,
                    new Vector2(_previousPosition.X * Engine.TileWidth + Engine.Origin.X,
                        _previousPosition.Y * Engine.TileHeight + Engine.Origin.Y),
                    Color.White);

                // Wall
                _gameRef.SpriteBatch.Draw(_suddenDeathWall,
                    new Vector2(_currentPosition.X * Engine.TileWidth + Engine.Origin.X, 
                        (((float)_timer.Milliseconds / (float)_moveTime.Milliseconds) * (float)_currentPosition.Y) * Engine.TileHeight + Engine.Origin.Y), 
                    Color.White);
            }
        }
        #endregion

        #region Method Region

        #region Private Method Region
        private bool AllVisited()
        {
            for (int x = 1; x < _mapSize.X - 2; x++)
                for (int y = 1; y < _mapSize.Y - 2; y++)
                    if (!_visited[x, y])
                        return false;
            return true;
        }

        private Vector2 NextMapPosition()
        {
            return new Vector2(GamePlayScreen.Random.Next(_gameRef.GraphicsDevice.Viewport.Width - (_mapSize.X * Engine.TileWidth)),
                GamePlayScreen.Random.Next(_gameRef.GraphicsDevice.Viewport.Height - (_mapSize.Y * Engine.TileHeight))); 
        }
        #endregion

        #endregion
    }
}

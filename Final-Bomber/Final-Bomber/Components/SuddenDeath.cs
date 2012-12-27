using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Final_Bomber.TileEngine;
using Microsoft.Xna.Framework.Media;

namespace Final_Bomber.Components
{
    public class SuddenDeath
    {
        #region Field Region
        private FinalBomber gameRef;
        private Point mapSize;
        private bool hasStarted;
        private Texture2D suddenDeathShadow;
        private Texture2D suddenDeathWall;
        private Point currentPosition;
        private Point previousPosition;
        private TimeSpan timer;
        private TimeSpan moveTime;
        private LookDirection lookDirection;
        private bool[,] visited;

        // Map moving
        private Vector2 nextMapPosition;
        #endregion

        #region Propery Region

        public bool HasStarted
        {
            get { return hasStarted; }
        }

        public bool[,] Visited
        {
            get { return visited; }
        }
        #endregion

        #region Constructor Region
        public SuddenDeath(FinalBomber game, Point pos)
        {
            gameRef = game;
            hasStarted = false;
            suddenDeathShadow = gameRef.Content.Load<Texture2D>("Graphics/Characters/suddenDeathShadow");
            suddenDeathWall = gameRef.Content.Load<Texture2D>("Graphics/Characters/edgeWall");
            currentPosition = pos;
            previousPosition = Point.Zero;

            timer = TimeSpan.Zero;
            moveTime = TimeSpan.FromSeconds(Config.SuddenDeathWallSpeed);
            lookDirection = LookDirection.Right;

            Point mapSize = game.GamePlayScreen.World.Levels[game.GamePlayScreen.World.CurrentLevel].Size;

            visited = new bool[mapSize.X, mapSize.Y];
            for (int i = 0; i < mapSize.X; i++)
            {
                visited[i, 0] = true;
                visited[i, mapSize.Y - 1] = true;
            }
            for (int i = 0; i < mapSize.Y; i++)
            {
                visited[0, i] = true;
                visited[mapSize.X - 1, i] = true;
            }

            // Map moving
            nextMapPosition = Vector2.Zero;
        }
        #endregion

        #region XNA Method Region
        public void Update(GameTime gameTime)
        {
            timer += gameTime.ElapsedGameTime;

            if (!hasStarted && timer >= Config.SuddenDeathTimer)
            {
                MediaPlayer.Play(gameRef.GamePlayScreen.MapSongHurry);
                hasStarted = true;
                timer = TimeSpan.Zero;
            }

            if (hasStarted)
            {
                #region Walls
                // We change sudden death's wall position
                if (timer >= moveTime)
                {
                    if (!AllVisited())
                    {
                        visited[currentPosition.X, currentPosition.Y] = true;
                        MapItem mapItem = gameRef.GamePlayScreen.World.Levels[gameRef.GamePlayScreen.World.CurrentLevel].
                            Map[currentPosition.X, currentPosition.Y];
                        if (mapItem != null)
                        {
                            if (mapItem is Player)
                            {
                                mapItem.Destroy();
                                List<Player> pl = gameRef.GamePlayScreen.PlayerList.FindAll(p => p.Sprite.CellPosition == currentPosition);
                                foreach (Player p in pl)
                                    p.Destroy();
                            }
                            else if (mapItem is Teleporter || mapItem is Arrow || mapItem is Bomb)
                            {
                                List<Player> pl = gameRef.GamePlayScreen.PlayerList.FindAll(p => p.Sprite.CellPosition == currentPosition);
                                foreach(Player p in pl)
                                    p.Destroy();
                                List<Bomb> bl = gameRef.GamePlayScreen.BombList.FindAll(b => b.Sprite.CellPosition == currentPosition);
                                foreach(Bomb b in bl)
                                    b.Remove();
                                mapItem.Remove();
                            }
                            else
                                mapItem.Remove();
                        }

                        gameRef.GamePlayScreen.World.Levels[gameRef.GamePlayScreen.World.CurrentLevel].CollisionLayer[currentPosition.X, currentPosition.Y] = true;
                        UnbreakableWall u = new UnbreakableWall(gameRef, Engine.CellToVector(currentPosition));
                        gameRef.GamePlayScreen.World.Levels[gameRef.GamePlayScreen.World.CurrentLevel].Map[currentPosition.X, currentPosition.Y] = u;
                        gameRef.GamePlayScreen.UnbreakableWallList.Add(u);
                        timer = TimeSpan.Zero;
                        previousPosition = currentPosition;

                        switch (lookDirection)
                        {
                            case LookDirection.Up:
                                currentPosition.Y--;
                                if (!visited[currentPosition.X, currentPosition.Y] &&
                                    gameRef.GamePlayScreen.World.Levels[gameRef.GamePlayScreen.World.CurrentLevel].
                                    Map[currentPosition.X, currentPosition.Y] is UnbreakableWall)
                                {
                                    visited[currentPosition.X, currentPosition.Y] = true;
                                    if (visited[currentPosition.X, currentPosition.Y - 1])
                                        lookDirection = LookDirection.Right;
                                    else
                                        currentPosition.Y--;
                                }
                                if (visited[currentPosition.X, currentPosition.Y - 1])
                                    lookDirection = LookDirection.Right;
                                break;
                            case LookDirection.Down:
                                currentPosition.Y++;
                                if (!visited[currentPosition.X, currentPosition.Y] &&
                                    gameRef.GamePlayScreen.World.Levels[gameRef.GamePlayScreen.World.CurrentLevel].
                                    Map[currentPosition.X, currentPosition.Y] is UnbreakableWall)
                                {
                                    visited[currentPosition.X, currentPosition.Y] = true;
                                    if (visited[currentPosition.X, currentPosition.Y + 1])
                                        lookDirection = LookDirection.Left;
                                    else
                                        currentPosition.Y++;
                                }
                                if (visited[currentPosition.X, currentPosition.Y + 1])
                                    lookDirection = LookDirection.Left;
                                break;
                            case LookDirection.Left:
                                currentPosition.X--;
                                if (!visited[currentPosition.X, currentPosition.Y] &&
                                    gameRef.GamePlayScreen.World.Levels[gameRef.GamePlayScreen.World.CurrentLevel].
                                    Map[currentPosition.X, currentPosition.Y] is UnbreakableWall)
                                {
                                    visited[currentPosition.X, currentPosition.Y] = true;
                                    if (visited[currentPosition.X - 1, currentPosition.Y])
                                        lookDirection = LookDirection.Up;
                                    else
                                        currentPosition.X--;
                                }
                                if (visited[currentPosition.X - 1, currentPosition.Y])
                                    lookDirection = LookDirection.Up;
                                break;
                            case LookDirection.Right:
                                currentPosition.X++;
                                if (!visited[currentPosition.X, currentPosition.Y] &&
                                    gameRef.GamePlayScreen.World.Levels[gameRef.GamePlayScreen.World.CurrentLevel].
                                    Map[currentPosition.X, currentPosition.Y] is UnbreakableWall)
                                {
                                    visited[currentPosition.X, currentPosition.Y] = true;
                                    if (visited[currentPosition.X + 1, currentPosition.Y])
                                        lookDirection = LookDirection.Down;
                                    else
                                        currentPosition.X++;
                                }
                                if (visited[currentPosition.X + 1, currentPosition.Y])
                                    lookDirection = LookDirection.Down;
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
            if (hasStarted && !AllVisited())
            {
                // Shadow
                if (gameRef.GamePlayScreen.World.Levels[gameRef.GamePlayScreen.World.CurrentLevel].
                    Map[currentPosition.X, currentPosition.Y] == null)
                {
                    gameRef.SpriteBatch.Draw(suddenDeathShadow,
                        new Rectangle(currentPosition.X * Engine.TileWidth + (int)Engine.Origin.X, currentPosition.Y * Engine.TileHeight + (int)Engine.Origin.Y,
                            Engine.TileWidth, Engine.TileHeight),
                        new Rectangle(0, 0, Engine.TileWidth, Engine.TileHeight), Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
                }

                // Previous wall
                gameRef.SpriteBatch.Draw(suddenDeathWall,
                    new Vector2(previousPosition.X * Engine.TileWidth + Engine.Origin.X,
                        previousPosition.Y * Engine.TileHeight + Engine.Origin.Y),
                    Color.White);

                // Wall
                gameRef.SpriteBatch.Draw(suddenDeathWall,
                    new Vector2(currentPosition.X * Engine.TileWidth + Engine.Origin.X, 
                        (((float)timer.Milliseconds / (float)moveTime.Milliseconds) * (float)currentPosition.Y) * Engine.TileHeight + Engine.Origin.Y), 
                    Color.White);
            }
        }
        #endregion

        #region Method Region

        #region Private Method Region
        private bool AllVisited()
        {
            for (int x = 1; x < mapSize.X - 2; x++)
                for (int y = 1; y < mapSize.Y - 2; y++)
                    if (!visited[x, y])
                        return false;
            return true;
        }

        private Vector2 NextMapPosition()
        {
            return new Vector2(gameRef.GamePlayScreen.Random.Next(gameRef.GraphicsDevice.Viewport.Width - (mapSize.X * Engine.TileWidth)),
                gameRef.GamePlayScreen.Random.Next(gameRef.GraphicsDevice.Viewport.Height - (mapSize.Y * Engine.TileHeight))); 
        }
        #endregion

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FBLibrary.Core;
using Final_Bomber.Core.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Final_Bomber.TileEngine;
using Final_Bomber.Entities;
using System.Diagnostics;
using System.IO;

namespace Final_Bomber.WorldEngine
{
    public class Map
    {
        #region Field Region

        // Textures
        Texture2D _mapTexture;
        Texture2D _wallTexture;

        private TileMap _tileMap;

        private Point _size;
        private Entity[,] _board;
        private bool[,] _collisionLayer;

        private List<EdgeWall> _edgeWallList;
        private List<UnbreakableWall> _unbreakableWallList;
        private List<Teleporter> _teleporterList;
        private List<Arrow> _arrowList;

        #endregion

        #region Property Region

        public TileMap TileMap
        {
            get { return _tileMap; }
        }

        public Point Size
        {
            get { return _size; }
        }

        public Entity[,] Board
        {
            get { return _board; }
        }

        public bool[,] CollisionLayer
        {
            get { return _collisionLayer; }
        }

        #endregion

        #region Constructor Region

        public Map()
        {
            _edgeWallList = new List<EdgeWall>();
            _unbreakableWallList = new List<UnbreakableWall>();
            _teleporterList = new List<Teleporter>();
            _arrowList = new List<Arrow>();

            _wallTexture = FinalBomber.Instance.Content.Load<Texture2D>("Graphics/Characters/edgeWall");
            _mapTexture = FinalBomber.Instance.Content.Load<Texture2D>("Graphics/Tilesets/tileset1");
        }

        public Map(Point mSize, TileMap tMap, Entity[,] m, bool[,] cLayer)
            : this()
        {
            _size = mSize;
            _board = m;
            _tileMap = tMap;
            _collisionLayer = cLayer;
        }

        #endregion

        #region Method Region

        public void Update(GameTime gameTime)
        {
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
        {
            // Background
            var position = new Vector2(
                Engine.Origin.X - (int)(Engine.Origin.X / Engine.TileWidth) * Engine.TileWidth - Engine.TileWidth,
                Engine.Origin.Y - (int)(Engine.Origin.Y / Engine.TileHeight) * Engine.TileHeight - Engine.TileHeight);

            for (int i = 0; i < (FinalBomber.Instance.GraphicsDevice.Viewport.Width / Engine.TileWidth) + 2; i++)
            {
                for (int j = 0; j < (FinalBomber.Instance.GraphicsDevice.Viewport.Height / Engine.TileHeight) + 2; j++)
                {
                    if (!((position.X + i * Engine.TileWidth > Engine.Origin.X &&
                        position.X + i * Engine.TileWidth < Engine.Origin.X + Size.X * Engine.TileWidth - Engine.TileWidth) &&
                        (position.Y + j * Engine.TileHeight > Engine.Origin.Y &&
                        position.Y + j * Engine.TileHeight < Engine.Origin.Y + Size.Y * Engine.TileHeight - Engine.TileHeight)))
                    {
                        spriteBatch.Draw(_wallTexture, new Vector2(position.X + (i * Engine.TileWidth), position.Y + (j * Engine.TileHeight)), Color.White);
                    }
                }
            }

            _tileMap.Draw(spriteBatch, camera, _collisionLayer);

            // Draw entities
            foreach (var edgeWall in _edgeWallList)
                edgeWall.Draw(gameTime);

            foreach (var unbreakableWall in _unbreakableWallList)
                unbreakableWall.Draw(gameTime);

            foreach (var teleporter in _teleporterList)
                teleporter.Draw(gameTime);

            foreach (var arrow in _arrowList)
                arrow.Draw(gameTime);
        }

        public List<Point> FindEmptyCells()
        {
            var emptyCells = new List<Point>();
            for (int x = 0; x < Size.X; x++)
            {
                for (int y = 0; y < Size.Y; y++)
                {
                    if (!CollisionLayer[x, y])
                        emptyCells.Add(new Point(x, y));
                }
            }

            return emptyCells;
        }

        public void Parse(string file)
        {
            try
            {
                var streamReader = new StreamReader("Content/Maps/" + file);
                string line = streamReader.ReadLine();
                if (line != null)
                {
                    string[] lineSplit = line.Split(' ');
                    var parsedMapSize = new int[] { int.Parse(lineSplit[0]), int.Parse(lineSplit[1]) };

                    var mapSize = new Point(parsedMapSize[0], parsedMapSize[1]);
                    var tilesets = new List<Tileset>() { new Tileset(_mapTexture, 64, 32, 32, 32) };

                    var collisionLayer = new bool[mapSize.X, mapSize.Y];
                    var mapPlayersPosition = new int[mapSize.X, mapSize.Y];
                    var board = new Entity[mapSize.X, mapSize.Y];
                    var layer = new MapLayer(mapSize.X, mapSize.Y);
                    var voidPosition = new List<Point>();
                    var playerPositions = new Dictionary<int, Point>();

                    Point currentPosition = Point.Zero;
                    int j = 0;
                    while (!streamReader.EndOfStream)
                    {
                        line = streamReader.ReadLine();
                        Debug.Assert(line != null, "line != null");
                        lineSplit = line.Split(' ');
                        int playerNumber = 0;
                        currentPosition.Y = j;
                        for (int i = 0; i < lineSplit.Length; i++)
                        {
                            int id = int.Parse(lineSplit[i]);

                            currentPosition.X = i;
                        
                            switch (id)
                            {
                                case (int)EntityType.UnbreakableWall:
                                    var unbreakableWall = new UnbreakableWall(currentPosition);
                                    board[i, j] = unbreakableWall;
                                    _unbreakableWallList.Add(unbreakableWall);
                                    collisionLayer[i, j] = true;
                                    break;
                                case (int)EntityType.EdgeWall:
                                    var edgeWall = new EdgeWall(currentPosition);
                                    board[i, j] = edgeWall;
                                    _edgeWallList.Add(edgeWall);
                                    collisionLayer[i, j] = true;
                                    break;
                                /*
                                case (int)Entity.Type.Wall:
                                    var wall = new Wall(currentPosition);
                                    _wallList.Add(wall);
                                    board[i, j] = wall;
                                    collisionLayer[i, j] = true;
                                    break;
                                */
                                case (int)EntityType.Teleporter:
                                    var teleporter = new Teleporter(currentPosition);
                                    board[i, j] = teleporter;
                                    _teleporterList.Add(teleporter);
                                    break;
                                case (int)EntityType.Arrow:
                                    var arrow = new Arrow(currentPosition, LookDirection.Down);
                                    _arrowList.Add(arrow);
                                    board[i, j] = arrow;
                                    break;
                                case (int)EntityType.Player:
                                    if (playerNumber <= Config.PlayersNumber)
                                    {
                                        playerPositions[playerNumber] = currentPosition;
                                        playerNumber++;
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }

                        j++;
                    }

                    var mapLayers = new List<MapLayer> { layer };

                    var tileMap = new TileMap(tilesets, mapLayers);

                    _size = mapSize;
                    _board = board;
                    _tileMap = tileMap;
                    _collisionLayer = collisionLayer;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        #endregion
    }
}

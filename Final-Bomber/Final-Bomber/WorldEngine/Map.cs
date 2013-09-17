using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        readonly TileMap tileMap;

        private Point size;
        private Entity[,] board;
        private bool[,] collisionLayer;
        private int[,] hazardMap;

        private List<Wall> _wallList;
        private List<PowerUp> powerUpList;
        private List<EdgeWall> _edgeWallList;
        private List<Bomb> _bombList;
        private List<UnbreakableWall> _unbreakableWallList;
        private List<Teleporter> _teleporterList;
        private List<Arrow> _arrowList;

        #endregion

        #region Property Region

        public TileMap TileMap
        {
            get { return tileMap; }
        }

        public Point Size
        {
            get { return size; }
        }

        public Entity[,] Board
        {
            get { return board; }
        }

        public bool[,] CollisionLayer
        {
            get { return collisionLayer; }
        }

        public int[,] HazardMap
        {
            get { return hazardMap; }
        }

        #endregion

        #region Constructor Region

        public Map(Point mSize, TileMap tMap, Entity[,] m, bool[,] cLayer)
        {
            size = mSize;
            this.board = m;
            this.tileMap = tMap;
            this.collisionLayer = cLayer;
            this.hazardMap = new int[mSize.X, mSize.Y];

            _wallList = new List<Wall>();
            powerUpList = new List<PowerUp>();
            _edgeWallList = new List<EdgeWall>();
            _bombList = new List<Bomb>();
            _unbreakableWallList = new List<UnbreakableWall>();
            _teleporterList = new List<Teleporter>();
            _arrowList = new List<Arrow>();
        }

        #endregion

        #region Method Region

        public void Update(GameTime gameTime)
        {
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
        {
            tileMap.Draw(spriteBatch, camera, collisionLayer);
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

        private void Parse(string file, Texture2D mapTexture)
        {
            try
            {
                var streamReader = new StreamReader("Content/Maps/" + file);
                string line = streamReader.ReadLine();
                string[] lineSplit = line.Split(' ');
                var parsedMapSize = new int[] { int.Parse(lineSplit[0]), int.Parse(lineSplit[1]) };

                var mapSize = new Point(parsedMapSize[0], parsedMapSize[1]);
                var tilesets = new List<Tileset>() { new Tileset(mapTexture, 64, 32, 32, 32) };

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
                            case (int)Entity.Type.UnbreakableWall:
                                var unbreakableWall = new UnbreakableWall(currentPosition);
                                board[i, j] = unbreakableWall;
                                _unbreakableWallList.Add(unbreakableWall);
                                collisionLayer[i, j] = true;
                                break;
                            case (int)Entity.Type.EdgeWall:
                                var edgeWall = new EdgeWall(currentPosition);
                                board[i, j] = edgeWall;
                                _edgeWallList.Add(edgeWall);
                                collisionLayer[i, j] = true;
                                break;
                            case (int)Entity.Type.Wall:
                                var wall = new Wall(currentPosition);
                                _wallList.Add(wall);
                                board[i, j] = wall;
                                collisionLayer[i, j] = true;
                                break;
                            case (int)Entity.Type.Teleporter:
                                var teleporter = new Teleporter(currentPosition);
                                board[i, j] = teleporter;
                                _teleporterList.Add(teleporter);
                                break;
                            case (int)Entity.Type.Arrow:
                                var arrow = new Arrow(currentPosition, LookDirection.Down);
                                _arrowList.Add(arrow);
                                board[i, j] = arrow;
                                break;
                            case (int)Entity.Type.Player:
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
                var level = new Map(mapSize, tileMap, board, collisionLayer);
                /*
                World = new World(GameRef, GameRef.ScreenRectangle);
                World.Levels.Add(level);
                World.CurrentLevel = 0;

                foreach (int playerID in playerPositions.Keys)
                {
                    if (Config.AIPlayers[playerID])
                    {
                        var player = new AIPlayer(Math.Abs(playerID));
                        PlayerList.Add(player);
                        board[playerPositions[playerID].X, playerPositions[playerID].Y] = player;
                    }
                    else
                    {
                        var player = new HumanPlayer(Math.Abs(playerID));
                        PlayerList.Add(player);
                        board[playerPositions[playerID].X, playerPositions[playerID].Y] = player;
                    }
                }
                */
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        #endregion
    }
}

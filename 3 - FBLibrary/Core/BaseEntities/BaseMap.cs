using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Microsoft.Xna.Framework;

namespace FBLibrary.Core.BaseEntities
{
    public abstract class BaseMap
    {
        #region Field Region

        public string Name;
        public bool Loaded;
        public Point Size;
        public IEntity[,] Board;
        public bool[,] CollisionLayer;
        public readonly List<Point> PlayerSpawnPoints;
        public int PlayerNumber;

        private string _md5;
        private readonly List<BaseEdgeWall> _baseEdgeWallList;
        private readonly List<BaseUnbreakableWall> _baseUnbreakableWallList;

        #endregion

        #region Constructor Region

        protected BaseMap()
        {
            PlayerSpawnPoints = new List<Point>();
            Loaded = false;
            PlayerNumber = 0;

            _baseEdgeWallList = new List<BaseEdgeWall>();
            _baseUnbreakableWallList = new List<BaseUnbreakableWall>();
        }

        #endregion

        #region Method Region

        public void Reset()
        {
            if (Loaded)
            {
                for (int x = 0; x < Size.X; x++)
                {
                    for (int y = 0; y < Size.Y; y++)
                    {
                        if (Board[x, y] is BaseEdgeWall ||
                            Board[x, y] is BaseUnbreakableWall)
                        {
                            continue;
                        }

                        Board[x, y] = null;
                        CollisionLayer[x, y] = false;
                    }
                }
            }
        }

        public void Parse(string file, BaseGameManager gameManager)
        {
            Name = file;

            try
            {
                var streamReader = new StreamReader("Content/Maps/" + file);
                string line = streamReader.ReadLine();
                if (line != null)
                {
                    string[] lineSplit = line.Split(' ');
                    var parsedMapSize = new int[] { int.Parse(lineSplit[0]), int.Parse(lineSplit[1]) };

                    Size = new Point(parsedMapSize[0], parsedMapSize[1]);
                    CollisionLayer = new bool[Size.X, Size.Y];
                    Board = new IEntity[Size.X, Size.Y];

                    var mapPlayersPosition = new int[Size.X, Size.Y];
                    var playerPositions = new Dictionary<int, Point>();

                    Point currentPosition = Point.Zero;
                    int j = 0;
                    while (!streamReader.EndOfStream)
                    {
                        line = streamReader.ReadLine();

                        if (line == null)
                        {
                            // TODO: better error management for the lib
                            //Program.Log.Error("Map parsing: line == null");
                            break;
                        }

                        lineSplit = line.Split(' ');
                        currentPosition.Y = j;
                        for (int i = 0; i < lineSplit.Length; i++)
                        {
                            int id = int.Parse(lineSplit[i]);

                            currentPosition.X = i;

                            switch (id)
                            {
                                case (int)EntityType.Void:
                                    break;
                                case (int)EntityType.UnbreakableWall:
                                    AddUnbreakableWall(currentPosition);
                                    break;
                                case (int)EntityType.EdgeWall:
                                    AddEdgeWall(currentPosition);
                                    break;
                                case (int)EntityType.Wall:
                                    gameManager.AddWall(currentPosition);
                                    /*
                                    var wall = new BaseWall(currentPosition);
                                    gameManager.WallList.Add(wall);
                                    board[i, j] = wall;
                                    collisionLayer[i, j] = true;*/
                                    break;
                                /*
                            case (int)EntityType.Teleporter:
                                var teleporter = new BaseTeleporter(currentPosition);
                                board[i, j] = teleporter;
                                _teleporterList.Add(teleporter);
                                break;
                            case (int)EntityType.Arrow:
                                var arrow = new BaseArrow(currentPosition, LookDirection.Down);
                                _arrowList.Add(arrow);
                                board[i, j] = arrow;
                                break;
                                */
                                case (int)EntityType.Player:
                                    PlayerSpawnPoints.Add(currentPosition);
                                    PlayerNumber++;
                                    break;
                                default:
                                    break;
                            }
                        }

                        j++;
                    }

                    Loaded = true;
                }
            }
            catch (Exception ex)
            {
                throw;
                //Program.Log.Error(ex.Message);
            }
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

        public string GetMd5()
        {
            // If we already have computed the md5 hash
            if (_md5 != null)
                return _md5;

            if (Name != null)
            {
                string path = "Content/Maps/" + Name;

                if (File.Exists(path))
                {
                    using (var md5 = MD5.Create())
                    {
                        using (var stream = File.OpenRead(path))
                        {
                            // Compute the MD5 hash of the file
                            _md5 = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
                            return _md5;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("This map doesn't exist ! (" + path + ")");
                }
            }

            return null;
        }

        #endregion

        protected abstract void AddUnbreakableWall(Point position);
        protected virtual void AddUnbreakableWall(BaseUnbreakableWall unbreakableWall)
        {
            CollisionLayer[unbreakableWall.CellPositionX, unbreakableWall.CellPositionY] = true;
            Board[unbreakableWall.CellPositionX, unbreakableWall.CellPositionY] = unbreakableWall;

            _baseUnbreakableWallList.Add(unbreakableWall);
        }

        protected abstract void AddEdgeWall(Point position);
        protected virtual void AddEdgeWall(BaseEdgeWall edgeWall)
        {
            CollisionLayer[edgeWall.CellPositionX, edgeWall.CellPositionY] = true;
            Board[edgeWall.CellPositionX, edgeWall.CellPositionY] = edgeWall;

            _baseEdgeWallList.Add(edgeWall);
        }

        #region Displaying region
        public void DisplayBoard()
        {
            for (int y = 0; y < Size.Y; y++)
            {
                for (int x = 0; x < Size.X; x++)
                {
                    if (Board[x, y] is BaseEdgeWall)
                    {
                        Console.Write("E ");
                    }
                    else if (Board[x, y] is BaseUnbreakableWall)
                    {
                        Console.Write("U ");
                    }
                    else if (Board[x, y] is BaseWall)
                    {
                        Console.Write("W ");
                    }
                    else if (Board[x, y] is BasePlayer)
                    {
                        Console.Write("P ");
                    }
                    else
                    {
                        Console.Write("N ");
                    }
                }

                Console.WriteLine();
            }
        }

        public void DisplayCollisionLayer()
        {
            for (int y = 0; y < Size.Y; y++)
            {
                for (int x = 0; x < Size.X; x++)
                {
                    Console.Write(CollisionLayer[x, y] ? "1 " : "0 ");
                }

                Console.WriteLine();
            }
        }
        #endregion
    }
}

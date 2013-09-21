using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using FBLibrary.Core;
using FBLibrary.Core.BaseEntities;
using Final_BomberServer.Core.Entities;
using Microsoft.Xna.Framework;

namespace Final_BomberServer.Core.WorldEngine
{
    public class Map : BaseMap
    {
        private List<EdgeWall> _edgeWallList;
        private List<UnbreakableWall> _unbreakableWallList;
        private List<Teleporter> _teleporterList;
        private List<Arrow> _arrowList;

        public Map()
        {
            _edgeWallList = new List<EdgeWall>();
            _unbreakableWallList = new List<UnbreakableWall>();
            _teleporterList = new List<Teleporter>();
            _arrowList = new List<Arrow>();
        }

        public void Parse(string file, GameManager gameManager)
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

                    var collisionLayer = new bool[mapSize.X, mapSize.Y];
                    var mapPlayersPosition = new int[mapSize.X, mapSize.Y];
                    var board = new IEntity[mapSize.X, mapSize.Y];
                    var voidPosition = new List<Point>();
                    var playerPositions = new Dictionary<int, Point>();

                    Point currentPosition = Point.Zero;
                    int j = 0;
                    while (!streamReader.EndOfStream)
                    {
                        line = streamReader.ReadLine();

                        if (line == null)
                        {
                            Program.Log.Error("Map parsing: line == null");
                            break;
                        }

                        lineSplit = line.Split(' ');
                        int playerNumber = 0;
                        currentPosition.Y = j;
                        for (int i = 0; i < lineSplit.Length; i++)
                        {
                            int id = int.Parse(lineSplit[i]);

                            currentPosition.X = i;

                            switch (id)
                            {
                                case (int)EntityType.Void:
                                    // Do we put a Wall ?
                                    if (board[i, j] == null &&
                                        gameManager.Random.Next(0, 100) < GameConfiguration.WallPercentage)
                                    {
                                        var wall = new Wall(currentPosition);
                                        gameManager.WallList.Add(wall);
                                        board[i, j] = wall;
                                        collisionLayer[i, j] = true;
                                    }
                                    break;
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
                                    if (playerNumber <= 5)  // TODO: load max player from a map file
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

                    Size = mapSize;
                    Board = board;
                    CollisionLayer = collisionLayer;
                }
            }
            catch (Exception ex)
            {
                Program.Log.Error(ex.Message);
            }
        }

        #region Displaying region
        public void DisplayBoard()
        {
            for (int x = 0; x < Size.X; x++)
            {
                for (int y = 0; y < Size.Y; y++)
                {
                    if (Board[x, y] is EdgeWall)
                    {
                        Console.Write("E ");
                    }
                    else if (Board[x, y] is UnbreakableWall)
                    {
                        Console.Write("U ");
                    }
                    else if (Board[x, y] is Wall)
                    {
                        Console.Write("W ");
                    }
                    else if (Board[x, y] is Player)
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
            for (int x = 0; x < Size.X; x++)
            {
                for (int y = 0; y < Size.Y; y++)
                {
                    Console.Write(CollisionLayer[x, y] ? "1 " : "0 ");
                }

                Console.WriteLine();
            }
        }
        #endregion
    }
}

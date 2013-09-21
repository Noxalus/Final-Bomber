﻿using System;
using System.Collections.Generic;
using FBLibrary.Core;
using FBLibrary.Core.BaseEntities;
using Final_Bomber.Core;
using Final_Bomber.Core.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Final_Bomber.TileEngine;
using System.Diagnostics;
using System.IO;

namespace Final_Bomber.WorldEngine
{
    public class Map : BaseMap
    {
        #region Field Region

        // Textures
        Texture2D _mapTexture;
        Texture2D _wallTexture;

        private TileMap _tileMap;

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

        public List<Teleporter> TeleporterList
        {
            get { return _teleporterList; }
            set { _teleporterList = value; }
        }

        #endregion

        #region Constructor Region

        public Map()
        {
            _edgeWallList = new List<EdgeWall>();
            _unbreakableWallList = new List<UnbreakableWall>();
            TeleporterList = new List<Teleporter>();
            _arrowList = new List<Arrow>();

            _wallTexture = FinalBomber.Instance.Content.Load<Texture2D>("Graphics/Characters/edgeWall");
            _mapTexture = FinalBomber.Instance.Content.Load<Texture2D>("Graphics/Tilesets/tileset1");
        }

        public Map(Point mSize, TileMap tMap, IEntity[,] m, bool[,] cLayer)
            : this()
        {
            Size = mSize;
            Board = m;
            _tileMap = tMap;
            CollisionLayer = cLayer;
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

            _tileMap.Draw(spriteBatch, camera, CollisionLayer);

            // Draw entities
            foreach (var edgeWall in _edgeWallList)
                edgeWall.Draw(gameTime);

            foreach (var unbreakableWall in _unbreakableWallList)
                unbreakableWall.Draw(gameTime);

            foreach (var teleporter in TeleporterList)
                teleporter.Draw(gameTime);

            foreach (var arrow in _arrowList)
                arrow.Draw(gameTime);
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
                    var tilesets = new List<Tileset>() { new Tileset(_mapTexture, 64, 32, 32, 32) };

                    var collisionLayer = new bool[mapSize.X, mapSize.Y];
                    var mapPlayersPosition = new int[mapSize.X, mapSize.Y];
                    var board = new IEntity[mapSize.X, mapSize.Y];
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
                                case (int)EntityType.Void:
                                    // Do we put a Wall ?
                                    if (board[i, j] == null &&
                                        GameManager.Random.Next(0, 100) < GameConfiguration.WallPercentage)
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
                                    TeleporterList.Add(teleporter);
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

                    Size = mapSize;
                    Board = board;
                    _tileMap = tileMap;
                    CollisionLayer = collisionLayer;
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Final_Bomber.TileEngine;
using Final_Bomber.Components;

namespace Final_Bomber.WorldEngine
{
    public class Level
    {
        #region Field Region

        readonly TileMap tileMap;

        private Point size;
        private Entity[,] map;
        private bool[,] collisionLayer;
        private int[,] hazardMap;

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

        public Entity[,] Map
        {
            get { return map; }
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

        public Level(Point mSize, TileMap tMap, Entity[,] m, bool[,] cLayer)
        {
            size = mSize;
            this.map = m;
            this.tileMap = tMap;
            this.collisionLayer = cLayer;
            this.hazardMap = new int[mSize.X, mSize.Y];
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

        #endregion
    }
}

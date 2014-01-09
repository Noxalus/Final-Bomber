using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FBLibrary.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FBClient.WorldEngine;

namespace FBClient.TileEngine
{
    public class MapLayer
    {
        #region Field Region

        Tile[,] layer;

        #endregion

        #region Property Region

        public int Width
        {
            get { return layer.GetLength(1); }
        }

        public int Height
        {
            get { return layer.GetLength(0); }
        }

        #endregion

        #region Constructor Region

        public MapLayer(Tile[,] map)
        {
            this.layer = (Tile[,])map.Clone();
        }

        public MapLayer(int width, int height)
        {
            layer = new Tile[height, width];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    layer[y, x] = new Tile(0, 0);
                }
            }
        }

        #endregion

        #region Method Region

        public Tile GetTile(int x, int y)
        {
            return layer[y, x];
        }

        public void SetTile(int x, int y, Tile tile)
        {
            layer[y, x] = tile;
        }

        public void SetTile(int x, int y, int tileIndex, int tileset)
        {
            layer[y, x] = new Tile(tileIndex, tileset);
        }

        public void Draw(SpriteBatch spriteBatch, Camera2D camera, List<Tileset> tilesets, bool[,] collisionLayer)
        {
            Vector2 cameraPosition = camera.Position - new Vector2(camera.ViewportRectangle.Width / 2f, camera.ViewportRectangle.Height / 2f);
            Point cameraPoint = Engine.VectorToCell(cameraPosition * (1 / camera.Zoom));
            Point viewPoint = Engine.VectorToCell(
                new Vector2(
                    (cameraPosition.X + camera.ViewportRectangle.Width) * (1 / camera.Zoom),
                    (cameraPosition.Y + camera.ViewportRectangle.Height) * (1 / camera.Zoom)));

            var min = new Point();
            var max = new Point();

            min.X = Math.Max(0, cameraPoint.X - 1);
            min.Y = Math.Max(0, cameraPoint.Y - 1);
            max.X = Math.Min(viewPoint.X + 1, Width);
            max.Y = Math.Min(viewPoint.Y + 1, Height);

            var destination = new Rectangle(0, 0, Engine.TileWidth, Engine.TileHeight);

            for (int y = min.Y; y < max.Y; y++)
            {
                destination.Y = y * Engine.TileHeight + (int)Engine.Origin.Y;

                for (int x = min.X; x < max.X; x++)
                {
                    Tile tile = GetTile(x, y);

                    if (tile.TileIndex == -1 || tile.Tileset == -1)
                        continue;

                    destination.X = x * Engine.TileWidth + (int)Engine.Origin.X;

                    if (collisionLayer[x, y])
                    {
                        spriteBatch.Draw(
                            tilesets[tile.Tileset].Texture,
                            destination,
                            tilesets[tile.Tileset].SourceRectangles[tile.TileIndex],
                            Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(
                            tilesets[tile.Tileset].Texture,
                            destination,
                            tilesets[tile.Tileset].SourceRectangles[tile.TileIndex],
                            Color.White);
                    }
                }
            }
        }

        /*
        public static MapLayer FromMapLayerData(MapLayerData data)
        {
            MapLayer layer = new MapLayer(data.Width, data.Height);

            for (int y = 0; y < data.Height; y++)
                for (int x = 0; x < data.Width; x++)
                {
                    layer.SetTile(
                        x,
                        y,
                        data.GetTile(x, y).TileIndex,
                        data.GetTile(x, y).TileSetIndex);
                }

            return layer;
        }
        */
        #endregion
    }
}

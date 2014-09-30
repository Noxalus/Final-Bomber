using System;
using System.Collections.Generic;
using FBLibrary.Core;
using Microsoft.Xna.Framework;
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

        public void Draw(Camera2D camera, List<Tileset> tilesets)
        {
            var destination = new Rectangle(0, 0, Engine.TileWidth, Engine.TileHeight);

            // TODO: Replace this drawcall with a single drawcall to render all grass tiles
            for (int y = 0; y < Height; y++)
            {
                destination.Y = y * Engine.TileHeight + (int)Engine.Origin.Y;

                for (int x = 0; x < Width; x++)
                {
                    Tile tile = GetTile(x, y);

                    if (tile.TileIndex == -1 || tile.Tileset == -1)
                        continue;

                    destination.X = x * Engine.TileWidth + (int)Engine.Origin.X;

                    var position = new Vector2(destination.X, destination.Y);
                    if (camera.IsVisible(position))
                    {
                        FinalBomber.Instance.SpriteBatch.Draw(
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

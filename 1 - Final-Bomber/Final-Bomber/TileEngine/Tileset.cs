using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FBClient.TileEngine
{
    public class Tileset
    {
        #region Fields and Properties

        Rectangle[] sourceRectangles;

        #endregion

        #region Property Region

        public Texture2D Texture { get; private set; }

        public int TileWidth { get; private set; }

        public int TileHeight { get; private set; }

        public int TilesWide { get; private set; }

        public int TilesHigh { get; private set; }

        public Rectangle[] SourceRectangles
        {
            get { return (Rectangle[])sourceRectangles.Clone(); }
        }

        #endregion

        #region Constructor Region

        public Tileset(Texture2D image, int tilesWide, int tilesHigh, int tileWidth, int tileHeight)
        {
            Texture = image;
            TileWidth = tileWidth;
            TileHeight = tileHeight;
            TilesWide = tilesWide;
            TilesHigh = tilesHigh;

            int tiles = tilesWide * tilesHigh;

            sourceRectangles = new Rectangle[tiles];

            int tile = 0;

            for (int y = 0; y < tilesHigh; y++)
                for (int x = 0; x < tilesWide; x++)
                {
                    sourceRectangles[tile] = new Rectangle(
                        x * tileWidth,
                        y * tileHeight,
                        tileWidth,
                        tileHeight);
                    tile++;
                }
        }

        #endregion

        #region Method Region
        #endregion
    }
}

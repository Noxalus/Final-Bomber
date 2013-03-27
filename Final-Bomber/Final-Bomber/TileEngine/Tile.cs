using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Final_Bomber.TileEngine
{
    public class Tile
    {
        #region Field Region

        #endregion

        #region Property Region

        public int TileIndex { get; private set; }

        public int Tileset { get; private set; }

        #endregion

        #region Constructor Region

        public Tile(int tileIndex, int tileset)
        {
            TileIndex = tileIndex;
            Tileset = tileset;
        }

        #endregion
    }
}

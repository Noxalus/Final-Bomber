using Final_BomberServer.Core.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final_BomberServer.Core
{
    public class MapTile
    {
        public MapTile(int xPos, int yPos)
        {
            tilePosition = new Vector2(xPos, yPos);
        }

        public const int SIZE = 50;
        public const string DIR = "Tiles\\";
        public bool walkable = true;
        public OldBomb Bombed = null;
        public PowerUp Poweruped = null;
        public bool stopExplosion = false; //Om den stoppar explosionen eller om denlåter explosionen gå igenom
        public bool canExplode = false; //Om den kan explodera, så blir tilen en floor piece

        MapTiles currentTile = MapTiles.None;
        public MapTiles CurrentTile
        {
            get { return currentTile; }
            set { currentTile = value; }
        }

        Vector2 tilePosition;
        public Vector2 TilePosition
        {
            get { return tilePosition; }
            set { tilePosition = value; }
        }

        public Vector2 GetMapPos()
        {
            return tilePosition * SIZE;
        }

        public int GetListPos()
        {
            if (GameSettings.GetCurrentMap() != null)
            {
                return (int)tilePosition.Y * GameSettings.GetCurrentMap().mapWidth + (int)tilePosition.X;
            }
            else
            {
                return -1;
            }
        }

        public void SetSettings(bool walkable, bool stopExplosion, bool canExplode)
        {
            this.walkable = walkable;
            this.stopExplosion = stopExplosion;
            this.canExplode = canExplode;
        }

        public MapTile Clone()
        {
            MapTile rtn = new MapTile((int)tilePosition.X, (int)tilePosition.Y);
            rtn.walkable = walkable;
            rtn.stopExplosion = stopExplosion;
            rtn.canExplode = canExplode;
            rtn.currentTile = currentTile;
            return rtn;
        }

        public enum MapTiles
        {
            BlueBlock,
            BrownBlock,
            GrayFloor,
            Water,
            Poison,
            None,
            StartFloor,
            //Måste alltid lägga till nya under detta, så att alla maps funkar fastän det kommer en ny version av spelet
        }
    }
}

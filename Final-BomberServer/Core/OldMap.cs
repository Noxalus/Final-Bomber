using System;
using System.Collections.Generic;
using FBLibrary.Core;
using Final_BomberServer.Core.Entities;

namespace Final_BomberServer.Core
{
    public class OldMap
    {
        public OldMap(string path)
        {
            this.path = path;
            mapName = GetMapName(path);
        }

        public int mapWidth, mapHeight;
        public string mapName;
        public Int64 id;
        public string path;
        List<MapTile> originalMapTiles; //De nya tiles ändras när de sprängs, så den kommer ihåg hur banan såg ut genom dessa orginal tiles
        List<MapTile> mapTiles;
        public List<MapTile> startPositions = new List<MapTile>();
        public int playerAmount; //Antalet spelare som kan spela
        public List<byte> mapData = new List<byte>();
        Random rnd = new Random(Environment.TickCount);
        public int suddenDeathTime;

        public bool Loaded = false;

        public List<MapTile> GetMapTiles
        {
            get { return mapTiles; }
        }

        public void Load()
        {
            originalMapTiles = new List<MapTile>();
            if (Loaded)
                throw new Exception("Already loaded map!");
            /*
            db_FileIO db = new db_FileIO();
            db.OpenToRead(path);
            id = db.ReadInt64();
            mapWidth = db.ReadByte();
            mapHeight = db.ReadByte();
            for (int i = 0; i < mapHeight; i++)
            {
                for (int j = 0; j < mapWidth; j++)
                {
                    CreateTile(db.ReadByte(), j, i);
                }
            }
            db.Close();
            
            #region LoadData
            db.OpenToRead(path);
            while (!db.FileEnd())
            {
                mapData.Add(db.ReadByte());
            }
            db.Close();
            #endregion
            */
            playerAmount = startPositions.Count; //Räknar ut hur många spelare som kan köra max
            suddenDeathTime = (mapWidth * 3 + mapHeight * 3) * 2 * 1000; //Räknar ut timern till suddenDeath
            Loaded = true;
        }

        public void CreateMap()
        {
            //Skapar banan utifrån det den har loadad
            mapTiles = new List<MapTile>();
            foreach (MapTile tile in originalMapTiles)
            {
                mapTiles.Add(tile.Clone());
            }
            rnd = new Random(Environment.TickCount); //Ser till så att den ändras
        }

        public MapTile GetTileByPosition(float x, float y)
        {
            y /= MapTile.SIZE;
            x /= MapTile.SIZE;
            int ix = (int)Math.Floor(x);
            int iy = (int)Math.Floor(y);
            if (!(ix < 0 || ix > mapWidth - 1 || iy < 0 || iy > mapHeight - 1))
            {
                return mapTiles[iy * mapWidth + ix];
            }
            else
            {
                return null;
            }
        }

        public MapTile GetTileByTilePosition(int x, int y)
        {
            if (!(x < 0 || x > mapWidth - 1 || y < 0 || y > mapHeight - 1))
            {
                return mapTiles[y * mapWidth + x];
            }
            return null;
        }

        public MapTile GetTileByPlayerDirection(Player player, LookDirection direction)
        {
            MapTile nextTile = null;
            switch (direction)
            {
                case LookDirection.Down:
                    nextTile = GetTileByTilePosition((int)player.NextTile.TilePosition.X, (int)player.NextTile.TilePosition.Y + 1);
                    break;
                case LookDirection.Left:
                    nextTile = GetTileByTilePosition((int)player.NextTile.TilePosition.X - 1, (int)player.NextTile.TilePosition.Y);
                    break;
                case LookDirection.Right:
                    nextTile = GetTileByTilePosition((int)player.NextTile.TilePosition.X + 1, (int)player.NextTile.TilePosition.Y);
                    break;
                case LookDirection.Up:
                    nextTile = GetTileByTilePosition((int)player.NextTile.TilePosition.X, (int)player.NextTile.TilePosition.Y - 1);
                    break;
                case LookDirection.Idle:
                    nextTile = player.NextTile;
                    break;

            }
            if (nextTile != null)
            {
                if (nextTile.walkable == true)
                {
                    if (nextTile.Bombed != null)
                    {
                        if (nextTile.Bombed.player.Id != player.Id)
                            return null;
                    }
                    return nextTile;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public List<MapTile> GetTilesCollisionWithPlayer(Player player)
        {
            List<MapTile> rtn = new List<MapTile>();
            MapTile tile = null;
            for (int i = 0; i < 4; i++)
            {
                switch (i)
                {
                    case 0:
                        tile = GetTileByPosition((player.Position.X + player.SizeOffset.X),
                            (player.Position.Y + player.SizeOffset.Y));
                        break;
                    case 1:
                        tile = GetTileByPosition((player.Position.X + player.SizeOffset.X) + player.Dimension.X,
                            (player.Position.Y + player.SizeOffset.Y));
                        break;
                    case 2:
                        tile = GetTileByPosition((player.Position.X + player.SizeOffset.X),
                            (player.Position.Y + player.SizeOffset.Y) + player.Dimension.Y);
                        break;
                    case 3:
                        tile = GetTileByPosition((player.Position.X + player.SizeOffset.X) + player.Dimension.X,
                            (player.Position.Y + player.SizeOffset.Y) + player.Dimension.Y);
                        break;
                }
                rtn.Add(tile);
            }
            return rtn;
        }

        private string GetMapName(string path)
        {
            for (int i = path.Length - 3; i >= 0; i--)
            {
                if (path[i] == '\\')
                {
                    return path.Substring(i + 1, path.Length - i - 5);
                }
            }
            return path;
        }

        private void CreateTile(byte tile, int x, int y)
        {
            MapTile maptile = new MapTile(x, y);
            switch (tile)
            {
                //Block
                case (byte)MapTile.MapTiles.BlueBlock:
                    maptile.CurrentTile = MapTile.MapTiles.BlueBlock;
                    maptile.SetSettings(false, true, false);

                    break;

                case (byte)MapTile.MapTiles.BrownBlock:
                    maptile.CurrentTile = MapTile.MapTiles.BrownBlock;
                    maptile.SetSettings(false, true, true);
                    break;

                //Floor
                case (byte)MapTile.MapTiles.GrayFloor:
                    maptile.CurrentTile = MapTile.MapTiles.GrayFloor;
                    maptile.SetSettings(true, false, false);
                    break;

                case (byte)MapTile.MapTiles.StartFloor:
                    maptile.CurrentTile = MapTile.MapTiles.GrayFloor;
                    startPositions.Add(maptile);
                    maptile.SetSettings(true, false, false);
                    break;

                //Liquid
                case (byte)MapTile.MapTiles.Water:
                    maptile.CurrentTile = MapTile.MapTiles.Water;
                    maptile.SetSettings(false, false, false);
                    break;

                case (byte)MapTile.MapTiles.Poison:
                    maptile.CurrentTile = MapTile.MapTiles.Poison;
                    maptile.SetSettings(false, false, false);
                    break;

                default:
                    maptile.CurrentTile = MapTile.MapTiles.None;
                    maptile.SetSettings(false, false, false);
                    break;
            }
            originalMapTiles.Add(maptile);
        }

        private void ExplodeTile(MapTile tile)
        {
            tile.canExplode = false;
            int x = (int)tile.TilePosition.X;
            int y = (int)tile.TilePosition.Y;
            int pos = y * mapWidth + x;
            mapTiles[pos] = new MapTile(x, y);
            mapTiles[pos].CurrentTile = MapTile.MapTiles.GrayFloor;
            mapTiles[pos].SetSettings(true, false, false);
            GameSettings.gameServer.SendExplodeTile(pos);
            CreatePowerup(mapTiles[pos]);
        }

        private void CreatePowerup(MapTile tile)
        {
            if (tile.Poweruped == null)
            {
                int drop = rnd.Next(100) + 1;
                if (drop <= GameSettings.GameValues.PowerupDrop.Powerups)
                {
                    int type = rnd.Next(100) + 1;
                    if (type <= GameSettings.GameValues.PowerupDrop.MovementSpeed)
                    {
                        tile.Poweruped = new PowerUp(PowerUpType.Speed);
                    }
                    else
                    {
                        if (type <= GameSettings.GameValues.PowerupDrop.Tickrate)
                        {
                            //tile.Poweruped = new PowerUp(PowerUpType.TickRate);
                        }
                        else
                        {
                            if (type <= GameSettings.GameValues.PowerupDrop.Lifes)
                            {
                                //tile.Poweruped = new PowerUp(PowerUpType.ExtraLife);
                            }
                            else
                            {
                                if (type <= GameSettings.GameValues.PowerupDrop.BombRange)
                                {
                                    tile.Poweruped = new PowerUp(PowerUpType.Power);
                                }
                                else
                                {
                                    if (type <= GameSettings.GameValues.PowerupDrop.BombAmount)
                                    {
                                        tile.Poweruped = new PowerUp(PowerUpType.Bomb);
                                    }
                                }
                            }
                        }
                    }
                }
                if (tile.Poweruped != null)
                {
                    GameSettings.gameServer.SendPowerupDrop(tile);
                }
            }
        }

        #region PlayerCollision
        /*public Vector2 PlayerCollisionPos( Player player )
    {
        MapTile tile = null;
        List<MapTile> tiles = GetTilesCollisionWithPlayer( player );
        for ( int i = 0; i < 4; i++ )
        {
            tile = tiles[i];
            if ( tile != null )
            {
                if ( tile.walkable == false )
                {
                    return GetCollisionPos( player, tile );
                } else
                {
                    if ( tile.Bombed != null )
                    {
                        if ( tile.Bombed.Player.Id != Player.Id )
                            return GetCollisionPos( player, tile );
                    }
                }
            } else
            {
                return player.oldPosition;
            }
        }
        return null;
    }

    private Vector2 GetCollisionPos( Player player, MapTile tile ) //Räknar ut vart spelaren colliderade så han kan plaseras där
    {
        switch ( player.newAction )
        {
            case LookDirection.Down:
                return new Vector2( player.Position.X, tile.GetMapPos().Y - player.SizeOffset.Y - player.Size.Y - 1 );
            case LookDirection.Left:
                return new Vector2( tile.GetMapPos().X + MapTile.SIZE - player.SizeOffset.X + 1, player.Position.Y );
            case LookDirection.Right:
                return new Vector2( tile.GetMapPos().X - player.Size.X - player.SizeOffset.X - 1, player.Position.Y );
            case LookDirection.Up:
                return new Vector2( player.Position.X, tile.GetMapPos().Y + MapTile.SIZE - player.SizeOffset.Y + 1);
        }
        return null;
    }*/
        #endregion

        #region Explosions
        public void CalcBombExplosion(OldBomb bomb) //Räknar ut hur explosionen ser ut
        {
            bomb.Explosion.Add(new Explosion(Explosion.ExplosionType.Mid, bomb.Position));
            for (int i = 0; i < 4; i++)
            {
                for (int j = 1; j <= bomb.player.BombPower; j++)
                {
                    MapTile tile = null;
                    switch (i)
                    {
                        case 0: //Up
                            tile = GetTileByTilePosition((int)bomb.Position.TilePosition.X,
                        (int)bomb.Position.TilePosition.Y - j);
                            break;
                        case 1: //Right
                            tile = GetTileByTilePosition((int)bomb.Position.TilePosition.X + j,
                        (int)bomb.Position.TilePosition.Y);
                            break;
                        case 2: //Down
                            tile = GetTileByTilePosition((int)bomb.Position.TilePosition.X,
                        (int)bomb.Position.TilePosition.Y + j);
                            break;
                        case 3: //Left
                            tile = GetTileByTilePosition((int)bomb.Position.TilePosition.X - j,
                        (int)bomb.Position.TilePosition.Y);
                            break;
                    }
                    if (tile != null)
                    {
                        if (tile.walkable)
                        {
                            AddExplosion(bomb, tile, i, j, false);
                        }
                        else
                        {
                            if (tile.stopExplosion)
                            {
                                if (tile.canExplode)
                                {
                                    AddExplosion(bomb, tile, i, j, true);
                                    j = bomb.player.BombPower + 1; //Avsluta denna riktning
                                }
                                else
                                {
                                    j = bomb.player.BombPower + 1; //Avsluta denna riktning
                                }
                            }
                            else
                            {
                                AddExplosion(bomb, tile, i, j, false);
                            }
                        }
                    }
                    else
                    {
                        j = bomb.player.BombPower + 1; //Avsluta denna riktning
                    }
                }
            }
        }

        private void AddExplosion(OldBomb bomb, MapTile tile, int dir, int nr, bool end)
        {
            switch (dir)
            {
                case 0: //Up
                    if (nr == bomb.player.BombPower || end)
                    {
                        bomb.Explosion.Add(new Explosion(Explosion.ExplosionType.UpEnd, tile));
                    }
                    else
                    {
                        bomb.Explosion.Add(new Explosion(Explosion.ExplosionType.Up, tile));
                    }
                    break;
                case 1: //Right
                    if (nr == bomb.player.BombPower || end)
                    {
                        bomb.Explosion.Add(new Explosion(Explosion.ExplosionType.RightEnd, tile));
                    }
                    else
                    {
                        bomb.Explosion.Add(new Explosion(Explosion.ExplosionType.Right, tile));
                    }
                    break;
                case 2: //Down
                    if (nr == bomb.player.BombPower || end)
                    {
                        bomb.Explosion.Add(new Explosion(Explosion.ExplosionType.DownEnd, tile));
                    }
                    else
                    {
                        bomb.Explosion.Add(new Explosion(Explosion.ExplosionType.Down, tile));
                    }
                    break;
                case 3: //Left
                    if (nr == bomb.player.BombPower || end)
                    {
                        bomb.Explosion.Add(new Explosion(Explosion.ExplosionType.LeftEnd, tile));
                    }
                    else
                    {
                        bomb.Explosion.Add(new Explosion(Explosion.ExplosionType.Left, tile));
                    }
                    break;
            }
        }

        public void CheckToRemoveExplodedTiles(OldBomb bomb) //Kollar om bomben tar sönder någon tile
        {
            foreach (Explosion ex in bomb.Explosion)
            {
                if (ex.Position.canExplode)
                {
                    ExplodeTile(ex.Position);
                    bomb.player.Stats.TilesBlowned++;
                }

            }
        }
        #endregion

        //Endofclass
    }
}

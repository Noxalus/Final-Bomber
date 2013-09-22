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

        public string MapName;
        public Point Size;
        public IEntity[,] Board;
        public bool[,] CollisionLayer;
        public List<Point> PlayerSpawnPoints;
        private string _md5;

        #endregion

        #region Constructor Region

        protected BaseMap()
        {
            PlayerSpawnPoints = new List<Point>();
        }

        #endregion

        #region Method Region

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

            if (MapName != null)
            {
                string path = "Content/Maps/" + MapName;

                if (Directory.Exists(path))
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
            }

            return null;
        }

        #endregion
    }
}

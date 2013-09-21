using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FBLibrary.Core.BaseEntities
{
    public abstract class BaseMap
    {
        #region Field Region

        public Point Size;
        public IEntity[,] Board;
        public bool[,] CollisionLayer;

        #endregion

        #region Constructor Region

        protected BaseMap()
        {
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

        #endregion
    }
}

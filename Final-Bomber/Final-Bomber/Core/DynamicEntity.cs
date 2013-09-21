using Final_Bomber.Entities;
using Final_Bomber.WorldEngine;
using Microsoft.Xna.Framework;

namespace Final_Bomber.Core
{
    public abstract class DynamicEntity : Entity
    {
        public Point PreviousCellPosition;

        protected DynamicEntity()
        {
            PreviousCellPosition = Point.Zero;
        }

        public virtual void Update(GameTime gameTime, Map map, int[,] hazardMap)
        {
            PreviousCellPosition = Sprite.CellPosition;
        }

        protected bool IsChangingCell()
        {
            return (Sprite.CellPosition != PreviousCellPosition);
        }
    }
}
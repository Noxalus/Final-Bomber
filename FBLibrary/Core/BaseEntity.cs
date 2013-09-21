using Microsoft.Xna.Framework;

namespace FBLibrary.Core
{
    public abstract class BaseEntity
    {
        protected Vector2 Position;
        protected Point CellPosition;
        protected float Speed;

        protected BaseEntity()
        {
            Position = Vector2.Zero;
            CellPosition = Point.Zero;
            Speed = 0f;
        }

        public void ChangePosition(Point p)
        {
            CellPosition = p;
            Position = Engine.CellToVector(CellPosition);
        }

        public void ChangePosition(int x, int y)
        {
            CellPosition.X = x;
            CellPosition.Y = y;
            Position = Engine.CellToVector(CellPosition);
        }

        public void ChangePosition(float x, float y)
        {
            Position.X = x;
            Position.Y = y;
            CellPosition = Engine.VectorToCell(Position);
        }
    }
}

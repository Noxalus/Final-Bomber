using Microsoft.Xna.Framework;

namespace FBLibrary.Core
{
    public abstract class BaseEntity : IEntity
    {
        private Vector2 _position;
        private Point _cellPosition;

        #region Properties

        public Vector2 Position
        {
            get { return _position; }
            set
            { _position = value; }
        }

        public float PositionX
        {
            set { _position.X = value; }
            get { return _position.X; }
        }

        public float PositionY
        {
            set { _position.Y = value; }
            get { return _position.Y; }
        }

        public int CellPositionX
        {
            get { return _cellPosition.X; }
            set { _cellPosition.X = value; }
        }

        public int CellPositionY
        {
            get { return _cellPosition.Y; }
            set { _cellPosition.Y = value; }
        }

        public Point CellPosition
        {
            get { return _cellPosition; }
        }

        public Point Dimension { get; set; }

        #endregion

        protected BaseEntity()
        {
            Dimension = GameConfiguration.BaseTileSize;
        }

        protected BaseEntity(Point cellPosition) : this()
        {
            _cellPosition = cellPosition;
            Position = Engine.CellToVector(CellPosition);
        }

        public void ChangePosition(Point p)
        {
            _cellPosition = p;
            Position = Engine.CellToVector(CellPosition);
        }

        public void ChangePosition(int x, int y)
        {
            _cellPosition.X = x;
            _cellPosition.Y = y;
            Position = Engine.CellToVector(CellPosition);
        }

        public void ChangePosition(float x, float y)
        {
            _position.X = x;
            _position.Y = y;
            _cellPosition = Engine.VectorToCell(Position);
        }

        public abstract void Destroy();
        public abstract void Remove();
    }
}

using Microsoft.Xna.Framework;

namespace FBLibrary.Core
{
    public abstract class DynamicEntity : BaseEntity
    {
        protected Point PreviousCellPosition;
        private float _speed;
        private Vector2 _velocity;

        public LookDirection CurrentDirection;
        protected LookDirection PreviousDirection;

        protected bool IsMoving;

        public float Speed
        {
            get { return _speed; }
            set { _speed = MathHelper.Clamp(value, GameConfiguration.PlayerSpeedIncrementeurPercentage, GameConfiguration.MaxSpeed); }
        }

        public Vector2 Velocity
        {
            get { return _velocity; }
            set
            {
                _velocity = value;
                if (_velocity != Vector2.Zero)
                    _velocity.Normalize();
            }
        }

        protected DynamicEntity()
        {
            PreviousCellPosition = Point.Zero;
            CurrentDirection = LookDirection.Idle;
            PreviousDirection = CurrentDirection;
        }

        protected DynamicEntity(Point cellPosition) : base(cellPosition)
        {
            PreviousCellPosition = Point.Zero;
        }

        public virtual void Update()
        {
            PreviousCellPosition = CellPosition;
            CellPosition = Engine.VectorToCell(Position, Dimension);
        }

        public bool IsChangingCell()
        {
            return (CellPosition != PreviousCellPosition);
        }
    }
}

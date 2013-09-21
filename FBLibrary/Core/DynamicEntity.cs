using Microsoft.Xna.Framework;

namespace FBLibrary.Core
{
    public abstract class DynamicEntity : BaseEntity
    {
        protected Point PreviousCellPosition;
        private float _speed;
        private Vector2 _velocity;

        protected bool IsMoving;

        public float Speed
        {
            get { return _speed; }
            set { _speed = MathHelper.Clamp(value, GameConfiguration.PlayerSpeedIncrementeur, GameConfiguration.MaxSpeed); }
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
        }

        protected DynamicEntity(Point cellPosition)
        {
            PreviousCellPosition = Point.Zero;
        }
        
        protected virtual void Update()
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

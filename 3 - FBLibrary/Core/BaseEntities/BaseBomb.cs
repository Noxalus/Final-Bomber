
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FBLibrary.Core.BaseEntities
{
    public abstract class BaseBomb : DynamicEntity
    {
        public int PlayerId;
        public TimeSpan Timer;
        protected int Power;
        private bool WillExplode;

        public List<Point> ActionField { get; private set; }

        // Map
        protected Point MapSize;
        protected bool[,] CollisionLayer;

        // References
        protected int[,] HazardMap;

        protected BaseBomb(int playerId, Point cellPosition, int power, TimeSpan timer, float playerSpeed)
            : base(cellPosition)
        {
            // ID of the player that planted the bomb
            PlayerId = playerId;

            Power = power;
            Speed = playerSpeed;

            // Bomb's timer
            Timer = timer;
            WillExplode = false;

            // Action field
            ActionField = new List<Point>();
        }

        public void Initialize(Point mapSize, bool[,] collisionLayer, int[,] hazardMap)
        {
            MapSize = mapSize;
            CollisionLayer = collisionLayer;
            HazardMap = hazardMap;

            ComputeActionField(1);

            DestructionTime = GameConfiguration.BombDestructionTime;
        }

        public override void Update()
        {
            #region Timer

            if (Timer >= TimeSpan.Zero)
            {
                Timer -= TimeSpan.FromMilliseconds(GameConfiguration.DeltaTime);

                // The bomb will explode soon
                if (CurrentDirection == LookDirection.Idle &&
                    !WillExplode && Timer.TotalSeconds < 1)
                {
                    ComputeActionField(2);
                    WillExplode = true;
                }
            }

            #endregion

            base.Update();
        }

        // Compute bomb's effect field: 1 => just planted, 2 => will explode soon, 3 => deathly
        protected void ComputeActionField(int dangerType)
        {
            // Reset the actionField list
            // We put the bomb in its action field
            ActionField = new List<Point> { new Point(CellPosition.X, CellPosition.Y) };

            if (HazardMap[CellPosition.X, CellPosition.Y] < dangerType)
            {
                HazardMap[CellPosition.X, CellPosition.Y] = dangerType;
            }

            // 0 => Top, 1 => Bottom, 2 => Left, 3 => Right
            var obstacles = new Dictionary<String, bool>
            {
                {"up", false},
                {"down", false},
                {"right", false},
                {"left", false}
            };

            int tempPower = Power - 1;
            while (tempPower >= 0)
            {
                // Directions
                int up = CellPosition.Y - (Power - tempPower);
                int down = CellPosition.Y + (Power - tempPower);
                int right = CellPosition.X + (Power - tempPower);
                int left = CellPosition.X - (Power - tempPower);

                // Up
                Point addPosition;
                if (up >= 0 && !obstacles["up"])
                {
                    if (CollisionLayer[CellPosition.X, up])
                        obstacles["up"] = true;

                    addPosition = new Point(CellPosition.X, up);
                    ActionField.Add(addPosition);
                    if (HazardMap[addPosition.X, addPosition.Y] < dangerType)
                    {
                        HazardMap[addPosition.X, addPosition.Y] = dangerType;
                    }
                }

                // Down
                if (down < MapSize.Y - 1 && !obstacles["down"])
                {
                    if (CollisionLayer[CellPosition.X, down])
                        obstacles["down"] = true;

                    addPosition = new Point(CellPosition.X, down);
                    ActionField.Add(addPosition);
                    if (HazardMap[addPosition.X, addPosition.Y] < dangerType)
                    {
                        HazardMap[addPosition.X, addPosition.Y] = dangerType;
                    }
                }

                // Right
                if (right < MapSize.X - 1 && !obstacles["right"])
                {
                    if (CollisionLayer[right, CellPosition.Y])
                        obstacles["right"] = true;

                    addPosition = new Point(right, CellPosition.Y);
                    ActionField.Add(addPosition);
                    if (HazardMap[addPosition.X, addPosition.Y] < dangerType)
                    {
                        HazardMap[addPosition.X, addPosition.Y] = dangerType;
                    }
                }

                // Left
                if (left >= 0 && !obstacles["left"])
                {
                    if (CollisionLayer[left, CellPosition.Y])
                        obstacles["left"] = true;

                    addPosition = new Point(left, CellPosition.Y);
                    ActionField.Add(addPosition);
                    if (HazardMap[addPosition.X, addPosition.Y] < dangerType)
                    {
                        HazardMap[addPosition.X, addPosition.Y] = dangerType;
                    }
                }

                tempPower--;
            }
        }

        public void ChangeSpeed(float changing)
        {
            Speed = changing;
        }

        public void ResetTimer()
        {
            Timer = TimeSpan.Zero;
        }

        public override void Destroy()
        {
            InDestruction = true;

            ComputeActionField(3);
        }

        public override void Remove()
        {
            IsAlive = false;
        }

    }
}


using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FBLibrary.Core.BaseEntities
{
    public abstract class BaseBomb : DynamicEntity
    {
        public int PlayerId;
        protected int Power;
        public TimeSpan Timer;
        public TimeSpan TimerLenght;
        protected bool WillExplode;

        public List<Point> ActionField { get; private set; }

        // References
        protected BaseMap Map;
        protected int[,] HazardMap;

        protected BaseBomb(int playerId, Point cellPosition, int pow, TimeSpan timerLenght, float playerSpeed)
            : base(cellPosition)
        {
            // ID of the player that planted the bomb
            PlayerId = playerId;

            TimerLenght = timerLenght;
            Power = pow;
            Speed = playerSpeed;

            // Bomb's timer
            Timer = TimeSpan.Zero;
            WillExplode = false;

            // Action field
            ActionField = new List<Point>();
        }

        public void Initialize(BaseMap map, int[,] hazardMap)
        {
            Map = map;
            HazardMap = hazardMap;

            ComputeActionField(1);

            DestructionTime = GameConfiguration.BombDestructionTime;
        }

        public override void Update()
        {
            #region Timer

            if (Timer >= TimerLenght)
            {
                Timer = TimeSpan.FromSeconds(-1);
            }
            else if (Timer >= TimeSpan.Zero)
            {
                Timer += TimeSpan.FromMilliseconds(GameConfiguration.DeltaTime);

                // The bomb will explode soon
                if (CurrentDirection == LookDirection.Idle &&
                    !WillExplode && TimerLenght.TotalSeconds - Timer.TotalSeconds < 1)
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
                    if (Map.CollisionLayer[CellPosition.X, up])
                        obstacles["up"] = true;
                    // We don't count the outside walls
                    if (!(Map.Board[CellPosition.X, up] is BaseEdgeWall))
                    {
                        addPosition = new Point(CellPosition.X, up);
                        ActionField.Add(addPosition);
                        if (HazardMap[addPosition.X, addPosition.Y] < dangerType)
                        {
                            HazardMap[addPosition.X, addPosition.Y] = dangerType;
                        }
                    }
                }

                // Down
                if (down < Map.Size.Y - 1 && !obstacles["down"])
                {
                    if (Map.CollisionLayer[CellPosition.X, down])
                        obstacles["down"] = true;
                    // We don't count the outside walls
                    if (!(Map.Board[CellPosition.X, down] is BaseEdgeWall))
                    {
                        addPosition = new Point(CellPosition.X, down);
                        ActionField.Add(addPosition);
                        if (HazardMap[addPosition.X, addPosition.Y] < dangerType)
                        {
                            HazardMap[addPosition.X, addPosition.Y] = dangerType;
                        }
                    }
                }

                // Right
                if (right < Map.Size.X - 1 && !obstacles["right"])
                {
                    if (Map.CollisionLayer[right, CellPosition.Y])
                        obstacles["right"] = true;
                    // We don't count the outside walls
                    if (!(Map.Board[right, CellPosition.Y] is BaseEdgeWall))
                    {
                        addPosition = new Point(right, CellPosition.Y);
                        ActionField.Add(addPosition);
                        if (HazardMap[addPosition.X, addPosition.Y] < dangerType)
                        {
                            HazardMap[addPosition.X, addPosition.Y] = dangerType;
                        }
                    }
                }

                // Left
                if (left >= 0 && !obstacles["left"])
                {
                    if (Map.CollisionLayer[left, CellPosition.Y])
                        obstacles["left"] = true;
                    // We don't count the outside walls
                    if (!(Map.Board[left, CellPosition.Y] is BaseEdgeWall))
                    {
                        addPosition = new Point(left, CellPosition.Y);
                        ActionField.Add(addPosition);
                        if (HazardMap[addPosition.X, addPosition.Y] < dangerType)
                        {
                            HazardMap[addPosition.X, addPosition.Y] = dangerType;
                        }
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


    }
}

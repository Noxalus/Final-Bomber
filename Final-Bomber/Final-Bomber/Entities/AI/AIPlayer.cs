using System.Collections.Generic;
using Final_Bomber.Sprites;
using Final_Bomber.TileEngine;
using Microsoft.Xna.Framework;
using Final_Bomber.WorldEngine;
using Final_Bomber.Screens;
using System;

namespace Final_Bomber.Entities.AI
{
    class AIPlayer : Player
    {
        Vector2 _aiNextPosition;

        public List<Point> Path { get; private set; }

        public AIPlayer(int id)
            : base(id)
        {
            // AI
            _aiNextPosition = new Vector2(-1, -1);
            Path = new List<Point>();
        }

        protected override void Move()
        {
            Level level = FinalBomber.Instance.GamePlayScreen.World.Levels[FinalBomber.Instance.GamePlayScreen.World.CurrentLevel];

            #region Walk
            // If he hasn't reach his goal => we walk to this goal
            if (_aiNextPosition != new Vector2(-1, -1) && !AIFunction.HasReachNextPosition(Sprite.Position, Sprite.Speed, _aiNextPosition))
            {
                this.IsMoving = true;
                Sprite.IsAnimating = true;

                CheckIsBlocked(level);

                Walk();

                ComputeWallCollision();
            }
            #endregion

            #region Search a goal
            // Otherwise => we find another goal
            else
            {
                // We place the player at the center of its cell
                Sprite.Position = Engine.CellToVector(Sprite.CellPosition);

                #region Bomb => AI
                // Try to put a bomb
                // Put a bomb
                if (!HasBadItemEffect || (HasBadItemEffect && BadItemEffect != BadItemEffect.NoBomb))
                {
                    if (AIFunction.TryToPutBomb(Sprite.CellPosition, Power, level.Map, level.CollisionLayer, level.HazardMap, Config.MapSize))
                    {
                        if (this.CurrentBombNumber > 0)
                        {
                            var bo = FinalBomber.Instance.GamePlayScreen.BombList.Find(b => b.Sprite.CellPosition == this.Sprite.CellPosition);
                            if (bo == null)
                            {
                                this.CurrentBombNumber--;
                                var bomb = new Bomb(FinalBomber.Instance, this.Id, Sprite.CellPosition, this.Power, this.BombTimer, this.Sprite.Speed);

                                // We define a new way (to escape the bomb)
                                Path = AIFunction.MakeAWay(
                                    Sprite.CellPosition,
                                    AIFunction.SetNewDefenseGoal(Sprite.CellPosition, level.CollisionLayer, level.HazardMap, Config.MapSize),
                                    level.CollisionLayer, level.HazardMap, Config.MapSize);

                                FinalBomber.Instance.GamePlayScreen.AddBomb(bomb);
                            }
                        }
                    }
                }
                #endregion

                if (Path == null || Path.Count == 0)
                {
                    Sprite.IsAnimating = false;
                    this.IsMoving = false;
                    // We define a new goal
                    Path = AIFunction.MakeAWay(
                        Sprite.CellPosition,
                        AIFunction.SetNewGoal(Sprite.CellPosition, level.Map, level.CollisionLayer, level.HazardMap, Config.MapSize),
                        level.CollisionLayer, level.HazardMap, Config.MapSize);

                    if (Path != null)
                    {
                        _aiNextPosition = Engine.CellToVector(Path[Path.Count - 1]);
                        Path.Remove(Path[Path.Count - 1]);

                        CheckIsBlocked(level);
                    }
                }
                else
                {
                    // We finish the current way
                    _aiNextPosition = Engine.CellToVector(Path[Path.Count - 1]);
                    Path.Remove(Path[Path.Count - 1]);
                    /*
                    // Update the way of the AI each time it changes of cell => usefull to battle against players (little bug here)
                    aiWay = AI.MakeAWay(
                        Sprite.CellPosition,
                        AI.SetNewGoal(Sprite.CellPosition, level.Map, level.CollisionLayer, level.HazardMap), 
                        level.CollisionLayer, level.HazardMap);
                    */
                }
            }
            #endregion

            UpdatePlayerPosition();
        }

        private void Walk()
        {
            // Up
            if (Sprite.Position.Y > _aiNextPosition.Y)
            {
                Sprite.Position = new Vector2(Sprite.Position.X, Sprite.Position.Y - Sprite.Speed);
                Sprite.CurrentAnimation = AnimationKey.Up;
                LookDirection = LookDirection.Up;
            }
            // Down
            else if (Sprite.Position.Y < _aiNextPosition.Y)
            {
                Sprite.Position = new Vector2(Sprite.Position.X, Sprite.Position.Y + Sprite.Speed);
                Sprite.CurrentAnimation = AnimationKey.Down;
                LookDirection = LookDirection.Down;
            }
            // Right
            else if (Sprite.Position.X < _aiNextPosition.X)
            {
                Sprite.Position = new Vector2(Sprite.Position.X + Sprite.Speed, Sprite.Position.Y);
                Sprite.CurrentAnimation = AnimationKey.Right;
                LookDirection = LookDirection.Right;
            }
            // Left
            else if (Sprite.Position.X > _aiNextPosition.X)
            {
                Sprite.Position = new Vector2(Sprite.Position.X - Sprite.Speed, Sprite.Position.Y);
                Sprite.CurrentAnimation = AnimationKey.Left;
                LookDirection = LookDirection.Left;
            }
        }

        private void CheckIsBlocked(Level level)
        {
            // If the AI is blocked
            if (level.CollisionLayer[Engine.VectorToCell(_aiNextPosition).X, Engine.VectorToCell(_aiNextPosition).Y] ||
                    level.HazardMap[Engine.VectorToCell(_aiNextPosition).X, Engine.VectorToCell(_aiNextPosition).Y] >= 2)
            {
                Sprite.IsAnimating = false;
                this.IsMoving = false;
                // We define a new goal
                bool[,] collisionLayer = level.CollisionLayer;
                int[,] hazardMap = level.HazardMap;
                Entity[,] map = level.Map;
                Path = AIFunction.MakeAWay(
                    Sprite.CellPosition,
                    AIFunction.SetNewGoal(Sprite.CellPosition, map, collisionLayer, hazardMap, Config.MapSize),
                    collisionLayer, hazardMap, Config.MapSize);
            }
        }

        protected override void MoveFromEdgeWall()
        {

        }

        public override void ApplyBadItem(BadItemEffect effect)
        {
            base.ApplyBadItem(effect);

            switch (effect)
            {
                case BadItemEffect.TooSlow:
                    this.SpeedSaved = this.Sprite.Speed;
                    this.Sprite.Speed = Config.MinSpeed;
                    break;
                case BadItemEffect.TooSpeed:
                    SpeedSaved = this.Sprite.Speed;
                    this.Sprite.Speed = Config.MaxSpeed;
                    break;
                case BadItemEffect.BombTimerChanged:
                    this.BombTimerSaved = this.BombTimer;
                    int randomBombTimer = GamePlayScreen.Random.Next(Config.BadItemTimerChangedMin, Config.BadItemTimerChangedMax);
                    this.BombTimer = TimeSpan.FromSeconds(randomBombTimer);
                    break;
            }
        }

        protected override void RemoveBadItem()
        {
            base.RemoveBadItem();

            switch (BadItemEffect)
            {
                case BadItemEffect.TooSlow:
                    Sprite.Speed = SpeedSaved;
                    break;
                case BadItemEffect.TooSpeed:
                    Sprite.Speed = SpeedSaved;
                    break;
                case BadItemEffect.BombTimerChanged:
                    BombTimer = BombTimerSaved;
                    break;
            }
        }
    }
}

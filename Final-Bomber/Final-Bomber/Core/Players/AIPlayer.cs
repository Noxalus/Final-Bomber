using System;
using System.Collections.Generic;
using FBLibrary;
using FBLibrary.Core;
using Final_Bomber.Core.Entities;
using Final_Bomber.Entities;
using Final_Bomber.Entities.AI;
using Final_Bomber.Screens;
using Final_Bomber.Sprites;
using Final_Bomber.WorldEngine;
using Microsoft.Xna.Framework;

namespace Final_Bomber.Core.Players
{
    internal class AIPlayer : Player
    {
        private Vector2 _aiNextPosition;

        public AIPlayer(int id)
            : base(id)
        {
            // AI
            _aiNextPosition = new Vector2(-1, -1);
            Path = new List<Point>();
        }

        public List<Point> Path { get; private set; }

        protected override void Move(GameTime gameTime, Map map, int[,] hazardMap)
        {
            #region Walk

            // If he hasn't reach his goal => we walk to this goal
            if (_aiNextPosition != new Vector2(-1, -1) &&
                !AIFunction.HasReachNextPosition(Position, Speed, _aiNextPosition))
            {
                IsMoving = true;
                Sprite.IsAnimating = true;

                CheckIsBlocked(map, hazardMap);

                Walk();

                ComputeWallCollision(map);
            }
                #endregion

                #region Search a goal
                // Otherwise => we find another goal
            else
            {
                // We place the player at the center of its cell
                Position = Engine.CellToVector(CellPosition);

                #region Bomb => AI

                // Try to put a bomb
                // Put a bomb
                if (!HasBadEffect || (HasBadEffect && BadEffect != BadEffect.NoBomb))
                {
                    if (AIFunction.TryToPutBomb(CellPosition, BombPower, map.Board, map.CollisionLayer, hazardMap,
                        Config.MapSize))
                    {
                        if (CurrentBombAmount > 0)
                        {
                            Bomb bo =
                                FinalBomber.Instance.GamePlayScreen.BombList.Find(
                                    b => b.CellPosition == CellPosition);
                            if (bo == null)
                            {
                                CurrentBombAmount--;
                                var bomb = new Bomb(Id, CellPosition, BombPower, BombTimer, Speed);

                                // We define a new way (to escape the bomb)
                                Path = AIFunction.MakeAWay(
                                    CellPosition,
                                    AIFunction.SetNewDefenseGoal(CellPosition, map.CollisionLayer, hazardMap,
                                        Config.MapSize),
                                    map.CollisionLayer, hazardMap, Config.MapSize);

                                FinalBomber.Instance.GamePlayScreen.AddBomb(bomb);
                            }
                        }
                    }
                }

                #endregion

                if (Path == null || Path.Count == 0)
                {
                    Sprite.IsAnimating = false;
                    IsMoving = false;
                    // We define a new goal
                    Path = AIFunction.MakeAWay(
                        CellPosition,
                        AIFunction.SetNewGoal(CellPosition, map.Board, map.CollisionLayer, hazardMap,
                            Config.MapSize),
                        map.CollisionLayer, hazardMap, Config.MapSize);

                    if (Path != null)
                    {
                        _aiNextPosition = Engine.CellToVector(Path[Path.Count - 1]);
                        Path.Remove(Path[Path.Count - 1]);

                        CheckIsBlocked(map, hazardMap);
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
                        CellPosition,
                        AI.SetNewGoal(CellPosition, map.Board, map.CollisionLayer, hazardMap), 
                        map.CollisionLayer, hazardMap);
                    */
                }
            }

            #endregion

            UpdatePlayerPosition(map);
        }

        private void Walk()
        {
            // Up
            if (Position.Y > _aiNextPosition.Y)
            {
                Position = new Vector2(Position.X, Position.Y - Speed);
                Sprite.CurrentAnimation = AnimationKey.Up;
                CurrentDirection = LookDirection.Up;
            }
                // Down
            else if (Position.Y < _aiNextPosition.Y)
            {
                Position = new Vector2(Position.X, Position.Y + Speed);
                Sprite.CurrentAnimation = AnimationKey.Down;
                CurrentDirection = LookDirection.Down;
            }
                // Right
            else if (Position.X < _aiNextPosition.X)
            {
                Position = new Vector2(Position.X + Speed, Position.Y);
                Sprite.CurrentAnimation = AnimationKey.Right;
                CurrentDirection = LookDirection.Right;
            }
                // Left
            else if (Position.X > _aiNextPosition.X)
            {
                Position = new Vector2(Position.X - Speed, Position.Y);
                Sprite.CurrentAnimation = AnimationKey.Left;
                CurrentDirection = LookDirection.Left;
            }
        }

        private void CheckIsBlocked(Map map, int[,] hazardMap)
        {
            // If the AI is blocked
            if (map.CollisionLayer[Engine.VectorToCell(_aiNextPosition).X, Engine.VectorToCell(_aiNextPosition).Y] ||
                hazardMap[Engine.VectorToCell(_aiNextPosition).X, Engine.VectorToCell(_aiNextPosition).Y] >= 2)
            {
                Sprite.IsAnimating = false;
                IsMoving = false;
                // We define a new goal
                Path = AIFunction.MakeAWay(
                    CellPosition,
                    AIFunction.SetNewGoal(CellPosition, map.Board, map.CollisionLayer, hazardMap, map.Size),
                    map.CollisionLayer, hazardMap, map.Size);
            }
        }

        protected override void MoveFromEdgeWall()
        {
        }

        public override void ApplyBadItem(BadEffect effect)
        {
            base.ApplyBadItem(effect);

            switch (effect)
            {
                case BadEffect.TooSlow:
                    SpeedSaved = Speed;
                    Speed = Config.MinSpeed;
                    break;
                case BadEffect.TooSpeed:
                    SpeedSaved = Speed;
                    Speed = Config.MaxSpeed;
                    break;
                case BadEffect.BombTimerChanged:
                    BombTimerSaved = BombTimer;
                    int randomBombTimer = GamePlayScreen.Random.Next(GameConfiguration.BadItemTimerChangedMin,
                        GameConfiguration.BadItemTimerChangedMax);
                    BombTimer = TimeSpan.FromSeconds(randomBombTimer);
                    break;
            }
        }

        protected override void RemoveBadItem()
        {
            base.RemoveBadItem();

            switch (BadEffect)
            {
                case BadEffect.TooSlow:
                    Speed = SpeedSaved;
                    break;
                case BadEffect.TooSpeed:
                    Speed = SpeedSaved;
                    break;
                case BadEffect.BombTimerChanged:
                    BombTimer = BombTimerSaved;
                    break;
            }
        }
    }
}
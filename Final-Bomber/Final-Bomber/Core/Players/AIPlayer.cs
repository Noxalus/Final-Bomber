using System;
using System.Collections.Generic;
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
                !AIFunction.HasReachNextPosition(Sprite.Position, Sprite.Speed, _aiNextPosition))
            {
                IsMoving = true;
                Sprite.IsAnimating = true;

                CheckIsBlocked(map, hazardMap);

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
                    if (AIFunction.TryToPutBomb(Sprite.CellPosition, BombPower, map.Board, map.CollisionLayer, hazardMap,
                        Config.MapSize))
                    {
                        if (CurrentBombAmount > 0)
                        {
                            Bomb bo =
                                FinalBomber.Instance.GamePlayScreen.BombList.Find(
                                    b => b.Sprite.CellPosition == Sprite.CellPosition);
                            if (bo == null)
                            {
                                CurrentBombAmount--;
                                var bomb = new Bomb(Id, Sprite.CellPosition, BombPower, BombTimer, Sprite.Speed);

                                // We define a new way (to escape the bomb)
                                Path = AIFunction.MakeAWay(
                                    Sprite.CellPosition,
                                    AIFunction.SetNewDefenseGoal(Sprite.CellPosition, map.CollisionLayer, hazardMap,
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
                        Sprite.CellPosition,
                        AIFunction.SetNewGoal(Sprite.CellPosition, map.Board, map.CollisionLayer, hazardMap,
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
                        Sprite.CellPosition,
                        AI.SetNewGoal(Sprite.CellPosition, map.Board, map.CollisionLayer, hazardMap), 
                        map.CollisionLayer, hazardMap);
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
                CurrentDirection = LookDirection.Up;
            }
                // Down
            else if (Sprite.Position.Y < _aiNextPosition.Y)
            {
                Sprite.Position = new Vector2(Sprite.Position.X, Sprite.Position.Y + Sprite.Speed);
                Sprite.CurrentAnimation = AnimationKey.Down;
                CurrentDirection = LookDirection.Down;
            }
                // Right
            else if (Sprite.Position.X < _aiNextPosition.X)
            {
                Sprite.Position = new Vector2(Sprite.Position.X + Sprite.Speed, Sprite.Position.Y);
                Sprite.CurrentAnimation = AnimationKey.Right;
                CurrentDirection = LookDirection.Right;
            }
                // Left
            else if (Sprite.Position.X > _aiNextPosition.X)
            {
                Sprite.Position = new Vector2(Sprite.Position.X - Sprite.Speed, Sprite.Position.Y);
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
                    Sprite.CellPosition,
                    AIFunction.SetNewGoal(Sprite.CellPosition, map.Board, map.CollisionLayer, hazardMap, map.Size),
                    map.CollisionLayer, hazardMap, map.Size);
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
                    SpeedSaved = Sprite.Speed;
                    Sprite.Speed = Config.MinSpeed;
                    break;
                case BadItemEffect.TooSpeed:
                    SpeedSaved = Sprite.Speed;
                    Sprite.Speed = Config.MaxSpeed;
                    break;
                case BadItemEffect.BombTimerChanged:
                    BombTimerSaved = BombTimer;
                    int randomBombTimer = GamePlayScreen.Random.Next(Config.BadItemTimerChangedMin,
                        Config.BadItemTimerChangedMax);
                    BombTimer = TimeSpan.FromSeconds(randomBombTimer);
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
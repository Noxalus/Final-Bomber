using System;
using FBLibrary.Core;
using Final_Bomber.Controls;
using Final_Bomber.Core.Entities;
using Final_Bomber.Entities;
using Final_Bomber.Screens;
using Final_Bomber.Sprites;
using Final_Bomber.WorldEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Final_Bomber.Core.Players
{
    internal class HumanPlayer : Player
    {
        private Keys[] _keysSaved;

        public HumanPlayer(int id)
            : base(id)
        {
            Keys = Config.PlayersKeys[id - 1];
            Buttons = Config.PlayersButtons[id - 1];
        }

        public Keys[] Keys { get; set; }
        public Buttons[] Buttons { get; set; }

        public void Update(GameTime gameTime, Map map, int[,] hazardMap)
        {
            base.Update(gameTime, map, hazardMap);
        }

        protected override void Move(GameTime gameTime, Map map, int[,] hazardMap)
        {
            #region Moving input

            var motion = new Vector2();

            // Up
            if ((Config.PlayersUsingController[Id] && InputHandler.ButtonDown(Buttons[0], PlayerIndex.One)) ||
                InputHandler.KeyDown(Keys[0]))
            {
                Sprite.CurrentAnimation = AnimationKey.Up;
                CurrentDirection = LookDirection.Up;
                motion.Y = -1;
            }
                // Down
            else if ((Config.PlayersUsingController[Id] && InputHandler.ButtonDown(Buttons[1], PlayerIndex.One)) ||
                     InputHandler.KeyDown(Keys[1]))
            {
                Sprite.CurrentAnimation = AnimationKey.Down;
                CurrentDirection = LookDirection.Down;
                motion.Y = 1;
            }
                // Left
            else if ((Config.PlayersUsingController[Id] && InputHandler.ButtonDown(Buttons[2], PlayerIndex.One)) ||
                     InputHandler.KeyDown(Keys[2]))
            {
                Sprite.CurrentAnimation = AnimationKey.Left;
                CurrentDirection = LookDirection.Left;
                motion.X = -1;
            }
                // Right
            else if ((Config.PlayersUsingController[Id] && InputHandler.ButtonDown(Buttons[3], PlayerIndex.One)) ||
                     InputHandler.KeyDown(Keys[3]))
            {
                Sprite.CurrentAnimation = AnimationKey.Right;
                CurrentDirection = LookDirection.Right;
                motion.X = 1;
            }
            else
                CurrentDirection = LookDirection.Idle;

            #endregion

            #region Moving action

            // If the human player wants to move
            if (motion != Vector2.Zero)
            {
                IsMoving = true;
                Sprite.IsAnimating = true;
                motion.Normalize();

                Vector2 nextPosition = Sprite.Position + motion*Sprite.Speed;
                Point nextPositionCell = Engine.VectorToCell(nextPosition, Sprite.Dimension);

                #region Moving of the player

                // We move the player
                Sprite.Position += motion*Sprite.Speed;

                // If the player want to go to top...
                if (motion.Y == -1f)
                {
                    // ...  and that there is a wall
                    if (WallAt(new Point(Sprite.CellPosition.X, Sprite.CellPosition.Y - 1)))
                    {
                        // If he is more on the left side, we lag him to the left
                        if (IsMoreLeftSide() && !WallAt(new Point(Sprite.CellPosition.X - 1, Sprite.CellPosition.Y - 1)))
                        {
                            if (!WallAt(new Point(Sprite.CellPosition.X - 1, Sprite.CellPosition.Y)))
                                Sprite.Position = new Vector2(Sprite.Position.X - Sprite.Speed, Sprite.Position.Y);
                        }
                        else if (IsMoreRightSide() &&
                                 !WallAt(new Point(Sprite.CellPosition.X + 1, Sprite.CellPosition.Y - 1)))
                        {
                            if (!WallAt(new Point(Sprite.CellPosition.X + 1, Sprite.CellPosition.Y)))
                                Sprite.Position = new Vector2(Sprite.Position.X + Sprite.Speed, Sprite.Position.Y);
                        }
                    }
                        // ... and that there is no wall
                    else
                    {
                        // If he is more on the left side
                        if (IsMoreLeftSide())
                        {
                            Sprite.Position = new Vector2(Sprite.Position.X + Sprite.Speed, Sprite.Position.Y);
                        }
                            // If he is more on the right side
                        else if (IsMoreRightSide())
                        {
                            Sprite.Position = new Vector2(Sprite.Position.X - Sprite.Speed, Sprite.Position.Y);
                        }
                    }
                }
                    // If the player want to go to bottom and that there is a wall
                else if (motion.Y == 1)
                {
                    // Wall at the bottom ?
                    if (WallAt(new Point(Sprite.CellPosition.X, Sprite.CellPosition.Y + 1)))
                    {
                        // If he is more on the left side, we lag him to the left
                        if (IsMoreLeftSide() && !WallAt(new Point(Sprite.CellPosition.X - 1, Sprite.CellPosition.Y + 1)))
                        {
                            if (!WallAt(new Point(Sprite.CellPosition.X - 1, Sprite.CellPosition.Y)))
                                Sprite.Position = new Vector2(Sprite.Position.X - Sprite.Speed, Sprite.Position.Y);
                        }
                        else if (IsMoreRightSide() &&
                                 !WallAt(new Point(Sprite.CellPosition.X + 1, Sprite.CellPosition.Y + 1)))
                        {
                            if (!WallAt(new Point(Sprite.CellPosition.X + 1, Sprite.CellPosition.Y)))
                                Sprite.Position = new Vector2(Sprite.Position.X + Sprite.Speed, Sprite.Position.Y);
                        }
                    }
                    else
                    {
                        // If he is more on the left side
                        if (IsMoreLeftSide())
                        {
                            Sprite.Position = new Vector2(Sprite.Position.X + Sprite.Speed, Sprite.Position.Y);
                        }
                            // If he is more on the right side
                        else if (IsMoreRightSide())
                        {
                            Sprite.Position = new Vector2(Sprite.Position.X - Sprite.Speed, Sprite.Position.Y);
                        }
                    }
                }
                    // If the player want to go to left and that there is a wall
                else if (motion.X == -1)
                {
                    if (WallAt(new Point(Sprite.CellPosition.X - 1, Sprite.CellPosition.Y)))
                    {
                        // If he is more on the top side, we lag him to the top
                        if (IsMoreTopSide() && !WallAt(new Point(Sprite.CellPosition.X - 1, Sprite.CellPosition.Y - 1)))
                        {
                            if (!WallAt(new Point(Sprite.CellPosition.X, Sprite.CellPosition.Y - 1)))
                                Sprite.Position = new Vector2(Sprite.Position.X, Sprite.Position.Y - Sprite.Speed);
                        }
                        else if (IsMoreBottomSide() &&
                                 !WallAt(new Point(Sprite.CellPosition.X - 1, Sprite.CellPosition.Y + 1)))
                        {
                            if (!WallAt(new Point(Sprite.CellPosition.X, Sprite.CellPosition.Y + 1)))
                                Sprite.Position = new Vector2(Sprite.Position.X, Sprite.Position.Y + Sprite.Speed);
                        }
                    }
                    else
                    {
                        // If he is more on the top side, we lag him to the bottom
                        if (IsMoreTopSide())
                        {
                            Sprite.Position = new Vector2(Sprite.Position.X, Sprite.Position.Y + Sprite.Speed);
                        }
                        else if (IsMoreBottomSide())
                        {
                            Sprite.Position = new Vector2(Sprite.Position.X, Sprite.Position.Y - Sprite.Speed);
                        }
                    }
                }
                    // If the player want to go to right and that there is a wall
                else if (motion.X == 1)
                {
                    if (WallAt(new Point(Sprite.CellPosition.X + 1, Sprite.CellPosition.Y)))
                    {
                        // If he is more on the top side, we lag him to the top
                        if (IsMoreTopSide() && !WallAt(new Point(Sprite.CellPosition.X + 1, Sprite.CellPosition.Y - 1)))
                        {
                            if (!WallAt(new Point(Sprite.CellPosition.X, Sprite.CellPosition.Y - 1)))
                                Sprite.Position = new Vector2(Sprite.Position.X, Sprite.Position.Y - Sprite.Speed);
                        }
                        else if (IsMoreBottomSide() &&
                                 !WallAt(new Point(Sprite.CellPosition.X + 1, Sprite.CellPosition.Y + 1)))
                        {
                            if (!WallAt(new Point(Sprite.CellPosition.X, Sprite.CellPosition.Y + 1)))
                                Sprite.Position = new Vector2(Sprite.Position.X, Sprite.Position.Y + Sprite.Speed);
                        }
                    }
                    else
                    {
                        // If he is more on the top side, we lag him to the top
                        if (IsMoreTopSide())
                        {
                            Sprite.Position = new Vector2(Sprite.Position.X, Sprite.Position.Y + Sprite.Speed);
                        }
                        else if (IsMoreBottomSide())
                        {
                            Sprite.Position = new Vector2(Sprite.Position.X, Sprite.Position.Y - Sprite.Speed);
                        }
                    }
                }

                #endregion

                ComputeWallCollision();
            }
            else
            {
                IsMoving = false;
                Sprite.IsAnimating = false;
            }

            UpdatePlayerPosition();

            #region Bomb

            if ((HasBadItemEffect && BadItemEffect == BadItemEffect.BombDrop) ||
                ((Config.PlayersUsingController[Id] && InputHandler.ButtonDown(Buttons[4], PlayerIndex.One)) ||
                 InputHandler.KeyPressed(Keys[4]) &&
                 (!HasBadItemEffect || (HasBadItemEffect && BadItemEffect != BadItemEffect.NoBomb))))
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

                        FinalBomber.Instance.GamePlayScreen.AddBomb(bomb);
                    }
                }
            }

            #endregion

            #endregion
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
                case BadItemEffect.KeysInversion:
                    _keysSaved = (Keys[]) Keys.Clone();
                    var inversedKeysArray = new[] {1, 0, 3, 2};
                    for (int i = 0; i < inversedKeysArray.Length; i++)
                        Keys[i] = _keysSaved[inversedKeysArray[i]];
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
                case BadItemEffect.KeysInversion:
                    Keys = _keysSaved;
                    break;
                case BadItemEffect.BombTimerChanged:
                    BombTimer = BombTimerSaved;
                    break;
            }
        }

        protected override void MoveFromEdgeWall()
        {
            base.MoveFromEdgeWall();

            // The player is either at the top either at the bottom
            // => he can only move on the right or on the left
            if (Sprite.Position.Y <= 0 || Sprite.Position.Y >= (Config.MapSize.Y - 1)*Engine.TileHeight)
            {
                // If he wants to go to the left
                if (Sprite.Position.X > 0 && InputHandler.KeyDown(Keys[2]))
                    Sprite.Position = new Vector2(Sprite.Position.X - Sprite.Speed, Sprite.Position.Y);
                    // If he wants to go to the right
                else if (Sprite.Position.X < (Config.MapSize.X*Engine.TileWidth) - Engine.TileWidth &&
                         InputHandler.KeyDown(Keys[3]))
                    Sprite.Position = new Vector2(Sprite.Position.X + Sprite.Speed, Sprite.Position.Y);
            }
            // The player is either on the left either on the right
            if (Sprite.Position.X <= 0 || Sprite.Position.X >= (Config.MapSize.X - 1)*Engine.TileWidth)
            {
                // If he wants to go to the top
                if (Sprite.Position.Y > 0 && InputHandler.KeyDown(Keys[0]))
                    Sprite.Position = new Vector2(Sprite.Position.X, Sprite.Position.Y - Sprite.Speed);
                    // If he wants to go to the bottom
                else if (Sprite.Position.Y < (Config.MapSize.Y*Engine.TileHeight) - Engine.TileHeight &&
                         InputHandler.KeyDown(Keys[1]))
                    Sprite.Position = new Vector2(Sprite.Position.X, Sprite.Position.Y + Sprite.Speed);
            }

            if (Sprite.Position.Y <= 0)
                Sprite.CurrentAnimation = AnimationKey.Down;
            else if (Sprite.Position.Y >= (Config.MapSize.Y - 1)*Engine.TileHeight)
                Sprite.CurrentAnimation = AnimationKey.Up;
            else if (Sprite.Position.X <= 0)
                Sprite.CurrentAnimation = AnimationKey.Right;
            else if (Sprite.Position.X >= (Config.MapSize.X - 1)*Engine.TileWidth)
                Sprite.CurrentAnimation = AnimationKey.Left;

            #region Bombs => Edge gameplay

            if (InputHandler.KeyDown(Keys[4]) && CurrentBombAmount > 0)
            {
                // He can't put a bomb when he is on a corner
                if (
                    !((Sprite.CellPosition.Y == 0 &&
                       (Sprite.CellPosition.X == 0 || Sprite.CellPosition.X == Config.MapSize.X - 1)) ||
                      (Sprite.CellPosition.Y == Config.MapSize.Y - 1 &&
                       (Sprite.CellPosition.X == 0 || (Sprite.CellPosition.X == Config.MapSize.X - 1)))))
                {
                    Map level =
                        FinalBomber.Instance.GamePlayScreen.World.Levels[
                            FinalBomber.Instance.GamePlayScreen.World.CurrentLevel];
                    int lag = 0;
                    Point bombPosition = Sprite.CellPosition;
                    // Up
                    if (Sprite.CellPosition.Y == 0)
                    {
                        while (Sprite.CellPosition.Y + lag + 3 < Config.MapSize.Y &&
                               level.CollisionLayer[Sprite.CellPosition.X, Sprite.CellPosition.Y + lag + 3])
                        {
                            lag++;
                        }
                        bombPosition.Y = Sprite.CellPosition.Y + lag + 3;
                        if (bombPosition.Y < Config.MapSize.Y)
                        {
                            var bomb = new Bomb(Id, bombPosition, BombPower, BombTimer, GameConfiguration.BaseBombSpeed + Sprite.Speed);
                            level.CollisionLayer[bombPosition.X, bombPosition.Y] = true;
                            FinalBomber.Instance.GamePlayScreen.BombList.Add(bomb);
                            level.Board[bombPosition.X, bombPosition.Y] = bomb;
                            CurrentBombAmount--;
                        }
                    }
                    // Down
                    if (Sprite.CellPosition.Y == Config.MapSize.Y - 1)
                    {
                        while (Sprite.CellPosition.Y - lag - 3 >= 0 &&
                               level.CollisionLayer[Sprite.CellPosition.X, Sprite.CellPosition.Y - lag - 3])
                        {
                            lag++;
                        }
                        bombPosition.Y = Sprite.CellPosition.Y - lag - 3;
                        if (bombPosition.Y >= 0)
                        {
                            var bomb = new Bomb(Id, bombPosition, BombPower, BombTimer, GameConfiguration.BaseBombSpeed + Sprite.Speed);
                            level.CollisionLayer[bombPosition.X, bombPosition.Y] = true;
                            FinalBomber.Instance.GamePlayScreen.BombList.Add(bomb);
                            level.Board[bombPosition.X, bombPosition.Y] = bomb;
                            CurrentBombAmount--;
                        }
                    }
                    // Left
                    if (Sprite.CellPosition.X == 0)
                    {
                        while (Sprite.CellPosition.X + lag + 3 < Config.MapSize.X &&
                               level.CollisionLayer[Sprite.CellPosition.X + lag + 3, Sprite.CellPosition.Y])
                        {
                            lag++;
                        }
                        bombPosition.X = Sprite.CellPosition.X + lag + 3;
                        if (bombPosition.X < Config.MapSize.X)
                        {
                            var bomb = new Bomb(Id, bombPosition, BombPower, BombTimer, GameConfiguration.BaseBombSpeed + Sprite.Speed);
                            level.CollisionLayer[bombPosition.X, bombPosition.Y] = true;
                            FinalBomber.Instance.GamePlayScreen.BombList.Add(bomb);
                            level.Board[bombPosition.X, bombPosition.Y] = bomb;
                            CurrentBombAmount--;
                        }
                    }
                    // Right
                    if (Sprite.CellPosition.X == Config.MapSize.X - 1)
                    {
                        while (Sprite.CellPosition.X - lag - 3 >= 0 &&
                               level.CollisionLayer[Sprite.CellPosition.X - lag - 3, Sprite.CellPosition.Y])
                        {
                            lag++;
                        }
                        bombPosition.X = Sprite.CellPosition.X - lag - 3;
                        if (bombPosition.X >= 0)
                        {
                            var bomb = new Bomb(Id, bombPosition, BombPower, BombTimer, GameConfiguration.BaseBombSpeed + Sprite.Speed);
                            level.CollisionLayer[bombPosition.X, bombPosition.Y] = true;
                            FinalBomber.Instance.GamePlayScreen.BombList.Add(bomb);
                            level.Board[bombPosition.X, bombPosition.Y] = bomb;
                            CurrentBombAmount--;
                        }
                    }
                }
            }

            #endregion
        }
    }
}
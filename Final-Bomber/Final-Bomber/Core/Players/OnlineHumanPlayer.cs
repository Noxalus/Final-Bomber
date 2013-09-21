using System;
using System.Diagnostics;
using FBLibrary.Core;
using Final_Bomber.Controls;
using Final_Bomber.Core.Entities;
using Final_Bomber.Entities;
using Final_Bomber.Network;
using Final_Bomber.Screens;
using Final_Bomber.Sprites;
using Final_Bomber.WorldEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Final_Bomber.Core.Players
{
    public class OnlineHumanPlayer : Player
    {
        private Keys[] _keysSaved;
        private Vector2 _motionVector;

        public OnlineHumanPlayer(int id)
            : base(id)
        {
            Keys = Config.PlayersKeys[Id];
            Buttons = Config.PlayersButtons[Id];
            _motionVector = Vector2.Zero;
        }

        private Keys[] Keys { get; set; }
        private Buttons[] Buttons { get; set; }

        protected override void Move(GameTime gameTime, Map map, int[,] hazardMap)
        {
            #region Moving input

            _motionVector = Vector2.Zero;

            // Up
            if ((Config.PlayersUsingController[Id] && InputHandler.ButtonDown(Buttons[0], PlayerIndex.One)) ||
                InputHandler.KeyDown(Keys[0]))
            {
                Sprite.CurrentAnimation = AnimationKey.Up;
                CurrentDirection = LookDirection.Up;
                _motionVector.Y = -1;
            }
                // Down
            else if ((Config.PlayersUsingController[Id] && InputHandler.ButtonDown(Buttons[1], PlayerIndex.One)) ||
                     InputHandler.KeyDown(Keys[1]))
            {
                Sprite.CurrentAnimation = AnimationKey.Down;
                CurrentDirection = LookDirection.Down;
                _motionVector.Y = 1;
            }
                // Left
            else if ((Config.PlayersUsingController[Id] && InputHandler.ButtonDown(Buttons[2], PlayerIndex.One)) ||
                     InputHandler.KeyDown(Keys[2]))
            {
                Sprite.CurrentAnimation = AnimationKey.Left;
                CurrentDirection = LookDirection.Left;
                _motionVector.X = -1;
            }
                // Right
            else if ((Config.PlayersUsingController[Id] && InputHandler.ButtonDown(Buttons[3], PlayerIndex.One)) ||
                     InputHandler.KeyDown(Keys[3]))
            {
                Sprite.CurrentAnimation = AnimationKey.Right;
                CurrentDirection = LookDirection.Right;
                _motionVector.X = 1;
            }
            else
            {
                CurrentDirection = LookDirection.Idle;
            }

            #endregion

            //Debug.Print("Player position: " + Position);

            if (_motionVector != Vector2.Zero)
            {
                IsMoving = true;

                var dt = (float) gameTime.ElapsedGameTime.TotalSeconds;

                Position += _motionVector*(Speed*dt);

                ComputeWallCollision(map);
            }
            else
            {
                IsMoving = false;
            }

            Sprite.IsAnimating = IsMoving;

            #region Moving action

            /*
            // If the human player wants to move
            if (motion != Vector2.Zero)
            {
                this.IsMoving = true;
                Sprite.IsAnimating = true;
                motion.Normalize();

                Vector2 nextPosition = Position + motion * Speed;
                Point nextPositionCell = Engine.VectorToCell(nextPosition, Sprite.Dimension);

                #region Moving of the player

                // We move the player
                Position += motion * Speed;

                // If the player want to go to top...
                if (motion.Y == -1)
                {
                    // ...  and that there is a wall
                    if (WallAt(new Point(CellPosition.X, CellPosition.Y - 1)))
                    {
                        // If he is more on the left side, we lag him to the left
                        if (IsMoreLeftSide() && !WallAt(new Point(CellPosition.X - 1, CellPosition.Y - 1)))
                        {
                            if (!WallAt(new Point(CellPosition.X - 1, CellPosition.Y)))
                                Position = new Vector2(Position.X - Speed, Position.Y);
                        }
                        else if (IsMoreRightSide() &&
                                 !WallAt(new Point(CellPosition.X + 1, CellPosition.Y - 1)))
                        {
                            if (!WallAt(new Point(CellPosition.X + 1, CellPosition.Y)))
                                Position = new Vector2(Position.X + Speed, Position.Y);
                        }
                    }
                    // ... and that there is no wall
                    else
                    {
                        // If he is more on the left side
                        if (IsMoreLeftSide())
                        {
                            Position = new Vector2(Position.X + Speed, Position.Y);
                        }
                        // If he is more on the right side
                        else if (IsMoreRightSide())
                        {
                            Position = new Vector2(Position.X - Speed, Position.Y);
                        }
                    }
                }
                // If the player want to go to bottom and that there is a wall
                else if (motion.Y == 1)
                {
                    // Wall at the bottom ?
                    if (WallAt(new Point(CellPosition.X, CellPosition.Y + 1)))
                    {
                        // If he is more on the left side, we lag him to the left
                        if (IsMoreLeftSide() && !WallAt(new Point(CellPosition.X - 1, CellPosition.Y + 1)))
                        {
                            if (!WallAt(new Point(CellPosition.X - 1, CellPosition.Y)))
                                Position = new Vector2(Position.X - Speed, Position.Y);
                        }
                        else if (IsMoreRightSide() &&
                                 !WallAt(new Point(CellPosition.X + 1, CellPosition.Y + 1)))
                        {
                            if (!WallAt(new Point(CellPosition.X + 1, CellPosition.Y)))
                                Position = new Vector2(Position.X + Speed, Position.Y);
                        }
                    }
                    else
                    {
                        // If he is more on the left side
                        if (IsMoreLeftSide())
                        {
                            Position = new Vector2(Position.X + Speed, Position.Y);
                        }
                        // If he is more on the right side
                        else if (IsMoreRightSide())
                        {
                            Position = new Vector2(Position.X - Speed, Position.Y);
                        }
                    }
                }
                // If the player want to go to left and that there is a wall
                else if (motion.X == -1)
                {
                    if (WallAt(new Point(CellPosition.X - 1, CellPosition.Y)))
                    {
                        // If he is more on the top side, we lag him to the top
                        if (IsMoreTopSide() && !WallAt(new Point(CellPosition.X - 1, CellPosition.Y - 1)))
                        {
                            if (!WallAt(new Point(CellPosition.X, CellPosition.Y - 1)))
                                Position = new Vector2(Position.X, Position.Y - Speed);
                        }
                        else if (IsMoreBottomSide() &&
                                 !WallAt(new Point(CellPosition.X - 1, CellPosition.Y + 1)))
                        {
                            if (!WallAt(new Point(CellPosition.X, CellPosition.Y + 1)))
                                Position = new Vector2(Position.X, Position.Y + Speed);
                        }
                    }
                    else
                    {
                        // If he is more on the top side, we lag him to the bottom
                        if (IsMoreTopSide())
                        {
                            Position = new Vector2(Position.X, Position.Y + Speed);
                        }
                        else if (IsMoreBottomSide())
                        {
                            Position = new Vector2(Position.X, Position.Y - Speed);
                        }
                    }
                }
                // If the player want to go to right and that there is a wall
                else if (motion.X == 1)
                {
                    if (WallAt(new Point(CellPosition.X + 1, CellPosition.Y)))
                    {
                        // If he is more on the top side, we lag him to the top
                        if (IsMoreTopSide() && !WallAt(new Point(CellPosition.X + 1, CellPosition.Y - 1)))
                        {
                            if (!WallAt(new Point(CellPosition.X, CellPosition.Y - 1)))
                                Position = new Vector2(Position.X, Position.Y - Speed);
                        }
                        else if (IsMoreBottomSide() &&
                                 !WallAt(new Point(CellPosition.X + 1, CellPosition.Y + 1)))
                        {
                            if (!WallAt(new Point(CellPosition.X, CellPosition.Y + 1)))
                                Position = new Vector2(Position.X, Position.Y + Speed);
                        }
                    }
                    else
                    {
                        // If he is more on the top side, we lag him to the top
                        if (IsMoreTopSide())
                        {
                            Position = new Vector2(Position.X, Position.Y + Speed);
                        }
                        else if (IsMoreBottomSide())
                        {
                            Position = new Vector2(Position.X, Position.Y - Speed);
                        }
                    }
                }
                #endregion

                //ComputeWallCollision();
            }
            else
            {
                this.IsMoving = false;
                Sprite.IsAnimating = false;
            }
            */

            #endregion

            SendMovement();

            //UpdatePlayerPosition();

            #region Bomb

            /*
            if ((HasBadItemEffect && BadItemEffect == BadItemEffect.BombDrop) ||
                ((Config.PlayersUsingController[Id] && InputHandler.ButtonDown(Buttons[4], PlayerIndex.One)) || InputHandler.KeyPressed(Keys[4]) &&
                (!HasBadItemEffect || (HasBadItemEffect && BadItemEffect != BadItemEffect.NoBomb))))
            {
                if (this.CurrentBombAmount > 0)
                {
                    var bo = FinalBomber.Instance.GamePlayScreen.BombList.Find(b => b.CellPosition == this.CellPosition);
                    if (bo == null)
                    {
                        this.CurrentBombAmount--;
                        var bomb = new Bomb(this.Id, CellPosition, this.BombPower, this.BombTimer, this.Speed);

                        FinalBomber.Instance.GamePlayScreen.AddBomb(bomb);
                    }
                }
            }
            */

            #endregion
        }

        public override void ApplyBadItem(BadItemEffect effect)
        {
            base.ApplyBadItem(effect);

            switch (effect)
            {
                case BadItemEffect.TooSlow:
                    SpeedSaved = Speed;
                    Speed = Config.MinSpeed;
                    break;
                case BadItemEffect.TooSpeed:
                    SpeedSaved = Speed;
                    Speed = Config.MaxSpeed;
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
                    Speed = SpeedSaved;
                    break;
                case BadItemEffect.TooSpeed:
                    Speed = SpeedSaved;
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
            if (Position.Y <= 0 || Position.Y >= (Config.MapSize.Y - 1)*Engine.TileHeight)
            {
                // If he wants to go to the left
                if (Position.X > 0 && InputHandler.KeyDown(Keys[2]))
                    Position = new Vector2(Position.X - Speed, Position.Y);
                    // If he wants to go to the right
                else if (Position.X < (Config.MapSize.X*Engine.TileWidth) - Engine.TileWidth &&
                         InputHandler.KeyDown(Keys[3]))
                    Position = new Vector2(Position.X + Speed, Position.Y);
            }
            // The player is either on the left either on the right
            if (Position.X <= 0 || Position.X >= (Config.MapSize.X - 1)*Engine.TileWidth)
            {
                // If he wants to go to the top
                if (Position.Y > 0 && InputHandler.KeyDown(Keys[0]))
                    Position = new Vector2(Position.X, Position.Y - Speed);
                    // If he wants to go to the bottom
                else if (Position.Y < (Config.MapSize.Y*Engine.TileHeight) - Engine.TileHeight &&
                         InputHandler.KeyDown(Keys[1]))
                    Position = new Vector2(Position.X, Position.Y + Speed);
            }

            if (Position.Y <= 0)
                Sprite.CurrentAnimation = AnimationKey.Down;
            else if (Position.Y >= (Config.MapSize.Y - 1)*Engine.TileHeight)
                Sprite.CurrentAnimation = AnimationKey.Up;
            else if (Position.X <= 0)
                Sprite.CurrentAnimation = AnimationKey.Right;
            else if (Position.X >= (Config.MapSize.X - 1)*Engine.TileWidth)
                Sprite.CurrentAnimation = AnimationKey.Left;

            #region Bombs => Edge gameplay

            if (InputHandler.KeyDown(Keys[4]) && CurrentBombAmount > 0)
            {
                // He can't put a bomb when he is on edges
                if (
                    !((CellPosition.Y == 0 &&
                       (CellPosition.X == 0 || CellPosition.X == Config.MapSize.X - 1)) ||
                      (CellPosition.Y == Config.MapSize.Y - 1 &&
                       (CellPosition.X == 0 || (CellPosition.X == Config.MapSize.X - 1)))))
                {
                    Map level =
                        FinalBomber.Instance.GamePlayScreen.World.Levels[
                            FinalBomber.Instance.GamePlayScreen.World.CurrentLevel];
                    int lag = 0;
                    Point bombPosition = CellPosition;
                    // Up
                    if (CellPosition.Y == 0)
                    {
                        while (CellPosition.Y + lag + 3 < Config.MapSize.Y &&
                               level.CollisionLayer[CellPosition.X, CellPosition.Y + lag + 3])
                        {
                            lag++;
                        }
                        bombPosition.Y = CellPosition.Y + lag + 3;
                        if (bombPosition.Y < Config.MapSize.Y)
                        {
                            var bomb = new Bomb(Id, bombPosition, BombPower, BombTimer, GameConfiguration.BaseBombSpeed + Speed);
                            level.CollisionLayer[bombPosition.X, bombPosition.Y] = true;
                            FinalBomber.Instance.GamePlayScreen.BombList.Add(bomb);
                            level.Board[bombPosition.X, bombPosition.Y] = bomb;
                            CurrentBombAmount--;
                        }
                    }
                    // Down
                    if (CellPosition.Y == Config.MapSize.Y - 1)
                    {
                        while (CellPosition.Y - lag - 3 >= 0 &&
                               level.CollisionLayer[CellPosition.X, CellPosition.Y - lag - 3])
                        {
                            lag++;
                        }
                        bombPosition.Y = CellPosition.Y - lag - 3;
                        if (bombPosition.Y >= 0)
                        {
                            var bomb = new Bomb(Id, bombPosition, BombPower, BombTimer, GameConfiguration.BaseBombSpeed + Speed);
                            level.CollisionLayer[bombPosition.X, bombPosition.Y] = true;
                            FinalBomber.Instance.GamePlayScreen.BombList.Add(bomb);
                            level.Board[bombPosition.X, bombPosition.Y] = bomb;
                            CurrentBombAmount--;
                        }
                    }
                    // Left
                    if (CellPosition.X == 0)
                    {
                        while (CellPosition.X + lag + 3 < Config.MapSize.X &&
                               level.CollisionLayer[CellPosition.X + lag + 3, CellPosition.Y])
                        {
                            lag++;
                        }
                        bombPosition.X = CellPosition.X + lag + 3;
                        if (bombPosition.X < Config.MapSize.X)
                        {
                            var bomb = new Bomb(Id, bombPosition, BombPower, BombTimer, GameConfiguration.BaseBombSpeed + Speed);
                            level.CollisionLayer[bombPosition.X, bombPosition.Y] = true;
                            FinalBomber.Instance.GamePlayScreen.BombList.Add(bomb);
                            level.Board[bombPosition.X, bombPosition.Y] = bomb;
                            CurrentBombAmount--;
                        }
                    }
                    // Right
                    if (CellPosition.X == Config.MapSize.X - 1)
                    {
                        while (CellPosition.X - lag - 3 >= 0 &&
                               level.CollisionLayer[CellPosition.X - lag - 3, CellPosition.Y])
                        {
                            lag++;
                        }
                        bombPosition.X = CellPosition.X - lag - 3;
                        if (bombPosition.X >= 0)
                        {
                            var bomb = new Bomb(Id, bombPosition, BombPower, BombTimer, GameConfiguration.BaseBombSpeed + Speed);
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

        private void SendMovement()
        {
            if (PreviousLookDirection != CurrentDirection)
            {
                switch (CurrentDirection)
                {
                    case LookDirection.Down:
                        Debug.Print("[Client]I want to go down !");
                        GameSettings.GameServer.SendMovement((byte) GameServer.SMT.MoveDown);
                        break;
                    case LookDirection.Left:
                        Debug.Print("[Client]I want to go left !");
                        GameSettings.GameServer.SendMovement((byte) GameServer.SMT.MoveLeft);
                        break;
                    case LookDirection.Right:
                        Debug.Print("[Client]I want to go right !");
                        GameSettings.GameServer.SendMovement((byte) GameServer.SMT.MoveRight);
                        break;
                    case LookDirection.Up:
                        Debug.Print("[Client]I want to go up !");
                        GameSettings.GameServer.SendMovement((byte) GameServer.SMT.MoveUp);
                        break;
                    default:
                        Debug.Print("[Client]I want to go stay here !");
                        GameSettings.GameServer.SendMovement((byte) GameServer.SMT.Standing);
                        break;
                }
            }
        }

        public override void ChangeLookDirection(byte newLookDirection)
        {
            base.ChangeLookDirection(newLookDirection);

            // TODO
            /*
            if (LookDirection != LookDirection.Idle)
                _movementConfirmed = true;
            else
                _movementConfirmed = false;
            */
        }
    }
}
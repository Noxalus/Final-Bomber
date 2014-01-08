using System;
using System.Diagnostics;
using FBLibrary;
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
        public float Ping;

        public OnlineHumanPlayer(int id)
            : base(id)
        {
            Initialize();
        }

        public OnlineHumanPlayer(int id, PlayerStats stats)
            : base(id, stats)
        {
            Initialize();
        }

        private void Initialize()
        {
            Keys = Config.PlayersKeys[Id];
            Buttons = Config.PlayersButtons[Id];
            _motionVector = Vector2.Zero;

            Ping = 0f;
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

                Position += _motionVector * GetMovementSpeed();
            }
            else
            {
                IsMoving = false;
            }

            Sprite.IsAnimating = IsMoving;

            SendMovement();

            //UpdatePlayerPosition();

            #region Bomb

            if ((HasBadEffect && BadEffect == BadEffect.BombDrop) ||
                ((Config.PlayersUsingController[Id] && InputHandler.ButtonDown(Buttons[4], PlayerIndex.One)) || InputHandler.KeyPressed(Keys[4]) &&
                (!HasBadEffect || (HasBadEffect && BadEffect != BadEffect.NoBomb))))
            {
                if (this.CurrentBombAmount > 0)
                {
                    var bo = NetworkTestScreen.GameManager.BombList.Find(b => b.CellPosition == this.CellPosition);
                    if (bo == null)
                    {
                        // Send to server that we want to plant a bomb
                        GameSettings.GameServer.SendBombPlacing();
                    }
                }
            }

            #endregion

            base.Move(gameTime, map, hazardMap);
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
                case BadEffect.KeysInversion:
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
                case BadEffect.KeysInversion:
                    break;
                case BadEffect.BombTimerChanged:
                    BombTimer = BombTimerSaved;
                    break;
            }
        }

        protected override void MoveFromEdgeWall()
        {
            base.MoveFromEdgeWall();
        }

        private void SendMovement()
        {
            if (PreviousDirection != CurrentDirection)
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
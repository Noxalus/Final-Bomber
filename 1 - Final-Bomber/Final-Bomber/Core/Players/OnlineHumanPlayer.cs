using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FBLibrary.Core;
using FBClient.Controls;
using FBClient.Network;
using FBClient.WorldEngine;
using FBLibrary.Network;
using Microsoft.Xna.Framework;

namespace FBClient.Core.Players
{
    public class OnlineHumanPlayer : BaseHumanPlayer
    {
        public float Ping = 0f;
        private Vector2 _initialPosition;
        private Vector2 _nextPosition;
        private bool _isInterpolating;
        private TimeSpan _interpolationTimer;

        private TimeSpan _movementInterpolationTime = TimeSpan.FromMilliseconds(25);

        public void SetNextPosition(Vector2 nextPosition)
        {
            _nextPosition = nextPosition;
        }


        public OnlineHumanPlayer(int id, int controlSettingsId)
            : base(id, controlSettingsId)
        {
            _initialPosition = Vector2.Zero;
            _nextPosition = Vector2.Zero;
            _isInterpolating = false;
            _interpolationTimer = TimeSpan.Zero;
        }

        public OnlineHumanPlayer(int id, int controlSettingsId, PlayerStats stats)
            : base(id, controlSettingsId, stats)
        {
        }

        protected override void Move(GameTime gameTime, Map map, int[,] hazardMap)
        {
            base.Move(gameTime, map, hazardMap);

            SendMovement();

            #region Movement interpolation
            // If a new position has been received from server
            if (_nextPosition != Vector2.Zero)
            {
                // If the position received from server is not reached yet
                if (Math.Abs(Position.X - _nextPosition.X) > 0.1f &&
                    Math.Abs(Position.Y - _nextPosition.Y) > 0.1f)
                {
                    // First time that we interpolate => save the initial position
                    if (!_isInterpolating)
                    {
                        _isInterpolating = true;
                        _initialPosition = Position;
                    }

                    _interpolationTimer += gameTime.ElapsedGameTime;

                    // We interpolate
                    float interpolationAmount = MathHelper.Clamp((float)_interpolationTimer.TotalMilliseconds / (float)_movementInterpolationTime.TotalMilliseconds, 0f, 1f);
                    PositionX = MathHelper.Lerp(_initialPosition.X, _nextPosition.X, interpolationAmount);
                    PositionY = MathHelper.Lerp(_initialPosition.Y, _nextPosition.Y, interpolationAmount);

                    if (_interpolationTimer >= _movementInterpolationTime)
                    {
                        _interpolationTimer = TimeSpan.Zero;
                        _nextPosition = Vector2.Zero;
                        _isInterpolating = false;
                    }
                }
            }
            #endregion

            #region Bomb

            if ((HasBadEffect && BadEffect == BadEffect.BombDrop) ||
                ((Config.PlayersUsingController[Id] && InputHandler.ButtonDown(Buttons[4], PlayerIndex.One)) || InputHandler.KeyPressed(Keys[4]) &&
                (!HasBadEffect || (HasBadEffect && BadEffect != BadEffect.NoBomb))))
            {
                if (this.CurrentBombAmount > 0)
                {
                    var bo = GameServer.Instance.GameManager.BombList.Find(b => b.CellPosition == this.CellPosition);
                    if (bo == null)
                    {
                        // Send to server that we want to plant a bomb
                        GameServer.Instance.SendBombPlacing();
                    }
                }
            }

            #endregion
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
                        GameServer.Instance.SendMovement((byte)MessageType.ClientMessage.MoveDown);
                        break;
                    case LookDirection.Left:
                        Debug.Print("[Client]I want to go left !");
                        GameServer.Instance.SendMovement((byte)MessageType.ClientMessage.MoveLeft);
                        break;
                    case LookDirection.Right:
                        Debug.Print("[Client]I want to go right !");
                        GameServer.Instance.SendMovement((byte)MessageType.ClientMessage.MoveRight);
                        break;
                    case LookDirection.Up:
                        Debug.Print("[Client]I want to go up !");
                        GameServer.Instance.SendMovement((byte)MessageType.ClientMessage.MoveUp);
                        break;
                    default:
                        Debug.Print("[Client]I want to go stay here !");
                        GameServer.Instance.SendMovement((byte)MessageType.ClientMessage.Standing);
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
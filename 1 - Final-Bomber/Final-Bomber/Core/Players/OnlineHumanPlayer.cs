using System;
using System.Diagnostics;
using FBLibrary;
using FBLibrary.Core;
using FBClient.Controls;
using FBClient.Network;
using FBClient.WorldEngine;
using Microsoft.Xna.Framework;

namespace FBClient.Core.Players
{
    public class OnlineHumanPlayer : BaseHumanPlayer
    {
        public float Ping = 0f;

        public OnlineHumanPlayer(int id)
            : base(id)
        {
        }

        public OnlineHumanPlayer(int id, PlayerStats stats)
            : base(id, stats)
        {
        }

        protected override void Move(GameTime gameTime, Map map, int[,] hazardMap)
        {
            base.Move(gameTime, map, hazardMap);

            SendMovement();

            //UpdatePlayerPosition();

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
                    int randomBombTimer = GameConfiguration.Random.Next(
                        GameConfiguration.BadItemTimerChangedMin,
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
                        GameServer.Instance.SendMovement((byte) GameServer.SMT.MoveDown);
                        break;
                    case LookDirection.Left:
                        Debug.Print("[Client]I want to go left !");
                        GameServer.Instance.SendMovement((byte) GameServer.SMT.MoveLeft);
                        break;
                    case LookDirection.Right:
                        Debug.Print("[Client]I want to go right !");
                        GameServer.Instance.SendMovement((byte) GameServer.SMT.MoveRight);
                        break;
                    case LookDirection.Up:
                        Debug.Print("[Client]I want to go up !");
                        GameServer.Instance.SendMovement((byte) GameServer.SMT.MoveUp);
                        break;
                    default:
                        Debug.Print("[Client]I want to go stay here !");
                        GameServer.Instance.SendMovement((byte) GameServer.SMT.Standing);
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
using System;
using FBLibrary;
using FBLibrary.Core;
using Final_Bomber.Controls;
using Final_Bomber.Core.Entities;
using Final_Bomber.WorldEngine;
using Microsoft.Xna.Framework;

namespace Final_Bomber.Core.Players
{
    public class HumanPlayer : BaseHumanPlayer
    {
        public HumanPlayer(int id)
            : base(id)
        {
        }

        public HumanPlayer(int id, PlayerStats stats)
            : base(id, stats)
        {
        }

        protected override void Move(GameTime gameTime, Map map, int[,] hazardMap)
        {
            base.Move(gameTime, map, hazardMap);

            #region Bomb

            if ((HasBadEffect && BadEffect == BadEffect.BombDrop) ||
                ((Config.PlayersUsingController[Id] && InputHandler.ButtonDown(Buttons[4], PlayerIndex.One)) || InputHandler.KeyPressed(Keys[4]) &&
                (!HasBadEffect || (HasBadEffect && BadEffect != BadEffect.NoBomb))))
            {
                if (this.CurrentBombAmount > 0)
                {
                    var bo = GameManager.BombList.Find(b => b.CellPosition == this.CellPosition);
                    if (bo == null)
                    {
                        // Plant a new bomb
                        var bomb = new Bomb(Id, CellPosition, BombPower, BombTimer, Speed);
                        CurrentBombAmount--;
                        bomb.Initialize(GameManager.CurrentMap, GameManager.HazardMap);

                        GameManager.AddBomb(bomb);
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

        private void Move()
        {
            if (PreviousDirection != CurrentDirection)
            {
                var newPosition = Position;

                switch (CurrentDirection)
                {
                    case LookDirection.Down:
                        newPosition.Y++;
                        break;
                    case LookDirection.Left:
                        newPosition.X--;
                        break;
                    case LookDirection.Right:
                        newPosition.X++;
                        break;
                    case LookDirection.Up:
                        newPosition.Y--;
                        break;
                    default:
                        break;
                }

                Position = newPosition;
            }
        }
    }
}
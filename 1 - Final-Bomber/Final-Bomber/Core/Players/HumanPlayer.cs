using FBLibrary.Core;
using FBClient.Controls;
using FBClient.Core.Entities;
using FBClient.WorldEngine;
using Microsoft.Xna.Framework;

namespace FBClient.Core.Players
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
                        bomb.Initialize(GameManager.CurrentMap.Size, GameManager.CurrentMap.CollisionLayer, GameManager.HazardMap);

                        GameManager.AddBomb(bomb);
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
    }
}
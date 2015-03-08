using FBLibrary.Core;
using FBClient.Controls;
using FBClient.Core.Entities;
using FBClient.WorldEngine;
using Microsoft.Xna.Framework;

namespace FBClient.Core.Players
{
    public class HumanPlayer : BaseHumanPlayer
    {
        public HumanPlayer(int id, int controlSettingsId)
            : base(id, controlSettingsId)
        {
        }

        public HumanPlayer(int id, int controlSettingsId, PlayerStats stats)
            : base(id, controlSettingsId, stats)
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
                // Do we still have a bomb to plant?
                if (CurrentBombAmount <= 0) return;

                // Is there already a bomb here?
                var bo = GameManager.BombList.Find(b => b.CellPosition == CellPosition);
                if (bo != null) return;

                // Plant a new bomb
                var bomb = new Bomb(Id, CellPosition, BombPower, BombTimer, Speed);
                CurrentBombAmount--;
                bomb.Initialize(GameManager.CurrentMap.Size, GameManager.CurrentMap.CollisionLayer, GameManager.HazardMap);

                GameManager.AddBomb(bomb);
            }

            #endregion
        }

        protected override void RemoveBadItem()
        {
            base.RemoveBadItem();
        }
    }
}
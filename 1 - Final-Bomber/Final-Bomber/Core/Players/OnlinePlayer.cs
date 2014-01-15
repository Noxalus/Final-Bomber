using FBLibrary.Core;
using FBClient.Entities;
using FBClient.Sprites;
using FBClient.WorldEngine;
using Microsoft.Xna.Framework;

namespace FBClient.Core.Players
{
    internal class OnlinePlayer : Player
    {
        public OnlinePlayer(int id)
            : base(id)
        {
        }

        public override void Update(GameTime gameTime, Map map, int[,] hazardMap)
        {
            base.Update(gameTime, map, hazardMap);
        }

        protected override void Move(GameTime gameTime, Map map, int[,] hazardMap)
        {
            Vector2 motionVector = Vector2.Zero;

            switch (CurrentDirection)
            {
                case LookDirection.Down:
                    Sprite.CurrentAnimation = AnimationKey.Down;
                    motionVector.Y = 1;
                    break;
                case LookDirection.Left:
                    Sprite.CurrentAnimation = AnimationKey.Left;
                    motionVector.X = -1;
                    break;
                case LookDirection.Right:
                    Sprite.CurrentAnimation = AnimationKey.Right;
                    motionVector.X = 1;
                    break;
                case LookDirection.Up:
                    Sprite.CurrentAnimation = AnimationKey.Up;
                    motionVector.Y = -1;
                    break;
            }

            if (motionVector != Vector2.Zero)
            {
                IsMoving = true;
                Sprite.IsAnimating = true;
                Position += motionVector * GetMovementSpeed();
            }
            else
            {
                Sprite.IsAnimating = false;
            }

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
                        var bomb = new Bomb(this.Id, CellPosition, this.Power, this.BombTimer, this.Speed);

                        FinalBomber.Instance.GamePlayScreen.AddBomb(bomb);
                    }
                }
            }
            */

            #endregion
            
            base.Move(gameTime, map, hazardMap);
        }
    }
}
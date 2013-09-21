using System.Diagnostics;
using FBLibrary.Core;
using Final_Bomber.Entities;
using Final_Bomber.Sprites;
using Final_Bomber.WorldEngine;
using Microsoft.Xna.Framework;

namespace Final_Bomber.Core.Players
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
            Debug.Print("Look Direction: " + CurrentDirection);
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
                var dt = (float) gameTime.ElapsedGameTime.TotalSeconds;
                IsMoving = true;
                Sprite.IsAnimating = true;
                Position += motionVector*(Speed*dt);
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
using Final_Bomber.Entities;
using Final_Bomber.Controls;
using Final_Bomber.Screens;
using Final_Bomber.Sprites;
using Final_Bomber.TileEngine;
using Final_Bomber.WorldEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Final_Bomber.Network.Core
{
    class OnlinePlayer : Player
    {

        private LookDirection _oldLookDirection;

        public OnlinePlayer(int id)
            : base(id)
        {
            _oldLookDirection = LookDirection.Idle;
        }

        public override void Update(GameTime gameTime)
        {


            base.Update(gameTime);
        }

        protected override void Move()
        {
            #region Moving input

            _oldLookDirection = LookDirection;

            /*
            // Up
            if ((Config.PlayersUsingController[Id] && InputHandler.ButtonDown(Buttons[0], PlayerIndex.One)) || InputHandler.KeyDown(Keys[0]))
            {
                Sprite.CurrentAnimation = AnimationKey.Up;
                LookDirection = LookDirection.Up;
            }
            // Down
            else if ((Config.PlayersUsingController[Id] && InputHandler.ButtonDown(Buttons[1], PlayerIndex.One)) || InputHandler.KeyDown(Keys[1]))
            {
                Sprite.CurrentAnimation = AnimationKey.Down;
                LookDirection = LookDirection.Down;
            }
            // Left
            else if ((Config.PlayersUsingController[Id] && InputHandler.ButtonDown(Buttons[2], PlayerIndex.One)) || InputHandler.KeyDown(Keys[2]))
            {
                Sprite.CurrentAnimation = AnimationKey.Left;
                LookDirection = LookDirection.Left;
            }
            // Right
            else if ((Config.PlayersUsingController[Id] && InputHandler.ButtonDown(Buttons[3], PlayerIndex.One)) || InputHandler.KeyDown(Keys[3]))
            {
                Sprite.CurrentAnimation = AnimationKey.Right;
                LookDirection = LookDirection.Right;
            }
            else
            {
                LookDirection = LookDirection.Idle;
            }
            */
            #endregion

            #region Moving action

            /*
            // If the human player wants to move
            if (motion != Vector2.Zero)
            {
                this.IsMoving = true;
                Sprite.IsAnimating = true;
                motion.Normalize();

                Vector2 nextPosition = Sprite.Position + motion * Sprite.Speed;
                Point nextPositionCell = Engine.VectorToCell(nextPosition, Sprite.Dimension);

                #region Moving of the player

                // We move the player
                Sprite.Position += motion * Sprite.Speed;

                // If the player want to go to top...
                if (motion.Y == -1)
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

                //ComputeWallCollision();
            }
            else
            {
                this.IsMoving = false;
                Sprite.IsAnimating = false;
            }
            */

            MovePlayer();
            //UpdatePlayerPosition();

            #region Bomb
            /*
            if ((HasBadItemEffect && BadItemEffect == BadItemEffect.BombDrop) ||
                ((Config.PlayersUsingController[Id] && InputHandler.ButtonDown(Buttons[4], PlayerIndex.One)) || InputHandler.KeyPressed(Keys[4]) &&
                (!HasBadItemEffect || (HasBadItemEffect && BadItemEffect != BadItemEffect.NoBomb))))
            {
                if (this.CurrentBombNumber > 0)
                {
                    var bo = FinalBomber.Instance.GamePlayScreen.BombList.Find(b => b.Sprite.CellPosition == this.Sprite.CellPosition);
                    if (bo == null)
                    {
                        this.CurrentBombNumber--;
                        var bomb = new Bomb(FinalBomber.Instance, this.Id, Sprite.CellPosition, this.Power, this.BombTimer, this.Sprite.Speed);

                        FinalBomber.Instance.GamePlayScreen.AddBomb(bomb);
                    }
                }
            }
            */
            #endregion

            #endregion
        }

        public void MovePlayer()
        {
            /*
            switch (movementAction)
            {
                case ActionEnum.WalkingDown:
                    Speed = new Vector2(0, GetMovementSpeed());
                    break;
                case ActionEnum.WalkingUp:
                    Speed = new Vector2(0, -GetMovementSpeed());
                    break;
                case ActionEnum.WalkingLeft:
                    Speed = new Vector2(-GetMovementSpeed(), 0);
                    break;
                case ActionEnum.WalkingRight:
                    Speed = new Vector2(GetMovementSpeed(), 0);
                    break;
                case ActionEnum.Standing:
                    Speed = new Vector2(0);
                    break;
            }
            */
        }

        private float GetMovementSpeed()
        {
            float rtn = (/*(float)MoveSpeed * */(float)GameSettings.speed) / 1000;
            return rtn;
        }
    }
}

using FBLibrary.Core;
using Final_Bomber.Controls;
using Final_Bomber.Entities;
using Final_Bomber.Sprites;
using Final_Bomber.WorldEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Final_Bomber.Core.Players
{
    public abstract class BaseHumanPlayer : Player
    {
        protected GameManager GameManager;

        protected Keys[] KeysSaved;
        protected Vector2 MotionVector;

        protected Keys[] Keys { get; private set; }
        protected Buttons[] Buttons { get; private set; }

        protected BaseHumanPlayer(int id) : base(id)
        {
            Initialize();
        }

        protected BaseHumanPlayer(int id, PlayerStats stats)
            : base(id, stats)
        {
            Initialize();
        }

        private void Initialize()
        {
            Keys = Config.PlayersKeys[Id];
            Buttons = Config.PlayersButtons[Id];
            MotionVector = Vector2.Zero;
        }

        public void SetGameManager(GameManager gameManager)
        {
            GameManager = gameManager;
        }

        protected override void Move(GameTime gameTime, Map map, int[,] hazardMap)
        {
            #region Moving input

            MotionVector = Vector2.Zero;

            // Up
            if ((Config.PlayersUsingController[Id] && InputHandler.ButtonDown(Buttons[0], PlayerIndex.One)) ||
                InputHandler.KeyDown(Keys[0]))
            {
                Sprite.CurrentAnimation = AnimationKey.Up;
                CurrentDirection = LookDirection.Up;
                MotionVector.Y = -1;
            }
            // Down
            else if ((Config.PlayersUsingController[Id] && InputHandler.ButtonDown(Buttons[1], PlayerIndex.One)) ||
                     InputHandler.KeyDown(Keys[1]))
            {
                Sprite.CurrentAnimation = AnimationKey.Down;
                CurrentDirection = LookDirection.Down;
                MotionVector.Y = 1;
            }
            // Left
            else if ((Config.PlayersUsingController[Id] && InputHandler.ButtonDown(Buttons[2], PlayerIndex.One)) ||
                     InputHandler.KeyDown(Keys[2]))
            {
                Sprite.CurrentAnimation = AnimationKey.Left;
                CurrentDirection = LookDirection.Left;
                MotionVector.X = -1;
            }
            // Right
            else if ((Config.PlayersUsingController[Id] && InputHandler.ButtonDown(Buttons[3], PlayerIndex.One)) ||
                     InputHandler.KeyDown(Keys[3]))
            {
                Sprite.CurrentAnimation = AnimationKey.Right;
                CurrentDirection = LookDirection.Right;
                MotionVector.X = 1;
            }
            else
            {
                CurrentDirection = LookDirection.Idle;
            }

            #endregion

            if (MotionVector != Vector2.Zero)
            {
                IsMoving = true;

                Position += MotionVector * GetMovementSpeed();
            }
            else
            {
                IsMoving = false;
            }

            Sprite.IsAnimating = IsMoving;

            base.Move(gameTime, map, hazardMap);
        }
    }
}

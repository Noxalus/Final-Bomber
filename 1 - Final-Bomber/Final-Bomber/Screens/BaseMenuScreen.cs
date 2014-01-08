using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Final_Bomber.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Final_Bomber.Screens
{
    public abstract class BaseMenuScreen : BaseGameState
    {
        protected string[] MenuString;
        protected int IndexMenu;
        protected Vector2 MenuPosition;

        protected BaseMenuScreen(Game game, GameStateManager manager)
            : base(game, manager)
        {
            IndexMenu = 0;
        }

        public override void Initialize()
        {
            MenuPosition = new Vector2(Config.Resolutions[Config.IndexResolution, 0] / 2f, Config.Resolutions[Config.IndexResolution, 1] / 2f);

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (InputHandler.KeyPressed(Keys.Up))
            {
                if (IndexMenu <= 0)
                    IndexMenu = MenuString.Length - 1;
                else
                    IndexMenu--;
            }
            else if (InputHandler.KeyPressed(Keys.Down))
            {
                IndexMenu = (IndexMenu + 1) % MenuString.Length;
            }

            base.Update(gameTime);
        }

    }
}

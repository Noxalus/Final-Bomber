using System;
using System.Collections.Generic;
using Final_Bomber.Components.AI;
using Final_Bomber.Screens;
using Final_Bomber.Sprites;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Final_Bomber.Controls;
using Final_Bomber.TileEngine;
using Final_Bomber.WorldEngine;
using Microsoft.Xna.Framework.Input;

namespace Final_Bomber.Components
{
    public abstract class OnlinePlayer : Player
    {
        public OnlinePlayer(int id, FinalBomber game, Vector2 position)
            : base(id, game, position)
        {
        }
    }
}

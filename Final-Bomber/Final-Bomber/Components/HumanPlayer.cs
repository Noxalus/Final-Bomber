using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Final_Bomber.Components
{
    class HumanPlayer : Player
    {
        public HumanPlayer(int id, FinalBomber game, Vector2 position) : base(id, game, position)
        {
        }
    }
}

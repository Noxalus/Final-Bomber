using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Final_Bomber.Components.ArtificialIntelligence
{
    class AIPlayer : Player
    {
        public AIPlayer(int id, FinalBomber game, Vector2 position) : base(id, game, position)
        {
        }
    }
}

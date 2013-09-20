using Final_Bomber.Entities;
using Final_Bomber.WorldEngine;
using Microsoft.Xna.Framework;

namespace Final_Bomber.Core
{
    public abstract class DynamicEntity : Entity
    {
        public abstract void Update(GameTime gameTime, Map map, int[,] hazardMap);
    }
}

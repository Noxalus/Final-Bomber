using Final_Bomber.Entities;
using Microsoft.Xna.Framework;

namespace Final_Bomber.Core
{
    public abstract class StaticEntity : Entity
    {
        public abstract void Update(GameTime gameTime);
    }
}
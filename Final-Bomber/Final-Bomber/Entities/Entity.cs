using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Final_Bomber.Sprites;
using Microsoft.Xna.Framework;

namespace Final_Bomber.Entities
{
    public abstract class Entity
    {
        public abstract AnimatedSprite Sprite { get; protected set; }

        public abstract void Update(GameTime gameTime);
        public abstract void Draw(GameTime gameTime);

        public abstract void Destroy();
        public abstract void Remove();
    }

    public class EntityCollection : List<Entity>
    {
    }
}

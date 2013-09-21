using Final_Bomber.Sprites;

namespace Final_Bomber.Entities
{
    public abstract class Entity
    {
        public abstract AnimatedSprite Sprite { get; protected set; }

        public abstract void Destroy();
        public abstract void Remove();
    }
}
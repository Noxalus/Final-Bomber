using Final_Bomber.Sprites;

namespace Final_Bomber.Entities
{
    public abstract class Entity
    {
        public enum Type
        {
            Void = 0,
            UnbreakableWall = 1,
            EdgeWall = 2,
            Wall = 3,
            Player = 4,
            Teleporter = 6,
            Arrow = 7
        };

        public abstract AnimatedSprite Sprite { get; protected set; }

        public abstract void Destroy();
        public abstract void Remove();
    }
}
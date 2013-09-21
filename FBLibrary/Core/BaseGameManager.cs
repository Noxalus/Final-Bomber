using System;
using System.Collections.Generic;
using FBLibrary.Core.BaseEntities;
using Microsoft.Xna.Framework;

namespace FBLibrary.Core
{
    public abstract class BaseGameManager
    {
        // Engine
        protected Engine Engine;

        public Random Random;

        protected BaseGameManager()
        {
            Random = new Random();

            // Engine
            Engine = new Engine(32, 32, Vector2.Zero);
        }
    }
}

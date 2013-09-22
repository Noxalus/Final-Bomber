using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using FBLibrary.Core.BaseEntities;
using Microsoft.Xna.Framework;

namespace FBLibrary.Core
{
    public abstract class BaseGameManager
    {
        // Engine
        protected Engine Engine;

        public int[,] HazardMap;

        public Random Random;

        protected BaseGameManager()
        {
            Random = new Random(Environment.TickCount);

            // Engine
            Engine = new Engine(32, 32, Vector2.Zero);
        }
    }
}

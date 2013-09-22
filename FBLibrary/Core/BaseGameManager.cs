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

        public Random Random;

        public Dictionary<string, string> MapDictionary;

        protected BaseGameManager()
        {
            Random = new Random(Environment.TickCount);

            // Engine
            Engine = new Engine(32, 32, Vector2.Zero);

            MapDictionary = new Dictionary<string, string>();
            LoadMaps();
        }

        public void LoadMaps()
        {
            MapDictionary.Clear();

            using (var md5 = MD5.Create())
            {
                //Get all the map files
                foreach (var source in Directory.GetFiles(@"Content\Maps\", "*.map", SearchOption.AllDirectories))
                {
                    using (var stream = File.OpenRead(source))
                    {
                        // Compute the MD5 hash of the file
                        string md5Hash = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
                        string fileName = Path.GetFileName(source);

                        if (fileName != null)
                            MapDictionary.Add(fileName, md5Hash);
                    }
                }
            }
        }

    }
}

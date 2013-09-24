using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using FBLibrary.Core;
using FBLibrary.Core.BaseEntities;
using Final_BomberServer.Core.Entities;
using Microsoft.Xna.Framework;

namespace Final_BomberServer.Core.WorldEngine
{
    public class Map : BaseMap
    {
        private List<EdgeWall> _edgeWallList;
        private List<UnbreakableWall> _unbreakableWallList;
        private List<Teleporter> _teleporterList;
        private List<Arrow> _arrowList;

        public Map()
        {
            _edgeWallList = new List<EdgeWall>();
            _unbreakableWallList = new List<UnbreakableWall>();
            _teleporterList = new List<Teleporter>();
            _arrowList = new List<Arrow>();
        }

        protected override void AddUnbreakableWall(Point position)
        {
            var unbreakableWall = new UnbreakableWall(position);
            _unbreakableWallList.Add(unbreakableWall);

            base.AddUnbreakableWall(unbreakableWall);
        }

        protected override void AddEdgeWall(Point position)
        {
            var edgeWall = new EdgeWall(position);
            _edgeWallList.Add(edgeWall);

            base.AddEdgeWall(edgeWall);
        }

        #region Displaying region
        public void DisplayBoard()
        {

            for (int y = 0; y < Size.Y; y++)
            {
                for (int x = 0; x < Size.X; x++)
                {
                    if (Board[x, y] is EdgeWall)
                    {
                        Console.Write("E ");
                    }
                    else if (Board[x, y] is UnbreakableWall)
                    {
                        Console.Write("U ");
                    }
                    else if (Board[x, y] is Wall)
                    {
                        Console.Write("W ");
                    }
                    else if (Board[x, y] is Player)
                    {
                        Console.Write("P ");
                    }
                    else
                    {
                        Console.Write("N ");
                    }
                }

                Console.WriteLine();
            }
        }

        public void DisplayCollisionLayer()
        {
            for (int y = 0; y < Size.Y; y++)
            {
                for (int x = 0; x < Size.X; x++)
                {
                    Console.Write(CollisionLayer[x, y] ? "1 " : "0 ");
                }

                Console.WriteLine();
            }
        }
        #endregion
    }
}

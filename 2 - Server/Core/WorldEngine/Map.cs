using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using FBLibrary.Core;
using FBLibrary.Core.BaseEntities;
using FBServer.Core.Entities;
using Microsoft.Xna.Framework;

namespace FBServer.Core.WorldEngine
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
    }
}

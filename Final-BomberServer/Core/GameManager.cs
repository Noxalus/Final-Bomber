using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FBLibrary.Core;
using Final_BomberServer.Core.Entities;

namespace Final_BomberServer.Core
{
    class GameManager : BaseGameManager
    {
        // Collections
        private readonly List<Bomb> _bombList;
        private readonly List<PowerUp> _powerUpList;
        private readonly List<Wall> _wallList;

        #region Properties

        public List<Wall> WallList
        {
            get { return _wallList; }
        }

        #endregion

        public GameManager()
            : base()
        {
            _wallList = new List<Wall>();
            _powerUpList = new List<PowerUp>();
            _bombList = new List<Bomb>();
        }
    }
}

using FBServer.Core.WorldEngine;
using FBServer.Host;
using System.Collections.Generic;
using System.Diagnostics;

namespace FBServer.Core
{
    partial class GameServerHandler
    {
        HostGame _game;
        public bool Running = false;

        public void Initialize()
        {
            _game = new HostGame();

            Running = true;
        }

        public void Update()
        {
            if (GameServer.Instance.GameManager.HasStarted)
                _game.Update();
            else
                _game.Initialize();
        }

        public void Dispose()
        {
            if (GameServer.Instance.HostStarted)
            {
                GameServer.Instance.EndServer("Bye bye !");
            }

            //MainServer.EndMainConnection("Bye bye !");

            Running = false;
        }
    }
}

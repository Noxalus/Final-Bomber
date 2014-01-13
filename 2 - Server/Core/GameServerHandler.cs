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
        Stopwatch _speedTimer;

        public void Initialize()
        {
            _speedTimer = new Stopwatch();
            _game = new HostGame();

            Running = true;

            _speedTimer.Start();
        }

        public void Update()
        {
            // This calculates tps, so that the old player's movement is synchronized with the client
            GameSettings.Speed = _speedTimer.Elapsed.Milliseconds;

            _speedTimer.Restart();

            if (_game.HasStarted)
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

            GameSettings.CurrentMap = 0;
            GameSettings.mapPlayList = new List<Map>();
        }
    }
}

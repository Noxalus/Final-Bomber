using System.Threading;
using Final_BomberServer.Host;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final_BomberServer.Core
{
    partial class GameServerHandler
    {
        HostGame game;
        public bool Running = false;
        Stopwatch speedTmr;

        public void Initialize()
        {
            speedTmr = new Stopwatch();
            game = new HostGame();

            speedTmr.Start();
            Running = true;
        }

        public void Update()
        {
            // This calculates tps, so that the old player's movement is synchronized with the client
            GameSettings.speed = speedTmr.ElapsedMilliseconds;

            double test = (double)speedTmr.ElapsedTicks / (double)Stopwatch.Frequency;
            Console.WriteLine(speedTmr.Elapsed.Ticks + "|" + speedTmr.ElapsedTicks + "|" + test);
            speedTmr.Reset();
            speedTmr.Start();

            //Thread.Sleep(10);

            ProgramStepProccesing();

            if (game.HasStarted)
                game.Update();
        }

        public void Dispose()
        {
            if (GameSettings.gameServer.HostStarted)
            {
                GameSettings.gameServer.EndServer("byebye");
                GameSettings.gameServer = null;
            }
            //MainServer.EndMainConnection("byebye");
            Running = false;
            GameSettings.CurrentMap = 0;
            GameSettings.mapPlayList = new List<Map>();
        }
    }
}

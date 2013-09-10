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
            GameSettings.speed = (long)(((double) speedTmr.ElapsedTicks/(double) Stopwatch.Frequency) * 1000); //speedTmr.ElapsedMilliseconds;

            //Console.WriteLine(speedTmr.Elapsed.Ticks + "|" + speedTmr.ElapsedTicks + "|" + test);
            /*
            // take the ElapsedTicks, then divide by Frequency to get seconds
            Console.WriteLine("ElapsedTicks to sec:  {0}",
                speedTmr.ElapsedTicks / (double)Stopwatch.Frequency);

            // take the Elapsed property, and query total number of seconds it represents
            Console.WriteLine("Elapsed.TotalSeconds: {0}", speedTmr.Elapsed.TotalSeconds);
            */

            //Console.WriteLine("Speed: " + GameSettings.speed);

            speedTmr.Reset();
            speedTmr.Start();

            Thread.Sleep(10);

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

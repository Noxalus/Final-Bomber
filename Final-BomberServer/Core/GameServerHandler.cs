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
            /*
            try
            {
                MainServer.StartMainConnection();
            }
            catch (TypeInitializationException)
            {
                Console.WriteLine("Couldn't connect to the internet, please check your connection");
            }
            */
            //t_couldntConnect.Start();
            speedTmr.Start();
            Running = true;
        }

        public void Update()
        {
            GameSettings.speed = speedTmr.ElapsedMilliseconds; //Detta räknar ut fps, så att gubbens rörelse blir synkade med clienterna
            speedTmr.Reset();
            speedTmr.Start();

            ProgramStepProccesing();
            /*
            if (MainServer.HasStarted)
                MainServer.RunMainConnection();
            */
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

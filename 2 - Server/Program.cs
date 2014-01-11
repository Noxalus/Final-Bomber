using System.Diagnostics;
using FBLibrary;
using FBServer.Core;
using System;
using System.Threading;
using FBServer.Core.WorldEngine;
using log4net;
using log4net.Config;

namespace FBServer
{
    class Program
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            // For log
            XmlConfigurator.Configure();

            // We get all map files to store name + md5 checksum in a dictionary 
            MapLoader.LoadMapFiles();

            if (args.Length > 0)
            {
                Program.Log.Info("Arguments:");
                foreach (string s in args)
                {
                    Program.Log.Info(s);
                }
            }

            var server = new GameServerHandler();

            if (!server.Running)
            {
                GameSettings.GameName = "Final-Server";
                Log.Info("Player Number: " + GameConfiguration.PlayerNumber);

                server = new GameServerHandler();
                server.Initialize();

                var timer = new Stopwatch();
                timer.Start();
                while(server.Running)
                {
                    // Delta time
                    GameConfiguration.DeltaTime = timer.Elapsed.Ticks;

                    server.Update();
                    //Program.Log.Info(timer.Elapsed.Milliseconds);
                    timer.Restart();
                    Thread.Sleep(15);
                }
            }
        }
    }
}

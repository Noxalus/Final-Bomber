using System;
using System.Diagnostics;
using FBLibrary;
using FBServer.Core;
using System.Threading;
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
                // Arguments to create a server with specific configuration 
                // (max player number, score to win, etc...)
                Log.Info("Arguments:");
                foreach (var s in args)
                {
                    Log.Info(s);
                }
            }

            var server = new GameServerHandler();

            if (!server.Running)
            {
                Log.Info(string.Format("[CONFIG] Player Number: {0}", GameConfiguration.PlayerNumber));

                server = new GameServerHandler();
                server.Initialize();

                var timer = new Stopwatch();
                timer.Start();
                TimeSpan timerTest = TimeSpan.Zero;
                TimeSpan timerForTimePerSecond = TimeSpan.Zero;
                int timePerSecond = 0;
                while(server.Running)
                {
                    server.Update();
                    
                    // Debug time :)
                    /*
                    timerTest += TimeSpan.FromTicks(GameConfiguration.DeltaTime);
                    Console.WriteLine("Timer test: " + timerTest);
                    
                    timerForTimePerSecond += TimeSpan.FromTicks(GameConfiguration.DeltaTime);

                    timePerSecond++;
                    if (timerForTimePerSecond.TotalSeconds > 1)
                    {
                        timerForTimePerSecond = TimeSpan.Zero;
                        Console.WriteLine("Framerate: " + timePerSecond);
                        timePerSecond = 0;
                    }
                    */
                    // Compute delta time
                    GameConfiguration.DeltaTime = timer.Elapsed.Ticks;

                    timer.Restart();

                    // Max 15 milliseconds between 2 main loop
                    if (timer.Elapsed.Milliseconds < ServerSettings.MaxDeltaTime)
                        Thread.Sleep(ServerSettings.MaxDeltaTime - timer.Elapsed.Milliseconds);
                    else
                        Thread.Sleep(ServerSettings.MaxDeltaTime);
                }
            }
        }
    }
}

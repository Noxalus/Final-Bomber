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
                // Arguments like max client number (ex: 32)
                Log.Info("Arguments:");
                foreach (string s in args)
                {
                    Log.Info(s);
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
                    // Compute delta time
                    GameConfiguration.DeltaTime = timer.Elapsed.Ticks;

                    server.Update();

                    timer.Restart();

                    // Max 15 milliseconds between 2 main loop
                    Thread.Sleep(15);
                }
            }
        }
    }
}

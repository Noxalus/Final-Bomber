using System.Diagnostics;
using Final_BomberServer.Core;
using System;
using System.Threading;
using log4net;
using log4net.Config;

namespace Final_BomberServer
{
    class Program
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            XmlConfigurator.Configure();

            if (args.Length > 0)
            {
                Console.WriteLine("Arguments:");
                foreach (string s in args)
                {
                    Console.WriteLine(s);
                }
            }

            var server = new GameServerHandler();

            if (!server.Running)
            {
                GameSettings.GameName = "Final-Server";

                server = new GameServerHandler();
                server.Initialize();

                var timer = new Stopwatch();
                timer.Start();
                while(server.Running)
                {
                    server.Update();
                    //Console.WriteLine(timer.Elapsed.Milliseconds);
                    timer.Restart();
                    Thread.Sleep(15);
                }
            }
        }
    }
}

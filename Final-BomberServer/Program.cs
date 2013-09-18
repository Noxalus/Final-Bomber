using System.Diagnostics;
using Final_BomberServer.Core;
using System;
using System.Threading;

namespace Final_BomberServer
{
    class Program
    {
        static void Main(string[] args)
        {
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
                    //Console.WriteLine(timer.Elapsed.Ticks);
                    timer.Restart();
                    Thread.Sleep(15);
                }
            }
        }
    }
}

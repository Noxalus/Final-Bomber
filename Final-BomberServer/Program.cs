using Final_BomberServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            GameServerHandler server = new GameServerHandler();

            if (!server.Running)
            {
                GameSettings.GameName = "Final-Server";

                server = new GameServerHandler();
                server.Initialize();

                while(server.Running)
                {
                    server.Update();
                }
            }

            Console.WriteLine("COUCOU");
            Console.ReadKey();
        }
    }
}

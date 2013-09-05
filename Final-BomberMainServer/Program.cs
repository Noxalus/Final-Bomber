using Final_BomberNetwork.GameServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final_BomberMainServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server();
            server.StartServer();
            server.RunServer();

            Console.ReadLine();
        }
    }
}

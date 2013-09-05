using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final_BomberServer.Core
{
    partial class GameServerHandler
    {
        //H.U.D.Timer t_couldntConnect = new H.U.D.Timer(false);
        bool disconnected = false;

        private void ProgramStepProccesing()
        {
            /*
            if (!MainServer.Connected && !MainServer.Disconnected && t_couldntConnect.Each(5000))
            {
                Console.WriteLine("Couldn't connect to main server");
            }
            if (MainServer.Disconnected && !disconnected) // kollar om man blev disconnectad och kör koden en gång
            {
                disconnected = true;
                game.Dispose();
                Console.WriteLine("Main Server has closed");
            }
            if (MainServer.Connected) // Om man har connectat
            {*/
                ConnectedGameProccesing(); // kör kod om man har connectat
            //}
        }

        bool sendHosting = false;

        private void ConnectedGameProccesing()
        {
            if (!sendHosting)
            {
                //MainServer.SendHosting(); // Säger till servern att den tänker hosta en bana
                sendHosting = true;
            }
            if (!game.HasStarted && GameSettings.GameValues.RecievedValues) //Kollar om den har fått informationen om spelet, och startar därefter servern
            {
                GameSettings.GameValues.RecievedValues = false;
                game.Initialize();
            }
        }
    }
}

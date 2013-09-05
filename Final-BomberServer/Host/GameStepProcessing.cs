using Final_BomberServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final_BomberServer.Host
{
    partial class HostGame
    {
        List<Player> alivePlayers;
        private void GameStepProccesing()
        {
            if (GameSettings.gameServer.clients.Count == GameSettings.GetCurrentMap().playerAmount
                && !StartedMatch && GameSettings.gameServer.clients.IsClientsReady())
            //Om rätt antal spelare har joinat, matchen har inte startat och alla spelare är färdiga att starta 
            {
                GameInitialize();
            }

            foreach (Client client in GameSettings.gameServer.clients)
            {
                if (client.NewClient && StartedMatch && client.isReady)
                {
                    GameSettings.gameServer.SendStartGame(client, true);
                    GameSettings.gameServer.SendPlayersToNew(client);
                    client.NewClient = false;
                }
            }

            alivePlayers = GameSettings.gameServer.clients.GetAlivePlayers();
            if (StartedMatch && alivePlayers.Count < 2)
            {
                //MainServer.SendPlayerStats();
                GameSettings.CurrentMap++;
                //MainServer.SendNextMap();
                EndGame();
                foreach (Client client in GameSettings.gameServer.clients)
                {
                    client.isReady = false;
                    GameSettings.gameServer.SendEnd(client);
                    client.Player = new Player(client.Player.PlayerId); //Återställer dens orginal värden
                    GameSettings.gameServer.SendGameInfo(client);
                }
            }
        }
    }
}

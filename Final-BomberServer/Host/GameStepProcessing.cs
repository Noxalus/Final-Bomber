using FBLibrary;
using FBLibrary.Core;
using Final_BomberServer.Core;
using System.Collections.Generic;

namespace Final_BomberServer.Host
{
    partial class HostGame
    {
        List<Player> _alivePlayers;
        private void GameStepProccesing()
        {
            if (GameSettings.gameServer.Clients.Count == GameConfiguration.PlayerNumber // TO CHANGE
                && !StartedMatch /*&& GameSettings.gameServer.clients.IsClientsReady()*/)
            {
                GameInitialize();
            }

            foreach (Client client in GameSettings.gameServer.Clients)
            {
                if (client.NewClient && StartedMatch && client.isReady)
                {
                    GameSettings.gameServer.SendStartGame(client, true);
                    GameSettings.gameServer.SendPlayersToNew(client);
                    client.NewClient = false;
                }
            }

            // End of game
            _alivePlayers = GameSettings.gameServer.Clients.GetAlivePlayers();
            if (StartedMatch && false /*_alivePlayers.Count < 2*/)
            {
                //MainServer.SendPlayerStats();
                GameSettings.CurrentMap++;
                //MainServer.SendNextMap();
                EndGame();
                foreach (Client client in GameSettings.gameServer.Clients)
                {
                    client.isReady = false;
                    GameSettings.gameServer.SendEnd(client);
                    client.Player = new Player(client.Player.Id); //Återställer dens orginal värden
                    GameSettings.gameServer.SendGameInfo(client);
                }
            }
        }
    }
}

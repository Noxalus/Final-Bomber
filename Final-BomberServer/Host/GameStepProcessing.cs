using System;
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
                && !StartedMatch && GameSettings.gameServer.Clients.IsClientsReady())
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

            // End of round
            _alivePlayers = GameSettings.gameServer.Clients.GetAlivePlayers();
            if (StartedMatch && _alivePlayers.Count < 1)
            {
                int maxScore = 0;
                foreach (var player in GameSettings.gameServer.Clients.GetPlayers())
                {
                    maxScore = Math.Max(maxScore, player.Stats.Score);
                }

                if (maxScore >= ServerSettings.ScoreToWin)
                {
                    // End of game
                    //MainServer.SendPlayerStats();
                    GameSettings.CurrentMap++;
                    //MainServer.SendNextMap();
                    EndGame();
                    foreach (Client client in GameSettings.gameServer.Clients)
                    {
                        client.isReady = false;
                        GameSettings.gameServer.SendEnd(client);
                        // Restore the original values
                        var newPlayer = new Player(client.Player.Id);
                        GameManager.AddPlayer(client, newPlayer);
                        GameSettings.gameServer.SendGameInfo(client);
                    }
                }
                else
                {
                    // Reset
                    //HostGame.GameManager.Reset();
                    foreach (Client client in GameSettings.gameServer.Clients)
                    {
                        GameSettings.gameServer.SendRoundEnd(client);

                        var newPlayer = new Player(client.Player.Id);
                        GameManager.AddPlayer(client, newPlayer);

                        GameSettings.gameServer.SendGameInfo(client);
                    } 
                }
            }
        }
    }
}

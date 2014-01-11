using System;
using FBLibrary;
using FBLibrary.Core;
using FBServer.Core;
using System.Collections.Generic;

namespace FBServer.Host
{
    partial class HostGame
    {
        List<Player> _alivePlayers;
        private void GameStepProccesing()
        {
            if (GameSettings.GameServer.Clients.Count == GameConfiguration.PlayerNumber // TO CHANGE
                && !StartedMatch && GameSettings.GameServer.Clients.IsClientsReady())
            {
                GameInitialize();
            }

            foreach (Client client in GameSettings.GameServer.Clients)
            {
                if (client.NewClient && StartedMatch && client.isReady)
                {
                    GameSettings.GameServer.SendStartGame(client, true);
                    GameSettings.GameServer.SendPlayersToNew(client);
                    client.NewClient = false;
                }
            }

            // End of round
            _alivePlayers = GameSettings.GameServer.Clients.GetAlivePlayers();
            if (StartedMatch && _alivePlayers.Count <= GameConfiguration.AlivePlayerRemaining)
            {
                int maxScore = 0;
                foreach (var player in GameSettings.GameServer.Clients.GetPlayers())
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
                    foreach (Client client in GameSettings.GameServer.Clients)
                    {
                        client.isReady = false;
                        GameSettings.GameServer.SendEnd(client);
                        // Restore the original values
                        var newPlayer = new Player(client.Player.Id);
                        GameManager.AddPlayer(client, newPlayer);
                        GameSettings.GameServer.SendGameInfo(client);
                    }
                }
                else
                {
                    // Reset
                    HostGame.GameManager.Reset();
                    EndGame();
                    foreach (Client client in GameSettings.GameServer.Clients)
                    {
                        client.isReady = false;
                        client.AlreadyPlayed = true;
                        GameSettings.GameServer.SendRoundEnd(client);

                        var newPlayer = new Player(client.Player.Id, client.Player.Stats);
                        GameManager.AddPlayer(client, newPlayer);

                        GameSettings.GameServer.SendGameInfo(client);
                    } 
                }
            }
        }
    }
}

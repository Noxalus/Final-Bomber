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
            if (GameServer.Instance.Clients.Count == GameConfiguration.PlayerNumber // TO CHANGE
                && !StartedMatch && GameServer.Instance.Clients.IsClientsReady())
            {
                GameInitialize();
            }

            foreach (Client client in GameServer.Instance.Clients)
            {
                if (client.NewClient && StartedMatch && client.isReady)
                {
                    GameServer.Instance.SendStartGame(client, true);
                    GameServer.Instance.SendPlayersToNew(client);
                    client.NewClient = false;
                }
            }

            // End of round
            _alivePlayers = GameServer.Instance.Clients.GetAlivePlayers();
            if (StartedMatch && _alivePlayers.Count <= GameConfiguration.AlivePlayerRemaining)
            {
                int maxScore = 0;
                foreach (var player in GameServer.Instance.Clients.GetPlayers())
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
                    foreach (Client client in GameServer.Instance.Clients)
                    {
                        client.isReady = false;
                        GameServer.Instance.SendEnd(client);
                        // Restore the original values
                        var newPlayer = new Player(client.Player.Id);
                        GameManager.AddPlayer(client, newPlayer);
                        GameServer.Instance.SendGameInfo(client);
                    }
                }
                else
                {
                    // Reset
                    HostGame.GameManager.Reset();
                    EndGame();
                    foreach (Client client in GameServer.Instance.Clients)
                    {
                        client.isReady = false;
                        client.AlreadyPlayed = true;
                        GameServer.Instance.SendRoundEnd(client);

                        var newPlayer = new Player(client.Player.Id, client.Player.Stats);
                        GameManager.AddPlayer(client, newPlayer);

                        GameServer.Instance.SendGameInfo(client);
                    } 
                }
            }
        }
    }
}

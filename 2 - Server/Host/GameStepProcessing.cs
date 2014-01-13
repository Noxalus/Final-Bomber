using System;
using System.Linq;
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
                if (client.NewClient && StartedMatch && client.IsReady)
                {
                    GameServer.Instance.SendStartGame(client, true);
                    GameServer.Instance.SendPlayersToNew(client);
                    client.NewClient = false;
                }
            }

            // End of round
            //_alivePlayers = GameServer.Instance.Clients.GetAlivePlayers();
            if (StartedMatch && 
                GameServer.Instance.GameManager.PlayerList.Count(player => player.IsAlive) <= 
                GameConfiguration.AlivePlayerRemaining)
            {
                int maxScore = GameServer.Instance.GameManager.PlayerList.First().Stats.Score;
                foreach (var player in GameServer.Instance.GameManager.PlayerList)
                {
                    if (player.Stats.Score > maxScore)
                        maxScore = player.Stats.Score;
                }

                if (maxScore >= ServerSettings.ScoreToWin)
                {
                    // End of game
                    //MainServer.SendPlayerStats();

                    // Next map
                    GameSettings.CurrentMap++;

                    EndGame();

                    foreach (Client client in GameServer.Instance.Clients)
                    {
                        client.IsReady = false;
                        GameServer.Instance.SendEnd(client);
                        // Restore the original values
                        var newPlayer = new Player(client.Player.Id);
                        GameServer.Instance.SendGameInfo(client);
                    }
                }
                else
                {
                    // Reset
                    GameServer.Instance.GameManager.Reset();
                    
                    foreach (Client client in GameServer.Instance.Clients)
                    {
                        client.IsReady = false;
                        client.AlreadyPlayed = true;
                        GameServer.Instance.SendRoundEnd(client);

                        //var newPlayer = new Player(client.Player.Id, client.Player.Stats);
                        //GameManager.AddPlayer(client, newPlayer);

                        GameServer.Instance.SendGameInfo(client);
                    } 
                }
            }
        }
    }
}

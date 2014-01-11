using System;
using System.Linq;
using FBLibrary.Core;

namespace FBServer.Host
{
    partial class GameServer
    {
        void ReceiveNeedMap(Client client)
        {
            SendCurrentMap(client);
            Program.Log.Info("Client " + client.ClientId + " need the current map, sending it to him");
        }

        void ReceiveReady(Client client, string username, string password)
        {
            if (!client.isReady)
            {
                if (!client.AlreadyPlayed)
                {
                    var playerNames = GameServer.Instance.Clients.Select(c => c.Username).ToList();

                    client.Username = username;
                    if (playerNames.Contains(client.Username))
                    {
                        var concat = 1;
                        while (playerNames.Contains(client.Username + concat))
                        {
                            concat++;
                        }

                        client.Username = username + concat;
                    }
                }

                client.isReady = true;
                //MainServer.SendCheckIfOnline(username, password);
                Program.Log.Info("Client " + client.ClientId + " is ready to play");
                client.Player.Name = client.Username;
            }
            else
            {
                Program.Log.Info("Client " + client.ClientId + " send again that he is ready !");
            }
        }

        void ReceiveMovePlayer(Client client, LookDirection movement)
        {
            if (client.Player.IsAlive)
                client.Player.SetMovement(movement); // Receives the player's current direction
        }

        #region BombPlacing
        public delegate void BombPlacingEventHandler(Client sender);
        public event BombPlacingEventHandler BombPlacing;
        protected virtual void OnBombPlacing(Client sender)
        {
            if (BombPlacing != null)
                BombPlacing(sender);
        }
        #endregion
        void ReceiveBombPlacing(Client client)
        {
            if (client.Player.IsAlive)
                OnBombPlacing(client);
        }
    }
}

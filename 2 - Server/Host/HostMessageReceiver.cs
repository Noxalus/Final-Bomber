using System.Linq;
using FBLibrary.Core;
using FBServer.Core;

namespace FBServer.Host
{
    sealed partial class GameServer
    {
        void ReceiveNeedMap(Client client)
        {
            SendCurrentMap(client);
            Program.Log.Info("Client " + client.ClientId + " need the current map, sending it to him");
        }

        void ReceiveClientCredentials(Client client, string username, string password)
        {
            client.Username = username;
            client.Password = password;

            //MainServer.SendCheckIfOnline(username, password);

            Program.Log.Info("Client " + client.ClientId + " sent player info. (username: " + username + "|password: " + password);

            // Create a new player
            var player = new Player(client.ClientId);

            Instance.GameManager.AddPlayer(client, player);
        }

        void ReceiveReady(Client client, bool value)
        {
            if (!client.IsReady)
            {
                client.IsReady = value;

                var isReadyText = (value) ? "ready" : "not ready";
                Program.Log.Info("Client " + client.ClientId + " is " + isReadyText + " !");
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

        private void OnBombPlacing(Client sender)
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

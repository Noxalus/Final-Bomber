using FBLibrary;
using FBLibrary.Core;
using FBServer.Core;

namespace FBServer.Host
{
    sealed partial class GameServer
    {
        void ReceiveCredentials(Client client, string username, string password)
        {
            client.Username = username;
            client.Password = password;

            //MainServer.SendCheckIfOnline(username, password);

            Program.Log.Info("Client " + client.ClientId + " sent player info. (username: " + username + "|password: " + password + ")");

            // Create a new player
            var player = new Player(client.ClientId);

            Instance.GameManager.AddPlayer(client, player);
        }

        void ReceiveMapSelection(Client client, string md5)
        {
            if (client.IsHost)
            {
                Instance.SelectedMapName = MapLoader.GetMapNameFromMd5(md5);

                SendSelectedMap(client);
            }
        }

        void ReceiveReady(Client client, bool ready)
        {
            client.IsReady = ready;

            if (client.IsReady)
            {
                Program.Log.Info("Client " + client.ClientId + " is ready !");
            }
            else
            {
                Program.Log.Info("Client " + client.ClientId + " is not ready actually !");
            }

            SendIsReady(client, ready);
        }

        private void ReceiveWantToStartGame()
        {
            Instance.SendGameWillStart();
        }

        void ReceiveNeedMap(Client client)
        {
            SendCurrentMap(client);
            Program.Log.Info("Client " + client.ClientId + " need the current map, sending it to him");
        }

        void ReceiveHasMap(Client client)
        {
            client.HasMap = true;
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

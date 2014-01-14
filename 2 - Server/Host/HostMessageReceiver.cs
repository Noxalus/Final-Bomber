using FBLibrary;
using FBLibrary.Core;
using FBServer.Core;

namespace FBServer.Host
{
    sealed partial class GameServer
    {
        #region Pre game actions

        void ReceiveCredentials(Client client, string username, string password)
        {
            client.Username = username;
            client.Password = password;

            //MainServer.SendCheckIfOnline(username, password);

            Program.Log.Info("[RECEIVE][Client #" + client.ClientId + "] Credentials. (username: " + username + "|password: " + password + ")");

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
                Program.Log.Info("[RECEIVE][Client #" + client.ClientId + "] Client is ready !");
            }
            else
            {
                Program.Log.Info("[RECEIVE][Client #" + client.ClientId + "]  Client is not ready actually !");
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
            Program.Log.Info("[RECEIVE][Client #" + client.ClientId + "] Client needs the current map, sending it to him ! :)");
        }

        void ReceiveHasMap(Client client)
        {
            client.HasMap = true;
        }

        #endregion

        #region Game actions
        void ReceiveMovePlayer(Client client, LookDirection movement)
        {
            // Receives the player's current direction
            if (client.Player.IsAlive)
                client.Player.SetMovement(movement); 
        }

        void ReceiveBombPlacing(Client client)
        {
            if (client.Player.IsAlive)
                OnBombPlacing(client);
        }

        #endregion
    }
}

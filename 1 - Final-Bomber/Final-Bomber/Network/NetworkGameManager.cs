using FBClient.Core;
using FBClient.Entities;

namespace FBClient.Network
{
    /// <summary>
    /// This class is specificaly used for the logic of network games
    /// </summary>
    public class NetworkGameManager : GameManager
    {
        public NetworkManager NetworkManager { get; set; }

        public NetworkGameManager()
        {
            NetworkManager = new NetworkManager();
        }

        public override void Initialize()
        {
            base.Initialize();

            NetworkManager.Initiliaze();
        }

        public void AddPlayers()
        {
            foreach (var client in GameServer.Instance.Clients)
            {
                client.Player.SetGameManager(this);
                AddPlayer(client.Player);
            }
        }

        public override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Dispose()
        {
            base.Dispose();

            NetworkManager.Dispose();
        }

        public override void Update()
        {
            base.Update();

            NetworkManager.Update();
        }

        #region Events actions

        public override void RoundEndAction()
        {
            base.RoundEndAction();
        }

        public void AddClient(Client client)
        {
            GameServer.Instance.Clients.AddClient(client);
        }

        public Client GetClientFromPlayer(Player player)
        {
            return GameServer.Instance.Clients.Find(c => c.Player == player);
        }

        #endregion
    }
}
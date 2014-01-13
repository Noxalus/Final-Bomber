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

        public void AddClient(Client client, bool me = false)
        {
            GameServer.Instance.Clients.AddClient(client, me);
        }

        #endregion
    }
}
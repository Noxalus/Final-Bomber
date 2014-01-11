using FBClient.Core;

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

            NetworkManager.LoadContent();
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

            NetworkManager.Reset();
            AddPlayer(NetworkManager.Me);
        }

        #endregion
    }
}
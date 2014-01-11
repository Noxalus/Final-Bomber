using FBClient.Core;

namespace FBClient.Network
{
    /// <summary>
    /// This class is specificaly used for the logic of network games
    /// </summary>
    public class NetworkGameManager : GameManager
    {
        private NetworkManager _networkManager;

        public NetworkGameManager()
        {
        }

        public void SetNetworkManager(NetworkManager networkManager)
        {
            _networkManager = networkManager;
        }

        #region Events actions

        protected override void RoundEndAction()
        {
            base.RoundEndAction();

            _networkManager.Reset();
            AddPlayer(_networkManager.Me);
        }

        #endregion
    }
}
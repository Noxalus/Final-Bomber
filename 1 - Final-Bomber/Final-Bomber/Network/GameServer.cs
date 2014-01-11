
using FBClient.Core;

namespace FBClient.Network
{
    partial class GameServer
    {
        public GameManager GameManager;

        private static GameServer _instance;

        public static GameServer Instance
        {
            get { return _instance ?? (_instance = new GameServer()); }
        }

        private GameServer()
        {
        }

        public void SetGameManager(GameManager gameManager)
        {
            GameManager = gameManager;
        }

    }
}

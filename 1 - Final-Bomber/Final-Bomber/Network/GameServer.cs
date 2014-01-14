using System.Collections.Generic;
using FBClient.Core;

namespace FBClient.Network
{
    sealed partial class GameServer
    {
        public NetworkGameManager GameManager;
        public readonly ClientCollection Clients;
        public Dictionary<string, string> Maps;
        public string SelectedMapMd5;

        private static GameServer _instance;
        
        public static GameServer Instance
        {
            get { return _instance ?? (_instance = new GameServer()); }
        }

        private GameServer()
        {
            Clients = new ClientCollection();
            Maps = new Dictionary<string, string>();
            SelectedMapMd5 = "";
        }

        public void SetGameManager(GameManager gameManager)
        {
            GameManager = (NetworkGameManager)gameManager;
            GameManager.Initialize();
        }
    }
}

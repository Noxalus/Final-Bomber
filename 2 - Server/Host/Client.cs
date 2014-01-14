using FBServer.Core;
using Lidgren.Network;

namespace FBServer.Host
{
    public class Client
    {
        public Client(ref NetConnection client, int clientId)
        {
            ClientConnection = client;
            ClientId = clientId;
            AlreadyPlayed = false;
        }

        public bool IsReady = false;
        public bool IsHost = false;
        public bool NewClient = true;
        public bool Spectating = false;
        public string Username = "[Unknown]";
        public string Password = "";
        public bool AlreadyPlayed;
        public float Ping = 0f;
        public bool HasMap = false;

        public int ClientId { get; private set; }

        public NetConnection ClientConnection { get; private set; }

        public Player Player { get; set; }
    }
}

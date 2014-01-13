using FBClient.Entities;

namespace FBClient.Network
{
    public class Client
    {
        public int Id;
        public string Username;
        public bool IsReady;
        public float Ping;

        public Player Player;

        public Client(int id)
        {
            Id = id;
            Username = "[Unknown]";
            IsReady = false;
            Ping = 0;

            Player = null;
        }
    }
}

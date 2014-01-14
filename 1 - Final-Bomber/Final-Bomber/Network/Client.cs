using System;
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

        public bool Spectating;
        public bool NewPlayer;

        public bool IsHost { get { return Id == 0; } }

        public Client(int id)
        {
            Id = id;
            Username = "[Unknown]";
            IsReady = false;
            Ping = 0f;

            Player = null;

            Spectating = false;
            NewPlayer = false;
        }

        public double GetRoundedPing()
        {
            return Math.Round(Ping);
        }
    }
}

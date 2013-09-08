using Final_BomberServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final_BomberServer.Host
{
    partial class GameServer
    {
        void ReceiveNeedMap(Client client)
        {
            SendCurrentMap(client);
            Console.WriteLine("\nClient " + client.ClientId + " need the current map, sending it to him");
        }

        void ReceiveReady(Client client, string username, string password)
        {
            client.Username = username;
            //MainServer.SendCheckIfOnline(username, password);
            Console.WriteLine("\nClient " + client.ClientId + " is ready to play");
        }

        void ReceiveMovePlayer(Client client, Player.ActionEnum movement)
        {
            if (!client.Player.IsDead)
                client.Player.SetMovement(movement); // Receives the player's current direction
        }

        #region BombPlacing
        public delegate void BombPlacingEventHandler(Client sender);
        public event BombPlacingEventHandler BombPlacing;
        protected virtual void OnBombPlacing(Client sender)
        {
            if (BombPlacing != null)
                BombPlacing(sender);
        }
        #endregion
        void ReceiveBombPlacing(Client client)
        {
            if (!client.Player.IsDead)
                OnBombPlacing(client);
        }

    }
}

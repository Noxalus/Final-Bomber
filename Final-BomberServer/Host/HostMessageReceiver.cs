using FBLibrary.Core;
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
            Program.Log.Info("\nClient " + client.ClientId + " need the current map, sending it to him");
        }

        void ReceiveReady(Client client, string username, string password)
        {
            client.Username = username;
            //MainServer.SendCheckIfOnline(username, password);
            Program.Log.Info("\nClient " + client.ClientId + " is ready to play");
        }

        void ReceiveMovePlayer(Client client, LookDirection movement)
        {
            if (client.Player.IsAlive)
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
            if (client.Player.IsAlive)
                OnBombPlacing(client);
        }

    }
}

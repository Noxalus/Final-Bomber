using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using FBLibrary.Core;
using Final_Bomber.Core;
using Final_Bomber.Core.Players;
using Final_Bomber.Entities;
using Final_Bomber.Screens;

namespace Final_Bomber.Network
{
    public class NetworkManager
    {
        public string PublicIp;

        // Players
        public OnlineHumanPlayer Me;

        // Game manager
        private readonly GameManager _gameManager;

        public NetworkManager(GameManager gameManager)
        {
            _gameManager = gameManager;

            Me = new OnlineHumanPlayer(0) { Name = "Me" };
        }

        public void Initiliaze()
        {

            PublicIp = "?";

            // Server events
            GameSettings.GameServer.NewPlayer += GameServer_NewPlayer;
            GameSettings.GameServer.MovePlayer += GameServer_MovePlayer;
            GameSettings.GameServer.End += GameServer_End;
        }

        public void Dispose()
        {
            GameSettings.GameServer.NewPlayer -= GameServer_NewPlayer;
            GameSettings.GameServer.MovePlayer -= GameServer_MovePlayer;
            GameSettings.GameServer.End -= GameServer_End;
        }

        public void Update()
        {
            if (GameSettings.GameServer.HasStarted)
            {
                GameSettings.GameServer.RunClientConnection();
            }
        }

        #region Server events

        private void GameServer_NewPlayer(int playerID, float moveSpeed, string username)
        {
            if (_gameManager.Players.GetPlayerByID(playerID) == null)
            {
                var player = new OnlinePlayer(playerID)
                {
                    Name = "Online Player"
                };
                //player.MoveSpeed = moveSpeed;
                //Entities.Add(player);
                _gameManager.Players.Add(player);
            }
        }

        private void GameServer_MovePlayer(object sender, MovePlayerArgs arg)
        {
            Player player = _gameManager.Players.GetPlayerByID(arg.PlayerID);

            if (player != null)
            {
                // TODO => Move Players on the map
                player.Position = arg.Position;
                player.ChangeLookDirection(arg.Action);
                /*
                player.MapPosition = arg.pos;
                if (arg.action != 255)
                    player.movementAction = (Player.ActionEnum)arg.action;
                player.UpdateAnimation();
                */
            }
        }

        private void GameServer_End(bool won)
        {
            /*
            endTmr.Start();
            if (!Spectator)
            {
                shouldEnd = true;
                haveWon = won;
            }
            */
        }

        #endregion

        #region IP Methods

        private string GetMyIpAddress()
        {
            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                }
            }
            return localIP;
        }

        private string GetPublicIP()
        {
            String direction = "";
            WebRequest request = WebRequest.Create("http://checkip.dyndns.org/");
            using (WebResponse response = request.GetResponse())
            using (StreamReader stream = new StreamReader(response.GetResponseStream()))
            {
                direction = stream.ReadToEnd();
            }

            //Search for the ip in the html
            int first = direction.IndexOf("Address: ") + 9;
            int last = direction.LastIndexOf("</body>");
            direction = direction.Substring(first, last - first);

            return direction;
        }

        #endregion
    }
}

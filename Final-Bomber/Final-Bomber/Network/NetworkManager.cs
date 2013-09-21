﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Final_Bomber.Core;
using Final_Bomber.Core.Players;
using Final_Bomber.Entities;

namespace Final_Bomber.Network
{
    class NetworkManager
    {
        public bool IsConnected;
        public string PublicIp;

        // Players
        public OnlineHumanPlayer Me;

        // Game manager
        private readonly GameManager _gameManager;

        public NetworkManager(GameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public void Initiliaze()
        {
            Me = new OnlineHumanPlayer(0) { Name = "Me" };

            IsConnected = false;
            PublicIp = "?";

            // Server events
            GameSettings.GameServer.StartInfo += GameServer_StartInfo;
            GameSettings.GameServer.StartGame += GameServer_StartGame;

            GameSettings.GameServer.NewPlayer += GameServer_NewPlayer;
            GameSettings.GameServer.MovePlayer += GameServer_MovePlayer;
            GameSettings.GameServer.End += GameServer_End;
        }

        public void Dispose()
        {
            GameSettings.GameServer.StartInfo -= GameServer_StartInfo;
            GameSettings.GameServer.StartGame -= GameServer_StartGame;

            GameSettings.GameServer.NewPlayer -= GameServer_NewPlayer;
            GameSettings.GameServer.MovePlayer -= GameServer_MovePlayer;
            GameSettings.GameServer.End -= GameServer_End;
        }

        public void Update()
        {
            if (!IsConnected)
            {
                GameSettings.GameServer.StartClientConnection("127.0.0.1", "2643");

                var connectedTmr = new Timer();
                connectedTmr.Start();
                while (!IsConnected)
                {
                    GameSettings.GameServer.RunClientConnection();
                    if (GameSettings.GameServer.Connected)
                    {
                        IsConnected = true;
                        //_publicIp = GetPublicIP();
                    }
                    else if (connectedTmr.Each(5000))
                    {
                        Debug.Print("Couldn't connect to the Game Server, please refresh the game list");
                        FinalBomber.Instance.Exit();
                    }
                }
            }
            else if (GameSettings.GameServer.HasStarted)
            {
                GameSettings.GameServer.RunClientConnection();
            }
        }

        #region Game Server events

        private bool _isReady = false;
        private void GameServer_StartInfo()
        {
            _isReady = true;
        }

        private void GameServer_StartGame(bool gameInProgress, int playerId, float moveSpeed, int suddenDeathTime)
        {
            if (true /*!mainGame.IsStarted*/)
            {
                /*
                if (lobbyScreen.IsLoaded)
                {
                    lobbyScreen.End();
                    lobbyScreen.Unload();
                }
                */
                if (!gameInProgress)
                {
                    Me.Id = playerId;
                    /*
                    me.MoveSpeed = moveSpeed;
                    suddenDeathTime = suddenDeathTime;
                    */
                }
                else
                {
                    /*
                    mainGame.me.Kill();
                    mainGame.Spectator = true;
                    */
                }

                //mainGame.Start();
            }
        }


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
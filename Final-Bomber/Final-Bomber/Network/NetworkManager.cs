﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using FBLibrary.Core;
using Final_Bomber.Core;
using Final_Bomber.Core.Entities;
using Final_Bomber.Core.Players;
using Final_Bomber.Entities;
using Final_Bomber.Screens;
using Microsoft.Xna.Framework;

namespace Final_Bomber.Network
{
    public class NetworkManager
    {
        #region Events

        #region NewPlayer
        public delegate void AddPlayerEventHandler();
        public event AddPlayerEventHandler AddPlayer;
        protected virtual void OnAddPlayer()
        {
            if (AddPlayer != null)
                AddPlayer();
        }
        #endregion

        #endregion

        public string PublicIp;

        // Players
        public OnlineHumanPlayer Me;

        // Game manager
        private readonly GameManager _gameManager;

        public NetworkManager(GameManager gameManager)
        {
            _gameManager = gameManager;
            Me = new OnlineHumanPlayer(0) { Name = PlayerInfo.Username };
        }

        public void Initiliaze()
        {
            PublicIp = "?";

            // Server events
            GameSettings.GameServer.NewPlayer += GameServer_NewPlayer;
            GameSettings.GameServer.MovePlayer += GameServer_MovePlayer;
            GameSettings.GameServer.PlacingBomb += GameServer_PlacingBomb;
            GameSettings.GameServer.BombExploded += GameServer_BombExploded;
            GameSettings.GameServer.PowerUpDrop += GameServer_PowerUpDrop;
            GameSettings.GameServer.RoundEnd += GameServer_RoundEnd;
            GameSettings.GameServer.End += GameServer_End;
        }

        public void LoadContent()
        {
            Me.LoadContent();
        }

        public void Dispose()
        {
            GameSettings.GameServer.NewPlayer -= GameServer_NewPlayer;
            GameSettings.GameServer.MovePlayer -= GameServer_MovePlayer;
            GameSettings.GameServer.PlacingBomb -= GameServer_PlacingBomb;
            GameSettings.GameServer.BombExploded -= GameServer_BombExploded;
            GameSettings.GameServer.PowerUpDrop -= GameServer_PowerUpDrop;
            GameSettings.GameServer.RoundEnd -= GameServer_RoundEnd;
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
                var player = new OnlinePlayer(playerID) { Name =  username };

                player.LoadContent();
                //player.MoveSpeed = moveSpeed;
                //Entities.Add(player);
                _gameManager.AddPlayer(player);

                OnAddPlayer();
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

        private void GameServer_PlacingBomb(int playerId, Point position)
        {
            Player player = _gameManager.Players.GetPlayerByID(playerId);

            if (player != null)
            {
                var bomb = new Bomb(playerId, position, player.BombPower, player.BombTimer, player.Speed);
                player.CurrentBombAmount--;
                bomb.Initialize(_gameManager.CurrentMap, _gameManager.HazardMap);

                _gameManager.AddBomb(bomb);
            }
        }

        private void GameServer_BombExploded(Point position)
        {
            Bomb bomb = _gameManager.BombList.Find(b => b.CellPosition == position);

            //bomb.Destroy();
            /*
            foreach (Explosion ex in explosions)
            {
                //Ser till att explosionerna smällter ihop på ett snyggt sätt
                if (ex.Type == Explosion.ExplosionType.Down || ex.Type == Explosion.ExplosionType.Left
                        || ex.Type == Explosion.ExplosionType.Right || ex.Type == Explosion.ExplosionType.Up)
                {
                    Explosion temp_ex = Explosions.GetExplosionAtPosition(ex.originalPos, true);
                    if (temp_ex != null)
                    {
                        if (temp_ex.Type != ex.Type && Explosion.ConvertToOpposit(temp_ex.Type) != ex.Type)
                        {
                            Explosion temp_ex2 = new Explosion(ex.originalPos, Explosion.ExplosionType.Mid, true);
                            temp_ex2.explosionExistanceTime -= (int)temp_ex.tmrEnd.ElapsedMilliseconds;
                            Explosions.Add(temp_ex2);
                            Entities.Add(temp_ex2);
                        }
                    }
                }
                //Lägger till explosionerna till listorna
                Explosions.Add(ex);
                Entities.Add(ex);
            }
            */
        }

        private void GameServer_PowerUpDrop(PowerUpType type, Point position)
        {
            _gameManager.AddPowerUp(type, position);
        }

        private void GameServer_RoundEnd()
        {
            _gameManager.Reset();
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

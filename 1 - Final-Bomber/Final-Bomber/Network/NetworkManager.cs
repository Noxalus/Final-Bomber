﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using FBLibrary;
using FBLibrary.Core;
using FBClient.Core;
using FBClient.Core.Entities;
using FBClient.Core.Players;
using FBClient.Entities;
using FBClient.Screens.GameScreens;
using Microsoft.Xna.Framework;

namespace FBClient.Network
{
    public class NetworkManager
    {
        #region Events

        #region NewPlayer
        public delegate void AddPlayerEventHandler(object sender, EventArgs e);
        public event AddPlayerEventHandler AddPlayer;
        protected virtual void OnAddPlayer(object sender, EventArgs e)
        {
            if (AddPlayer != null)
                AddPlayer(sender, e);
        }
        #endregion

        #endregion

        // Timers
        TimeSpan _timer;
        readonly Timer _tmr;
        readonly Timer _connectedTmr;
        private Timer _tmrWaitUntilStart;

        public string PublicIp;
        public bool IsConnected;
        private bool _isReady;

        // Players
        public OnlineHumanPlayer Me;

        // Game manager
        private readonly GameManager _gameManager;

        public NetworkManager(GameManager gameManager)
        {
            _gameManager = gameManager;
            Me = new OnlineHumanPlayer(0);
            Me.SetGameManager(_gameManager);
            IsConnected = true;

            _timer = new TimeSpan();
            _tmr = new Timer();
            _connectedTmr = new Timer();
        }

        public void Reset()
        {
            string username = Me.Name;
            Me = new OnlineHumanPlayer(0, Me.Stats) {Name = username};
            Me.SetGameManager(_gameManager);

            LoadContent();
        }

        public void Initiliaze()
        {
            PublicIp = "?";

            // Server events
            GameServer.Instance.StartInfo += GameServer_StartInfo;
            GameServer.Instance.StartGame += GameServer_StartGame;

            GameServer.Instance.UpdatePing += GameServer_UpdatePing;
            GameServer.Instance.NewPlayer += GameServer_NewPlayer;
            GameServer.Instance.RemovePlayer += GameServer_RemovePlayer;
            GameServer.Instance.MovePlayer += GameServer_MovePlayer;
            GameServer.Instance.PlacingBomb += GameServer_PlacingBomb;
            GameServer.Instance.BombExploded += GameServer_BombExploded;
            GameServer.Instance.PowerUpDrop += GameServer_PowerUpDrop;

            Me.Name = PlayerInfo.Username;

            _tmr.Start();
            _tmrWaitUntilStart = new Timer();
            _connectedTmr.Start();
        }

        public void LoadContent()
        {
            Me.LoadContent();
        }

        public void Dispose()
        {
            // Server events
            GameServer.Instance.StartInfo -= GameServer_StartInfo;
            GameServer.Instance.StartGame -= GameServer_StartGame;

            GameServer.Instance.UpdatePing -= GameServer_UpdatePing;
            GameServer.Instance.NewPlayer -= GameServer_NewPlayer;
            GameServer.Instance.RemovePlayer -= GameServer_RemovePlayer;
            GameServer.Instance.MovePlayer -= GameServer_MovePlayer;
            GameServer.Instance.PlacingBomb -= GameServer_PlacingBomb;
            GameServer.Instance.BombExploded -= GameServer_BombExploded;
            GameServer.Instance.PowerUpDrop -= GameServer_PowerUpDrop;
        }

        public void Update()
        {
            if (!IsConnected)
            {
                GameServer.Instance.RunClientConnection();
                if (GameServer.Instance.Connected)
                {
                    IsConnected = true;
                }
                else if (_connectedTmr.Each(5000))
                {
                    Debug.Print("Couldn't connect to the Game Server, please refresh the game list");
                    FinalBomber.Instance.Exit();
                }
            }
            else
            {
                if (GameServer.Instance.HasStarted)
                    GameServer.Instance.RunClientConnection();

                if (_isReady)
                    ProgramStepProccesing();
            }
        }

        private void ProgramStepProccesing()
        {
            if (!GameServer.Instance.Connected)
            {
                DisplayStatusBeforeExiting("The Game Server has closed/disconnected");
            }
            if (GameServer.Instance.Connected)
            {
                ConnectedGameProcessing();
            }
        }

        private void DisplayStatusBeforeExiting(string status)
        {
            throw new Exception("Exit ! (not connected to the server !)");
        }

        private void ConnectedGameProcessing()
        {
            if (_isReady)
            {
                GameServer.Instance.SendIsReady();
                _isReady = false;
            }
        }

        #region Server events

        private void GameServer_StartInfo()
        {
            _isReady = true;
        }

        private void GameServer_StartGame(bool gameInProgress, int playerId, float moveSpeed, int suddenDeathTime, List<Point> wallPositions)
        {
            if (!gameInProgress)
            {
                NetworkGamePlayScreen.NetworkManager.Me.Id = playerId;
                //NetworkTestScreen.NetworkManager.MoveSpeed = moveSpeed;
                GameConfiguration.SuddenDeathTimer = TimeSpan.FromMilliseconds(suddenDeathTime);
            }
            else
            {
                /*
                mainGame.me.Kill();
                mainGame.Spectator = true;
                */
            }

            _gameManager.AddWalls(wallPositions);
        }

        private void GameServer_NewPlayer(int playerId, float moveSpeed, string username, int score)
        {
            if (_gameManager.Players.GetPlayerByID(playerId) == null)
            {
                var player = new OnlinePlayer(playerId) {Name = username, Stats = {Score = score}};

                if (username == Me.Name)
                {
                    var playerNames = _gameManager.Players.Select(p => p.Name).ToList();

                    if (playerNames.Contains(Me.Name))
                    {
                        var concat = 1;
                        while (playerNames.Contains(Me.Name + concat))
                        {
                            concat++;
                        }

                        Me.Name += concat;
                    }
                }

                player.LoadContent();
                //player.MoveSpeed = moveSpeed;
                _gameManager.AddPlayer(player);

                OnAddPlayer(this, EventArgs.Empty);
            }
        }

        private void GameServer_RemovePlayer(int playerId)
        {
            Player player = _gameManager.Players.GetPlayerByID(playerId);

            if (player != null && Me.Id != playerId)
            {
                _gameManager.RemovePlayer(player);
            }
        }

        private void GameServer_MovePlayer(object sender, MovePlayerArgs arg)
        {
            Player player = _gameManager.Players.GetPlayerByID(arg.PlayerId);

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

        private void GameServer_UpdatePing(float ping)
        {
            Me.Ping = ping;
        }

        private void GameServer_PlacingBomb(int playerId, Point position)
        {
            Player player = _gameManager.Players.GetPlayerByID(playerId);

            if (player != null)
            {
                var bomb = new Bomb(playerId, position, player.BombPower, player.BombTimer, player.Speed);
                player.CurrentBombAmount--;
                bomb.Initialize(_gameManager.CurrentMap.Size, _gameManager.CurrentMap.CollisionLayer, _gameManager.HazardMap);

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

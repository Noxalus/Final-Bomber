using System;
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

        #region NewClient
        public delegate void AddClientEventHandler(object sender, EventArgs e);
        public event AddClientEventHandler AddClient;
        protected virtual void OnAddClient(object sender, EventArgs e)
        {
            if (AddClient != null)
                AddClient(sender, e);
        }
        #endregion

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

        public NetworkManager()
        {
            IsConnected = true;

            _timer = new TimeSpan();
            _tmr = new Timer();
            _connectedTmr = new Timer();
        }

        public void Initiliaze()
        {
            PublicIp = "?";

            // Server events
            GameServer.Instance.StartGame += GameServer_StartGame;

            GameServer.Instance.UpdatePing += GameServer_UpdatePing;
            GameServer.Instance.NewClient += GameServer_NewClient;
            //GameServer.Instance.RemovePlayer += GameServer_RemovePlayer;
            GameServer.Instance.MovePlayer += GameServer_MovePlayer;
            GameServer.Instance.PlacingBomb += GameServer_PlacingBomb;
            GameServer.Instance.BombExploded += GameServer_BombExploded;
            GameServer.Instance.PowerUpDrop += GameServer_PowerUpDrop;

            _tmr.Start();
            _tmrWaitUntilStart = new Timer();
            _connectedTmr.Start();
        }

        public void Dispose()
        {
            // Server events
            GameServer.Instance.StartGame -= GameServer_StartGame;

            GameServer.Instance.UpdatePing -= GameServer_UpdatePing;
            GameServer.Instance.NewClient -= GameServer_NewClient;
            //GameServer.Instance.RemovePlayer -= GameServer_RemovePlayer;
            GameServer.Instance.MovePlayer -= GameServer_MovePlayer;
            GameServer.Instance.PlacingBomb -= GameServer_PlacingBomb;
            GameServer.Instance.BombExploded -= GameServer_BombExploded;
            GameServer.Instance.PowerUpDrop -= GameServer_PowerUpDrop;
        }

        public void Update()
        {
            if (!IsConnected)
            {
                GameServer.Instance.Update();

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
                    GameServer.Instance.Update();

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

        private void GameServer_StartGame(bool gameInProgress, List<Point> wallPositions)
        {
            GameServer.Instance.GameManager.AddWalls(wallPositions);
        }

        private void GameServer_NewClient(int clientId, string username, bool isReady)
        {
            if (GameServer.Instance.Clients.GetClientById(clientId) == null)
            {
                var me = GameServer.Instance.Clients.Me;
                if (username == me.Username)
                {
                    var clientUsernames = GameServer.Instance.Clients.Select(c => c.Username).ToList();

                    if (clientUsernames.Contains(me.Username))
                    {
                        var concat = 1;
                        while (clientUsernames.Contains(me.Username + concat))
                        {
                            concat++;
                        }

                        me.Username += concat;
                    }
                }

                var client = new Client(clientId)
                {
                    Username = username, 
                    IsReady = isReady
                };

                GameServer.Instance.GameManager.AddClient(client);
            }
        }

        /*
        private void GameServer_RemovePlayer(int playerId)
        {
            Player player = GameServer.Instance.GameManager.Players.GetPlayerByID(playerId);

            if (player != null && Me.Id != playerId)
            {
                GameServer.Instance.GameManager.RemovePlayer(player);
            }
        }
        */

        private void GameServer_MovePlayer(object sender, MovePlayerArgs arg)
        {
            Client client = GameServer.Instance.Clients.GetClientById(arg.ClientId);

            if (client != null)
            {
                if (client.Player != null)
                {
                    client.Player.Position = arg.Position;
                    client.Player.ChangeLookDirection(arg.Direction);
                }
                else
                {
                    throw new Exception("This player doesn't exist ! (clientId: " + arg.ClientId + ")");
                }
            }
            else
            {
                throw new Exception("This client doesn't exist ! (clientId: " + arg.ClientId + ")");
            }
        }

        private void GameServer_UpdatePing(float ping)
        {
            GameServer.Instance.Clients.Me.Ping = ping;
        }

        private void GameServer_PlacingBomb(int clientId, Point position)
        {
            Client client = GameServer.Instance.Clients.GetClientById(clientId);

            if (client != null)
            {
                if (client.Player != null)
                {
                    var bomb = new Bomb(clientId, position, client.Player.BombPower, client.Player.BombTimer, client.Player.Speed);
                    client.Player.CurrentBombAmount--;

                    bomb.Initialize(GameServer.Instance.GameManager.CurrentMap.Size,
                        GameServer.Instance.GameManager.CurrentMap.CollisionLayer,
                        GameServer.Instance.GameManager.HazardMap);

                    GameServer.Instance.GameManager.AddBomb(bomb);
                }
                else
                {
                    throw new Exception("This player doesn't exist ! (clientId: " + clientId + ")");
                }
            }
            else
            {
                throw new Exception("This client doesn't exist ! (clientId: " + clientId + ")");
            }
        }

        private void GameServer_BombExploded(Point position)
        {
            Bomb bomb = GameServer.Instance.GameManager.BombList.Find(b => b.CellPosition == position);

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
            GameServer.Instance.GameManager.AddPowerUp(type, position);
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

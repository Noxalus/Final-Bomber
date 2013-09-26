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
            Me = new OnlineHumanPlayer(0);
        }

        public void Reset()
        {
            
        }

        public void Initiliaze()
        {
            PublicIp = "?";

            // Server events
            GameSettings.GameServer.UpdatePing += GameServer_UpdatePing;
            GameSettings.GameServer.NewPlayer += GameServer_NewPlayer;
            GameSettings.GameServer.RemovePlayer += GameServer_RemovePlayer;
            GameSettings.GameServer.MovePlayer += GameServer_MovePlayer;
            GameSettings.GameServer.PlacingBomb += GameServer_PlacingBomb;
            GameSettings.GameServer.BombExploded += GameServer_BombExploded;
            GameSettings.GameServer.PowerUpDrop += GameServer_PowerUpDrop;

            Me.Name = PlayerInfo.Username;
        }

        public void LoadContent()
        {
            Me.LoadContent();
        }

        public void Dispose()
        {
            // Server events
            GameSettings.GameServer.UpdatePing -= GameServer_UpdatePing;
            GameSettings.GameServer.NewPlayer -= GameServer_NewPlayer;
            GameSettings.GameServer.RemovePlayer -= GameServer_RemovePlayer;
            GameSettings.GameServer.MovePlayer -= GameServer_MovePlayer;
            GameSettings.GameServer.PlacingBomb -= GameServer_PlacingBomb;
            GameSettings.GameServer.BombExploded -= GameServer_BombExploded;
            GameSettings.GameServer.PowerUpDrop -= GameServer_PowerUpDrop;
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

                OnAddPlayer();
            }
        }

        private void GameServer_RemovePlayer(int playerID)
        {
            Player player = _gameManager.Players.GetPlayerByID(playerID);

            if (player != null && Me.Id != playerID)
            {
                _gameManager.RemovePlayer(player);
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

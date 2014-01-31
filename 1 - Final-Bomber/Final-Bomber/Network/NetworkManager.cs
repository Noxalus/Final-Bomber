using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using FBClient.Entities;
using FBLibrary.Core;
using FBClient.Core.Entities;
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
        
        public string PublicIp;
        public bool IsConnected;
        public bool IsReady;

        public NetworkManager()
        {
            IsConnected = false;
            IsReady = false;
        }

        public void Initiliaze()
        {
            PublicIp = "?";

            // Server events
            GameServer.Instance.UpdatePing += GameServer_UpdatePing;
            GameServer.Instance.NewClient += GameServer_NewClient;
            GameServer.Instance.MovePlayer += GameServer_MovePlayer;
            GameServer.Instance.PlacingBomb += GameServer_PlacingBomb;
            GameServer.Instance.BombExploded += GameServer_BombExploded;
            GameServer.Instance.PlayerKilled += GameServer_PlayerKilled;
            GameServer.Instance.PlayerSuicide += GameServer_PlayerSuicide;
            GameServer.Instance.PowerUpDrop += GameServer_PowerUpDrop;
            GameServer.Instance.PowerUpPickUp += GameServer_PowerUpPickUp;
        }

        public void Dispose()
        {
            // Server events
            GameServer.Instance.UpdatePing -= GameServer_UpdatePing;
            GameServer.Instance.NewClient -= GameServer_NewClient;
            GameServer.Instance.MovePlayer -= GameServer_MovePlayer;
            GameServer.Instance.PlacingBomb -= GameServer_PlacingBomb;
            GameServer.Instance.BombExploded -= GameServer_BombExploded;
            GameServer.Instance.PlayerKilled -= GameServer_PlayerKilled;
            GameServer.Instance.PlayerSuicide -= GameServer_PlayerSuicide;
            GameServer.Instance.PowerUpDrop -= GameServer_PowerUpDrop;
        }

        public void Update()
        {
            // Read new messages from server and check connection status
            GameServer.Instance.Update();

            // First connection
            if (!IsConnected)
            {
                if (GameServer.Instance.Connected)
                {
                    IsConnected = true;
                    // TODO: Find a good way to display client IP
                    //_publicIp = GetPublicIP();
                }
            }
            else
            {
                if (GameServer.Instance.Disconnected)
                {
                    throw new Exception("Exit ! (not connected to the server !)");
                }
            }
        }

        #region Server events

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

        public void GameServer_PlacingBomb(int clientId, Point position)
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

        private void GameServer_PlayerKilled(int victimId, int killerId)
        {
            GameServer.Instance.GameManager.KillPlayer(victimId, killerId);
        }

        private void GameServer_PlayerSuicide(int suicidalId)
        {
            Player suicidalPlayer = GameServer.Instance.GameManager.GetPlayerFromClientId(suicidalId);

            GameServer.Instance.GameManager.SuicidePlayer(suicidalPlayer);
        }

        private void GameServer_PowerUpDrop(PowerUpType type, Point position)
        {
            GameServer.Instance.GameManager.AddPowerUp(type, position);
        }

        private void GameServer_PowerUpPickUp(int playerId, Point position, PowerUpType type)
        {
            // Get power up
            PowerUp powerUp = GameServer.Instance.GameManager.PowerUpList.Find(pu => pu.CellPosition == position);

            // Get player
            Player player = GameServer.Instance.GameManager.Players.Find(p => p.Id == playerId);

            // Apply power up
            if (powerUp != null)
            {
                GameServer.Instance.GameManager.PickUpPowerUp(player, powerUp);
            }
            else
            {
                Program.Log.Error("This power up doesn't exist ! (position: " + position + "| type: " + type + ")");
            }
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

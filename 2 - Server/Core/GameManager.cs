using System.Collections.Generic;
using System.Linq;
using FBLibrary;
using FBLibrary.Core;
using FBLibrary.Core.BaseEntities;
using FBServer.Core.Entities;
using FBServer.Core.WorldEngine;
using FBServer.Host;
using Microsoft.Xna.Framework;

namespace FBServer.Core
{
    public class GameManager : BaseGameManager
    {
        // Collections
        private readonly List<Player> _playerList;
        private readonly List<Bomb> _bombList;
        private readonly List<PowerUp> _powerUpList;
        private readonly List<Wall> _wallList;

        private bool _gameInitialized;
        private bool _hasStarted;
        private bool _gameHasBegun;
        private bool _suddenDeath;

        #region Properties

        public bool HasStarted
        {
            get { return _hasStarted; }
        }

        public BaseMap CurrentMap
        {
            get { return BaseCurrentMap; }
        }

        public List<Player> PlayerList
        {
            get { return _playerList; }
        }

        public List<Wall> WallList
        {
            get { return _wallList; }
        }

        public List<Bomb> BombList
        {
            get { return _bombList; }
        }

        public List<PowerUp> PowerUpList
        {
            get { return _powerUpList; }
        }

        public bool GameHasBegun
        {
            get { return _gameHasBegun; }
            set { _gameHasBegun = value; }
        }

        public bool GameInitialized
        {
            get { return _gameInitialized; }
        }

        #endregion

        public GameManager()
        {
            _gameInitialized = false;
            _gameHasBegun = false;
            _hasStarted = false;
            _suddenDeath = false;

            // Collections
            _playerList = new List<Player>();
            _wallList = new List<Wall>();
            _powerUpList = new List<PowerUp>();
            _bombList = new List<Bomb>();

            BaseCurrentMap = new Map();

            GameEventManager = new GameEventManager(this);
        }

        public void Initialize()
        {
            _hasStarted = true;
        }

        public override void Reset()
        {
            base.Reset();

            _wallList.Clear();
            _powerUpList.Clear();
            _bombList.Clear();
            _playerList.ForEach(player => player.Reset());
        }

        public override void Update()
        {
            if (GameServer.Instance.GameManager.GameInitialized && GameHasBegun)
            {
                RunGameLogic();

                CheckRoundEnd();

                base.Update();
            }
            else
            {
                if (GameServer.Instance.Clients.Any() &&
                    (GameServer.Instance.Clients.TrueForAll(client => client.IsReady) &&
                    GameServer.Instance.Clients.TrueForAll(client => client.HasMap)))
                {
                    StartGame();
                }
            }
        }

        public void StartGame()
        {
            // Game has to be initialized ?
            if (GameServer.Instance.Clients.Count >= GameConfiguration.MinimumPlayerNumber &&
                GameServer.Instance.Clients.Count <= ServerSettings.MaxConnection &&
                !GameInitialized)
            {
                GameInitialize();
            }
        }

        private void GameInitialize()
        {
            // Load the map chosen
            GameServer.Instance.GameManager.LoadMap(GameServer.Instance.SelectedMapName);

            // Display map info
            /*
            GameServer.Instance.GameManager.CurrentMap.DisplayBoard();
            GameServer.Instance.GameManager.CurrentMap.DisplayCollisionLayer();
            */

            Program.Log.Info("[LOADED MAP]");

            GameHasBegun = false;

            // Sort randomly players
            GameServer.Instance.Clients.Sort(new ClientRandomSorter());

            for (int i = 0; i < GameServer.Instance.Clients.Count; i++)
            {
                // Puts each player on their respective spawn points
                GameServer.Instance.Clients[i].Player.ChangePosition(
                    GameServer.Instance.GameManager.CurrentMap.PlayerSpawnPoints[i]);

                // These clients already exist, they are neither new either spectator
                GameServer.Instance.Clients[i].NewClient = false;
                GameServer.Instance.Clients[i].Spectating = false;

                // Send players info to everyone
                //GameServer.Instance.SendNewClientInfo(GameServer.Instance.Clients[i]);
            }

            Program.Log.Info("[INITIALIZED GAME]");

            _gameInitialized = true;

            // We send to players that the game has begun
            GameServer.Instance.SendStartGame(false);

            GameServer.Instance.GameManager.GameHasBegun = true;
        }

        private void CheckRoundEnd()
        {
            // Round end ?
            if (false && // TODO: Remove this
                GameInitialized &&
                GameServer.Instance.GameManager.PlayerList.Count(player => player.IsAlive) <=
                GameConfiguration.AlivePlayerRemaining)
            {
                int maxScore = GameServer.Instance.GameManager.PlayerList.First().Stats.Score;
                foreach (var player in GameServer.Instance.GameManager.PlayerList)
                {
                    if (player.Stats.Score > maxScore)
                        maxScore = player.Stats.Score;
                }

                if (maxScore >= ServerSettings.ScoreToWin)
                {
                    // End of game
                    //MainServer.SendPlayerStats();

                    EndGame();

                    foreach (Client client in GameServer.Instance.Clients)
                    {
                        client.IsReady = false;

                        GameServer.Instance.SendEnd(client);

                        // TODO: Start the game again
                        // GameServer.Instance.SendGameInfo(client);
                    }
                }
                else
                {
                    // Reset
                    GameServer.Instance.GameManager.Reset();

                    foreach (Client client in GameServer.Instance.Clients)
                    {
                        client.IsReady = false;
                        client.AlreadyPlayed = true;
                        GameServer.Instance.SendRoundEnd(client);

                        // TODO: Start the game again
                        // GameServer.Instance.SendGameInfo(client);
                    }
                }
            }
        }

        private void EndGame()
        {
            _gameInitialized = false;
            foreach (Client client in GameServer.Instance.Clients)
            {
                //client.Player.nextDirection = LookDirection.Idle;
                GameServer.Instance.SendPlayerPosition(client, false, false);
            }
        }

        private void RunGameLogic()
        {
            if (GameHasBegun)
            {
                MovePlayers();
            }
        }

        private void MovePlayers()
        {
            foreach (Client client in GameServer.Instance.Clients)
            {
                // Move the player to the next position
                //Program.Log.Info("Player position: " + client.Player.Position);                
                client.Player.MovePlayer(GameServer.Instance.GameManager.CurrentMap);
            }
        }

        #region Update entities

        protected override void UpdateWalls()
        {
            for (int i = 0; i < _wallList.Count; i++)
            {
                _wallList[i].Update();

                // We clean the obsolete elements
                if (!_wallList[i].IsAlive)
                {
                    RemoveWall(_wallList[i]);
                }
            }

            base.UpdateWalls();
        }

        protected override void UpdateBombs()
        {
            for (int i = 0; i < _bombList.Count; i++)
            {
                _bombList[i].Update();

                // Do we delete the bomb
                if (!_bombList[i].IsAlive)
                {
                    RemoveBomb(_bombList[i]);
                }
            }

            base.UpdateBombs();
        }

        protected override void UpdatePowerUps()
        {
            for (int i = 0; i < _powerUpList.Count; i++)
            {
                if (!_powerUpList[i].IsAlive)
                {
                    RemovePowerUp(_powerUpList[i]);
                }
            }

            base.UpdatePowerUps();
        }

        protected override void UpdatePlayers()
        {
            var alivePlayers = _playerList.FindAll(p => p.IsAlive);
            foreach (var player in alivePlayers)
            {
                player.Update();
            }

            /*
            for (int i = 0; i < players.Count; i++)
            {

                if (!players[i].IsAlive)
                {
                    if (!BasePlayerList[i].OnEdge)
                    {
                        if (BaseCurrentMap.Board[BasePlayerList[i].CellPosition.X, BasePlayerList[i].CellPosition.Y] is BasePlayer)
                        {
                            RemovePlayer(BasePlayerList[i]);
                        }
                    }
                }
            }
            */

            base.UpdatePlayers();
        }
        #endregion

        public override void LoadMap(string mapName)
        {
            base.LoadMap(mapName);

            // We generate wall
            GenerateRandomWalls(ServerSettings.WallPercentage);

            CurrentMap.DisplayBoard();
        }

        #region Player methods

        public void AddPlayer(Client client, Player player)
        {
            client.Username = CheckUsernameAlreadyTaken(client);

            player.Name = client.Username;
            client.Player = player;

            _playerList.Add(player);

            base.AddPlayer(player);

            if (GameServer.Instance.GameManager.GameInitialized)
            {
                client.Spectating = true;
                client.NewClient = true;
            }

            // Send the server generated id to the corresponding client
            GameServer.Instance.SendClientId(client);
            // Send list of available map
            GameServer.Instance.SendAvailableMaps(client);

            // Send selected map
            GameServer.Instance.SendSelectedMap(client);

            GameServer.Instance.SendNewClientInfo(client);
            GameServer.Instance.SendClientsToNew(client);
        }

        /// <summary>
        /// Check if the client's username is already taken and provide a new unique username
        /// </summary>
        /// <param name="client">Client we want to check the username</param>
        /// <returns>A non-used username</returns>
        private string CheckUsernameAlreadyTaken(Client client)
        {
            var playerNames = _playerList.Select(p => p.Name).ToList();

            if (playerNames.Contains(client.Username))
            {
                var concat = 1;
                while (playerNames.Contains(client.Username + concat))
                {
                    concat++;
                }

                client.Username = client.Username + concat;
            }

            return client.Username;
        }

        protected override void DestroyPlayer(int playerId)
        {
            var player = _playerList.Find(p => p.Id == playerId);

            base.DestroyPlayer(player);
        }

        /*
        public void RemovePlayer(Client client, Player player)
        {
            var p = (BasePlayer)BaseCurrentMap.Board[player.CellPosition.X, player.CellPosition.Y];
            if (p.Id == player.Id)
                BaseCurrentMap.Board[player.CellPosition.X, player.CellPosition.Y] = null;

            client.Player = null;

            base.RemovePlayer(player);
        }
        */

        public override void KillPlayer(int victimId, int killerId)
        {
            base.KillPlayer(victimId, killerId);

            Client victimClient = GetClientFromId(victimId);
            Client killerClient = GetClientFromId(killerId);

            GameServer.Instance.SendKillPlayer(victimClient, killerClient);
        }

        public override void SuicidePlayer(BasePlayer suicidalPlayer)
        {
            base.SuicidePlayer(suicidalPlayer);

            Client suicidalClient = GetClientFromPlayer(suicidalPlayer);

            GameServer.Instance.SendSuicidePlayer(suicidalClient);
        }

        #endregion

        #region Wall methods

        public override void AddWall(Point position)
        {
            var wall = new Wall(position);
            _wallList.Add(wall);

            base.AddWall(wall);
        }

        public void AddWall(Wall wall)
        {
            _wallList.Add(wall);

            base.AddWall(wall);
        }


        protected override void DestroyWall(Point position)
        {
            var wall = _wallList.Find(w => w.CellPosition == position);

            base.DestroyWall(wall);

            if (wall == null)
                Program.Log.Error("Wall at " + position + " doesn't exist !");
            else
                Program.Log.Info("Wall at " + position + " has been removed.");
        }

        private void RemoveWall(Wall wall)
        {
            if (GameConfiguration.Random.Next(0, 100) < MathHelper.Clamp(GameConfiguration.PowerUpPercentage, 0, 100))
            {
                AddPowerUp(wall.CellPosition);
            }

            _wallList.Remove(wall);

            base.RemoveWall(wall);

            Program.Log.Info("Delete wall at " + wall.CellPosition);
        }

        #endregion

        #region Bomb methods

        public void AddBomb(Bomb bomb)
        {
            _bombList.Add(bomb);

            base.AddBomb(bomb);
        }

        protected override void DestroyBomb(Point position)
        {
            var bomb = _bombList.Find(b => b.CellPosition == position);

            base.DestroyBomb(bomb);

            if (bomb == null)
                Program.Log.Error("Bomb at " + position + " doesn't exist !");
            else
                Program.Log.Info("Bomb exploded at " + position);
        }

        private void RemoveBomb(Bomb bomb)
        {
            _bombList.Remove(bomb);

            base.RemoveBomb(bomb);

            Program.Log.Info("Bomb at " + bomb.CellPosition + " has been removed.");
        }

        #endregion

        #region Power up methods

        public override void AddPowerUp(Point position)
        {
            var powerUp = new PowerUp(position);
            _powerUpList.Add(powerUp);

            GameServer.Instance.SendPowerUpDrop(powerUp);

            base.AddPowerUp(powerUp);
        }

        public override void PickUpPowerUp(BasePlayer player, BasePowerUp powerUp)
        {
            powerUp.ApplyEffect(player);
            powerUp.PickUp();

            Program.Log.Info("Power up at " + powerUp.CellPosition + " has been taken by player " + player.Name + ".");


            Client client = GetClientFromPlayer(player);

            GameServer.Instance.SendPowerUpPickUp(client, (PowerUp)powerUp);

            powerUp.Remove();
        }

        protected override void DestroyPowerUp(Point position)
        {
            var powerUp = _powerUpList.Find(pu => pu.CellPosition == position);

            base.DestroyPowerUp(powerUp);

            if (powerUp == null)
                Program.Log.Error("Power up at " + position + "doesn't exist !");
            else
                Program.Log.Info("Power up at " + position + " has been destroyed.");
        }

        private void RemovePowerUp(PowerUp powerUp)
        {
            _powerUpList.Remove(powerUp);

            base.RemovePowerUp(powerUp);

            Program.Log.Info("Power up at " + powerUp.CellPosition + " has been removed.");
        }

        #endregion

        private Client GetClientFromPlayer(BasePlayer player)
        {
            return GameServer.Instance.Clients.Find(c => c.Player == player);
        }

        private Client GetClientFromId(int clientId)
        {
            return GameServer.Instance.Clients.Find(c => c.ClientId == clientId);
        }
    }
}

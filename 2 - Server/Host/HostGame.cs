using System.Diagnostics;
using FBLibrary;
using FBLibrary.Core;
using FBServer.Core;
using FBServer.Core.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBServer.Host
{
    partial class HostGame
    {
        bool _hasStarted;
        int _playerId = 0;

        Timer _beginGameTimer;
        bool _gameHasBegun;

        bool _suddenDeath = false;

        public bool HasStarted
        {
            get { return _hasStarted; }
        }

        public bool StartedMatch = false;

        public HostGame()
        {
        }

        public void Initialize()
        {
            GameServer.Instance.ConnectedClient += GameServer_ConnectedClient;
            GameServer.Instance.DisconnectedClient += GameServer_DisconnectedClient;
            GameServer.Instance.BombPlacing += GameServer_BombPlacing;
            GameServer.Instance.StartServer();

            /*
            GameServer.Instance.GameManager.LoadMap(MapLoader.MapFileDictionary.Keys.First());

            // Display info
            GameServer.Instance.GameManager.CurrentMap.DisplayBoard();
            GameServer.Instance.GameManager.CurrentMap.DisplayCollisionLayer();
            */
            
            _hasStarted = true;
        }

        public void Update()
        {
            GameServer.Instance.Update();

            GameStepProccesing();

            if (_beginGameTimer != null)
            {
                // Game really starts after 3 seconds
                if (!_gameHasBegun && _beginGameTimer.Each(3000))
                {
                    _beginGameTimer.Stop();
                    _gameHasBegun = true;
                }
            }

            if (StartedMatch && _gameHasBegun)
            {
                RunGameLogic();
            }
        }

        public void Dispose()
        {
            _hasStarted = false;
            StartedMatch = false;

            GameServer.Instance.EndServer("Ending game");
        }

        private void GameInitialize()
        {
            _gameHasBegun = false;
            _beginGameTimer = new Timer(true);
            //GameSettings.Get_gameManager.CurrentMap().CreateMap();
            //_gameManager.CurrentMap = GameSettings.Get_gameManager.CurrentMap();

            _suddenDeath = false;
            //suddenDeathTPE = 1000 / ((_gameManager.CurrentMap.mapWidth * _gameManager.CurrentMap.mapHeight) / 30); //Räknar ut tiden mellan varje explosion (i ms)
            //tmr_UntilSuddenDeath = new H.U.D.Timer(false);
            GameServer.Instance.Clients.Sort(new ClientRandomSorter());
            GameSettings.PlayingClients = new ClientCollection();

            foreach (Client client in GameServer.Instance.Clients) //Skickar spelarna till varandra
            {
                client.Player.ChangePosition(GameServer.Instance.GameManager.CurrentMap.PlayerSpawnPoints[client.Player.Id]);
                GameServer.Instance.SendStartGame(client, false); //Starta spelet alla Clienter!!
                GameSettings.PlayingClients.Add(client);
            }

            for (int i = 0; i < GameServer.Instance.Clients.Count; i++)
            {
                GameServer.Instance.Clients[i].NewClient = false;
                GameServer.Instance.Clients[i].Spectating = false;
            }

            // Send players info to everyone
            foreach (Client client in GameServer.Instance.Clients) 
            {
                GameServer.Instance.SendPlayerInfo(client);
            }

            Program.Log.Info("[INITIALIZED GAME]");

            StartedMatch = true;
        }

        private void EndGame()
        {
            StartedMatch = false;
            foreach (Client client in GameServer.Instance.Clients)
            {
                //client.Player.nextDirection = LookDirection.Idle;
                GameServer.Instance.SendPlayerPosition(client.Player, false);
            }
        }

        private void RunGameLogic()
        {
            if (_gameHasBegun)
            {
                MovePlayers();

                GameServer.Instance.GameManager.Update();

                //CheckSuddenDeath();
            }
        }

        #region GameLogic
        private void MovePlayers()
        {
            foreach (Client client in GameServer.Instance.Clients)
            {
                // Move the player to the next position
                //Program.Log.Info("Player position: " + client.Player.Position);                
                client.Player.MovePlayer(GameServer.Instance.GameManager.CurrentMap);
            }
        }

        private void SuddenDeath_ChangeDirection()
        {
            /*
            if (suddenDeathMovement.X == 1)
            {
                suddenDeathMovement.X = 0;
                suddenDeathMovement.Y = 1;
            }
            else
            {
                if (suddenDeathMovement.X == -1)
                {
                    suddenDeathMovement.X = 0;
                    suddenDeathMovement.Y = -1;
                }
                else
                {
                    if (suddenDeathMovement.Y == 1)
                    {
                        suddenDeathMovement.X = -1;
                        suddenDeathMovement.Y = 0;
                    }
                    else
                    {
                        if (suddenDeathMovement.Y == -1)
                        {
                            suddenDeathMovement.X = 1;
                            suddenDeathMovement.Y = 0;
                        }
                    }
                }
            }
            */
        }
        private void CheckSuddenDeath()//Kollar när sudden death startar och skapar explosionerna som vandrar runt banan
        {
            /*
            #region BeforeSuddenDeath
            if (!suddenDeath)
            {
                if (tmr_UntilSuddenDeath.Each(_gameManager.CurrentMap.suddenDeathTime))
                {
                    suddenDeath = true;
                    //tmr_UntilSuddenDeath.Stop();
                    //tmr_ExplosionSuddenDeath = new H.U.D.Timer(true);
                    suddenDeathBomb = new OldBomb(null, null);
                    suddenDeathBomb.Exploded = true;
                    suddenDeathBomb.IsSuddenDeath = true;
                    suddenDeathTileX = 0;
                    suddenDeathTileY = 0;
                    suddenDeathMovement = new Vector2(1, 0);
                    //Säger till clienterna att det är sudden death
                    GameServer.Instance.SendSuddenDeath();
                    //GameManager.BombList.Add(suddenDeathBomb);
                    //Lägger till första explosionen
                    Explosion ex = new Explosion(Explosion.ExplosionType.Mid, _gameManager.CurrentMap.GetTileByTilePosition(0, 0));
                    suddenDeathBomb.Explosion.Add(ex);
                    GameServer.Instance.SendSDExplosion(ex);
                    //Sätter allas liv till 1
                    foreach (Client client in GameServer.Instance.Clients)
                    {
                        //client.Player.lifes = 1;
                    }
                }
            }
            #endregion

            #region AfterSuddenDeath
            if (suddenDeath)
            {
                if (tmr_ExplosionSuddenDeath.Each(suddenDeathTPE))
                {
                    MapTile tile = _gameManager.CurrentMap.GetTileByTilePosition(suddenDeathTileX + (int)suddenDeathMovement.X,
                        suddenDeathTileY + (int)suddenDeathMovement.Y);
                    if (tile != null)
                    {
                        if (suddenDeathBomb.Explosion.GetExplosionByTile(tile) != null)
                        {
                            SuddenDeath_ChangeDirection();
                        }
                    }
                    else
                    {
                        SuddenDeath_ChangeDirection();
                    }
                    suddenDeathTileX += (int)suddenDeathMovement.X;
                    suddenDeathTileY += (int)suddenDeathMovement.Y;
                    Explosion ex = new Explosion(Explosion.ExplosionType.Mid, _gameManager.CurrentMap.GetTileByTilePosition(suddenDeathTileX, suddenDeathTileY));
                    suddenDeathBomb.Explosion.Add(ex);
                    ex.Position.walkable = true;
                    GameServer.Instance.SendSDExplosion(ex);
                }
            }
            #endregion
            */
        }

        #endregion

        #region ServerEvents
        private void GameServer_ConnectedClient(Client sender, EventArgs e)
        {
            if (true /*GameServer.Instance.clients.Count <= GameSettings.Get_gameManager.CurrentMap().playerAmount*/)
            {
                var player = new Player(_playerId);
                GameServer.Instance.GameManager.AddPlayer(sender, player);

                GameServer.Instance.SendGameInfo(sender);
                if (StartedMatch)
                {
                    sender.Player.IsAlive = false;
                    sender.Spectating = true;
                    sender.NewClient = true;
                }

                _playerId++;

                GameServer.Instance.SendClientInfo(sender);
                //MainServer.SendCurrentPlayerAmount();
            }
            else
            {
                GameServer.Instance.Clients.Remove(sender);
                Program.Log.Info("[FULLGAME] Client tried to connect");
                sender.ClientConnection.Disconnect("Full Server");
            }
        }

        private void GameServer_DisconnectedClient(Client sender, EventArgs e)
        {
            if (StartedMatch)
            {
                sender.Player.IsAlive = false;
                GameServer.Instance.SendRemovePlayer(sender.Player);
            }
            //MainServer.SendCurrentPlayerAmount();
        }

        // An evil player wants to plant a bomb
        private void GameServer_BombPlacing(Client sender)
        {
            if (_gameHasBegun)
            {
                var player = sender.Player;

                if (player.CurrentBombAmount > 0)
                {
                    var bo = GameServer.Instance.GameManager.BombList.Find(b => b.CellPosition == player.CellPosition);

                    if (bo != null) return;

                    var bomb = new Bomb(player.Id, player.CellPosition, player.BombPower, player.BombTimer,
                        player.Speed);

                    bomb.Initialize(GameServer.Instance.GameManager.CurrentMap.Size, 
                                    GameServer.Instance.GameManager.CurrentMap.CollisionLayer, 
                                    GameServer.Instance.GameManager.HazardMap);

                    GameServer.Instance.GameManager.CurrentMap.Board[bomb.CellPosition.X, bomb.CellPosition.Y] = bomb;
                    GameServer.Instance.GameManager.CurrentMap.CollisionLayer[bomb.CellPosition.X, bomb.CellPosition.Y] = true;

                    GameServer.Instance.GameManager.AddBomb(bomb);
                    player.CurrentBombAmount--;

                    GameServer.Instance.SendPlayerPlacingBomb(sender.Player, bomb.CellPosition);
                }
            }
        }

        private void Bomb_IsExploded(Bomb bomb)
        {
            /*
            //_gameManager.CurrentMap.CalcBombExplosion(bomb);
            GameServer.Instance.SendBombExploded(bomb);
            bomb.Position.Bombed = null;

            //Kollar om bomben exploderar en annan bomb
            if (GameSettings.BombsExplodeBombs)
            {
                foreach (Explosion ex in bomb.Explosion)
                {
                    foreach (Bomb b in GameManager.BombList)
                    {
                        if (b.Position == ex.Position)
                        {
                            if (b.Exploded == false)
                                b.Explode();
                        }
                    }
                }
            }
            */
        }
        #endregion
    }
}

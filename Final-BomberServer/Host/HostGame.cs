using System.Diagnostics;
using FBLibrary;
using FBLibrary.Core;
using Final_BomberServer.Core;
using Final_BomberServer.Core.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final_BomberServer.Host
{
    partial class HostGame
    {
        bool hasStarted;
        int playerId = 0;

        Timer tmr_BeginGame;
        bool gameHasBegun;

        bool suddenDeath = false;
        int suddenDeathTPE;
        int suddenDeathTileX;
        int suddenDeathTileY;
        //H.U.D.Timer tmr_UntilSuddenDeath;
        //H.U.D.Timer tmr_ExplosionSuddenDeath;
        OldBomb suddenDeathBomb;
        Vector2 suddenDeathMovement;

        public static GameManager GameManager; 

        public bool HasStarted
        {
            get { return hasStarted; }
        }

        public bool StartedMatch = false;

        public void Initialize()
        {
            GameSettings.gameServer = new GameServer();
            GameSettings.gameServer.ConnectedClient += gameServer_ConnectedClient;
            GameSettings.gameServer.DisconnectedClient += gameServer_DisconnectedClient;
            GameSettings.gameServer.BombPlacing += gameServer_BombPlacing;
            GameSettings.gameServer.StartServer();

            GameManager = new GameManager();

            GameManager.LoadMap(MapLoader.MapFileDictionary.Keys.First());

            // Display info
            GameManager.CurrentMap.DisplayBoard();
            GameManager.CurrentMap.DisplayCollisionLayer();

            hasStarted = true;
        }

        public void Update()
        {
            GameSettings.gameServer.RunServer();

            GameStepProccesing();
            
            if (tmr_BeginGame != null)
            {
                // Game really starts after 3 seconds
                if (!gameHasBegun /*&& tmr_BeginGame.Each(3000)*/)
                {
                    tmr_BeginGame.Stop();
                    //tmr_UntilSuddenDeath.Start();
                    gameHasBegun = true;
                }
            }

            if (StartedMatch && gameHasBegun)
                RunGameLogic();
        }

        public void Dispose()
        {
            hasStarted = false;
            StartedMatch = false;
            GameSettings.gameServer.EndServer("Ending Game");
        }

        private void GameInitialize()
        {
            gameHasBegun = false;
            tmr_BeginGame = new Timer(true);
            //GameSettings.Get_gameManager.CurrentMap().CreateMap();
            //_gameManager.CurrentMap = GameSettings.Get_gameManager.CurrentMap();

            suddenDeath = false;
            //suddenDeathTPE = 1000 / ((_gameManager.CurrentMap.mapWidth * _gameManager.CurrentMap.mapHeight) / 30); //Räknar ut tiden mellan varje explosion (i ms)
            //tmr_UntilSuddenDeath = new H.U.D.Timer(false);
            GameSettings.gameServer.Clients.Sort(new ClientRandomSorter());
            GameSettings.PlayingClients = new ClientCollection();
            foreach (Client client in GameSettings.gameServer.Clients) //Skickar spelarna till varandra
            {
                client.Player.ChangePosition(GameManager.CurrentMap.PlayerSpawnPoints[client.Player.Id]);
                GameSettings.gameServer.SendStartGame(client, false); //Starta spelet alla Clienter!!
                GameSettings.PlayingClients.Add(client);
            }

            for (int i = 0; i < GameSettings.gameServer.Clients.Count; i++)
            {
                //GameSettings.gameServer.clients[i].Player.PositionOnTile(_gameManager.CurrentMap.startPositions[i]);//Placerar spelarna på start positioner...
                //GameSettings.gameServer.clients[i].Player.invurnable.Start();//... och ser till att alla är invurnable i början av spelet
                GameSettings.gameServer.Clients[i].NewClient = false; //Spelaren är inte nyconnectad längre
                GameSettings.gameServer.Clients[i].Spectating = false;
            }

            foreach (Client client in GameSettings.gameServer.Clients) //Skickar spelarna till varandra
            {
                GameSettings.gameServer.SendPlayerInfo(client);
            }

            Program.Log.Info("[INITIALIZED GAME]");
            
            StartedMatch = true;
        }

        private void EndGame()
        {
            StartedMatch = false;
            foreach (Client client in GameSettings.gameServer.Clients)
            {
                client.Player.nextDirection = LookDirection.Idle;
                GameSettings.gameServer.SendPlayerPosition(client.Player, false);
            }
        }

        private void RunGameLogic()
        {
            if (gameHasBegun)
            {
                MovePlayers();
                CheckBombTimer();
                CheckWalls();
                UpdatePlayers();
                /*
                CheckPlayerGettingPowerup();
                CheckSuddenDeath();
                */
            }
        }

        #region GameLogic
        private void MovePlayers()
        {
            foreach (Client client in GameSettings.gameServer.Clients)
            {
                // Move the player to the next position
                //Program.Log.Info("Player position: " + client.Player.Position);                
                client.Player.MovePlayer(GameManager.CurrentMap); 
            }
        }

        private void CheckBombTimer()
        {
            for (int i = 0; i < GameManager.BombList.Count; i++)
            {
                GameManager.BombList[i].Update();
                // Do we delete the bomb
                if (!GameManager.BombList[i].IsAlive)
                {
                    // TODO: Finish this part !
                    //_gameManager._gameManager.CurrentMap.CheckToRemoveExplodedTiles(_gameManager.BombList[i]); //Kollar om den spränger bort någon tile
                    // The player get back his bomb
                    GameSettings.gameServer.Clients.GetPlayerFromId(GameManager.BombList[i].PlayerId).CurrentBombAmount++;
                    GameManager.CurrentMap.CollisionLayer[GameManager.BombList[i].CellPositionX, GameManager.BombList[i].CellPositionY] = false;
                    GameManager.CurrentMap.Board[
                        GameManager.BombList[i].CellPositionX, GameManager.BombList[i].CellPositionY] = null;
                    GameManager.BombList.RemoveAt(i);
                }
            }
        }

        private void CheckWalls()
        {
            #region Walls

            for (int i = 0; i < GameManager.WallList.Count; i++)
            {
                // Is it die ?
                if (GameManager.HazardMap[GameManager.WallList[i].CellPosition.X, GameManager.WallList[i].CellPosition.Y] == 3)
                {
                    GameManager.HazardMap[GameManager.WallList[i].CellPosition.X, GameManager.WallList[i].CellPosition.Y] = 0;
                    GameManager.WallList[i].Destroy();
                }

                // We clean the obsolete elements
                if (!GameManager.WallList[i].IsAlive)
                {
                    GameManager.CurrentMap.CollisionLayer[GameManager.WallList[i].CellPosition.X, GameManager.WallList[i].CellPosition.Y] = false;
                    /*
                    if (Random.Next(0, 100) < MathHelper.Clamp(Config.ItemNumber, 0, 100))
                    {
                        var powerUp = new PowerUp(_wallList[i].CellPosition);
                        _powerUpList.Add(powerUp);
                        CurrentMap.Board[_wallList[i].CellPosition.X, _wallList[i].CellPosition.Y] = powerUp;
                    }
                    else
                    {*/
                    GameManager.CurrentMap.Board[GameManager.WallList[i].CellPosition.X, GameManager.WallList[i].CellPosition.Y] = null;
                    //}

                    GameManager.WallList.Remove(GameManager.WallList[i]);
                }
            }

            #endregion
        }

        private void UpdatePlayers()
        {
            #region Player

            List<Player> players = GameSettings.gameServer.Clients.GetAlivePlayers();

            for (int i = 0; i < players.Count; i++)
            {
                players[i].Update();

                // Is it die ?
                if (!players[i].InDestruction && !players[i].IsInvincible && 
                    GameManager.HazardMap[players[i].CellPosition.X, players[i].CellPosition.Y] == 3)
                {
                    int bombId = -42;
                    List<Bomb> bl = GameManager.BombList.FindAll(b => b.InDestruction);
                    foreach (Bomb b in bl)
                    {
                        if (b.ActionField.Any(po => po == players[i].CellPosition))
                        {
                            bombId = b.PlayerId;
                        }
                    }
                    // Suicide
                    if (bombId == players[i].Id)
                        players[i].Stats.Suicides++;
                    else if (bombId >= 0 && bombId < players.Count)
                    {
                        players[i].Stats.Kills++;
                        Player player = players.Find(p => p.Id == bombId);
                        /*
                        if (player.OnEdge)
                        {
                            player.Rebirth(players[i].Position);
                            _deadPlayersNumber--;
                        }
                        */
                    }
                    players[i].Destroy();
                }
                
                // We clean the obsolete players
                if (!players[i].IsAlive)
                {
                    /*
                    if (!players[i].OnEdge)
                    {
                        if (CurrentMap.Board[players[i].CellPosition.X, players[i].CellPosition.Y] is Player)
                        {
                            var p = (Player)CurrentMap.Board[players[i].CellPosition.X, players[i].CellPosition.Y];
                            if (p.Id == players[i].Id)
                                CurrentMap.Board[players[i].CellPosition.X, players[i].CellPosition.Y] = null;
                        }
                        _deadPlayersNumber++;
                    }
                                        
                    if (true) //(Config.ActiveSuddenDeath && SuddenDeath.HasStarted))
                        players.Remove(players[i]);
                    else
                        players[i].OnEdge = true;
                    */
                }
            }

            #endregion

            /*
            foreach (Bomb bomb in GameManager.BombList)
            {
                if (bomb.Exploded)
                {
                    foreach (Explosion ex in bomb.Explosion)
                    {
                        List<Player> players = GameSettings.gameServer.Clients.GetPlayersFromTile(ex.Position, true);
                        foreach (Player player in players)
                        {
                            player.Burn(bomb);
                        }
                    }
                }
            }
            */
        }

        private void CheckPlayerGettingPowerup()
        {
            Vector2 pos;
            MapTile tile;
            foreach (Client client in GameSettings.gameServer.Clients)
            {
                /*
                pos = client.Player.GetCenterPosition();
                tile = GameSettings.Get_gameManager.CurrentMap().GetTileByPosition(pos.X, pos.Y);
                if (tile != null) //Om spelaren är död så kommer tilen att vara null
                {
                    if (tile.Poweruped != null)
                    {
                        tile.Poweruped.GetPowerup(client.Player);
                        GameSettings.gameServer.SendPowerupPick(client.Player, tile);
                        tile.Poweruped = null;
                    }
                }
                */
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
            #region BeforeSuddenDeath
            if (!suddenDeath)
            {
                if (false /*tmr_UntilSuddenDeath.Each(_gameManager.CurrentMap.suddenDeathTime)*/)
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
                    GameSettings.gameServer.SendSuddenDeath();
                    //GameManager.BombList.Add(suddenDeathBomb);
                    //Lägger till första explosionen
                    Explosion ex = new Explosion(Explosion.ExplosionType.Mid, null/*_gameManager.CurrentMap.GetTileByTilePosition(0, 0)*/);
                    suddenDeathBomb.Explosion.Add(ex);
                    GameSettings.gameServer.SendSDExplosion(ex);
                    //Sätter allas liv till 1
                    foreach (Client client in GameSettings.gameServer.Clients)
                    {
                        //client.Player.lifes = 1;
                    }
                }
            }
            #endregion

            #region AfterSuddenDeath
            if (suddenDeath)
            {
                if (true /*tmr_ExplosionSuddenDeath.Each(suddenDeathTPE)*/)
                {
                    MapTile tile = null /*_gameManager.CurrentMap.GetTileByTilePosition(suddenDeathTileX + (int)suddenDeathMovement.X,
                        suddenDeathTileY + (int)suddenDeathMovement.Y)*/;
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
                    Explosion ex = new Explosion(Explosion.ExplosionType.Mid, null /*_gameManager.CurrentMap.GetTileByTilePosition(suddenDeathTileX, suddenDeathTileY)*/);
                    suddenDeathBomb.Explosion.Add(ex);
                    ex.Position.walkable = true;
                    GameSettings.gameServer.SendSDExplosion(ex);
                }
            }
            #endregion
        }

        #endregion

        #region ServerEvents
        private void gameServer_ConnectedClient(Client sender, EventArgs e)
        {
            if (true /*GameSettings.gameServer.clients.Count <= GameSettings.Get_gameManager.CurrentMap().playerAmount*/)
            {
                sender.Player = new Player(playerId);
                GameSettings.gameServer.SendGameInfo(sender);
                if (StartedMatch)
                {
                    
                    sender.Player.IsAlive = false;
                    sender.Spectating = true;
                    sender.NewClient = true;
                }

                playerId++;
                //MainServer.SendCurrentPlayerAmount();
            }
            else
            {
                GameSettings.gameServer.Clients.Remove(sender);
                GameSettings.gameServer.WriteOutput("[FULLGAME] Client tried to connect");
                sender.ClientConnection.Disconnect("Full Server");
            }
        }

        private void gameServer_DisconnectedClient(Client sender, EventArgs e)
        {
            if (StartedMatch)
            {
                sender.Player.IsAlive = false;
                GameSettings.gameServer.SendRemovePlayer(sender.Player);
            }
            //MainServer.SendCurrentPlayerAmount();
        }

        // An evil player wants to plant a bomb
        private void gameServer_BombPlacing(Client sender)
        {
            if (gameHasBegun)
            {
                var player = sender.Player;

                if (player.CurrentBombAmount > 0)
                {
                    var bo = GameManager.BombList.Find(b => b.CellPosition == player.CellPosition);

                    if (bo == null)
                    {
                        var bomb = new Bomb(player.Id, player.CellPosition, player.BombPower, player.BombTimer,
                            player.Speed);

                        bomb.Initialize(GameManager.CurrentMap, GameManager.HazardMap);

                        //bomb.IsExploded += new Bomb.IsExplodedEventHandler(bomb_IsExploded);

                        GameManager.CurrentMap.Board[bomb.CellPosition.X, bomb.CellPosition.Y] = bomb;
                        GameManager.CurrentMap.CollisionLayer[bomb.CellPosition.X, bomb.CellPosition.Y] = true;

                        GameManager.BombList.Add(bomb);
                        player.CurrentBombAmount--;

                        GameSettings.gameServer.SendPlayerPlacingBomb(sender.Player, bomb.CellPosition);
                    }
                }
                /*
                if (sender.Player.CurrentBombAmount < sender.Player.TotalBombAmount)
                {
                    Vector2 cpos = sender.Player.GetCenterPosition();
                    MapTile pos = null;
                    List<Player> players = GameSettings.gameServer.Clients.GetPlayersFromTile(pos, true);
                    if (players.Count < 2)
                    {
                        if (pos.Bombed == null && pos.walkable)
                        {
                            var bomb = new Bomb(pos, sender.Player);
                            bomb.IsExploded += new Bomb.IsExplodedEventHandler(bomb_IsExploded);
                            GameManager.BombList.Add(bomb);
                            sender.Player.CurrentBombAmount++;
                            pos.Bombed = bomb;

                            GameSettings.gameServer.SendPlayerPlacingBomb(sender.Player, pos.GetMapPos().X, pos.GetMapPos().Y);
                        }
                    }
                }
                */
            }
        }

        private void bomb_IsExploded(OldBomb bomb)
        //Bombs har precis exploderat, skickar till clienterna att den har exploderat och hur
        {
            /*
            //_gameManager.CurrentMap.CalcBombExplosion(bomb);
            GameSettings.gameServer.SendBombExploded(bomb);
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

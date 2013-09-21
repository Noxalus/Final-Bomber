﻿using System.Diagnostics;
using FBLibrary.Core;
using Final_BomberServer.Core;
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
        List<Bomb> bombs;
        Map CurrentMap;

        Timer tmr_BeginGame;
        bool gameHasBegun;

        bool suddenDeath = false;
        int suddenDeathTPE;
        int suddenDeathTileX;
        int suddenDeathTileY;
        //H.U.D.Timer tmr_UntilSuddenDeath;
        //H.U.D.Timer tmr_ExplosionSuddenDeath;
        Bomb suddenDeathBomb;
        Vector2 suddenDeathMovement;

        public bool HasStarted
        {
            get { return hasStarted; }
        }

        public bool StartedMatch = false;

        public void Initialize()
        {
            GameSettings.gameServer = new GameServer();
            GameSettings.gameServer.ConnectedClient += new GameServer.ConnectedClientEventHandler(gameServer_ConnectedClient);
            GameSettings.gameServer.DisconnectedClient += new GameServer.DisconnectedClientEventHandler(gameServer_DisconnectedClient);
            GameSettings.gameServer.BombPlacing += new GameServer.BombPlacingEventHandler(gameServer_BombPlacing);
            GameSettings.gameServer.StartServer();
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
            bombs = new List<Bomb>();
            //GameSettings.GetCurrentMap().CreateMap();
            //CurrentMap = GameSettings.GetCurrentMap();
            suddenDeath = false;
            //suddenDeathTPE = 1000 / ((CurrentMap.mapWidth * CurrentMap.mapHeight) / 30); //Räknar ut tiden mellan varje explosion (i ms)
            //tmr_UntilSuddenDeath = new H.U.D.Timer(false);
            GameSettings.gameServer.Clients.Sort(new ClientRandomSorter());
            GameSettings.PlayingClients = new ClientCollection();
            foreach (Client client in GameSettings.gameServer.Clients) //Skickar spelarna till varandra
            {
                GameSettings.gameServer.SendStartGame(client, false); //Starta spelet alla Clienter!!
                GameSettings.PlayingClients.Add(client);
            }
            for (int i = 0; i < GameSettings.gameServer.Clients.Count; i++)
            {
                //GameSettings.gameServer.clients[i].Player.PositionOnTile(CurrentMap.startPositions[i]);//Placerar spelarna på start positioner...
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
                /*
                CheckBombTimer();
                CheckPlayerGettingBurned();
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
                client.Player.MovePlayer(); 
            }
        }

        private void CheckBombTimer()
        {
            for (int i = 0; i < bombs.Count; i++)
            {
                bombs[i].CheckTick();
                if (bombs[i].Remove)//När den ska tas bort
                {
                    CurrentMap.CheckToRemoveExplodedTiles(bombs[i]); //Kollar om den spränger bort någon tile
                    bombs[i].player.CurrentBombAmount--; //Så spelaren kan lägga en bomb igen
                    bombs.RemoveAt(i); //Ta bort bomben
                }
            }
        }

        private void CheckPlayerGettingBurned()
        {
            foreach (Bomb bomb in bombs)
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
        }

        private void CheckPlayerGettingPowerup()
        {
            Vector2 pos;
            MapTile tile;
            foreach (Client client in GameSettings.gameServer.Clients)
            {
                /*
                pos = client.Player.GetCenterPosition();
                tile = GameSettings.GetCurrentMap().GetTileByPosition(pos.X, pos.Y);
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
                if (false /*tmr_UntilSuddenDeath.Each(CurrentMap.suddenDeathTime)*/)
                {
                    suddenDeath = true;
                    //tmr_UntilSuddenDeath.Stop();
                    //tmr_ExplosionSuddenDeath = new H.U.D.Timer(true);
                    suddenDeathBomb = new Bomb(null, null);
                    suddenDeathBomb.Exploded = true;
                    suddenDeathBomb.IsSuddenDeath = true;
                    suddenDeathTileX = 0;
                    suddenDeathTileY = 0;
                    suddenDeathMovement = new Vector2(1, 0);
                    //Säger till clienterna att det är sudden death
                    GameSettings.gameServer.SendSuddenDeath();
                    bombs.Add(suddenDeathBomb);
                    //Lägger till första explosionen
                    Explosion ex = new Explosion(Explosion.ExplosionType.Mid, CurrentMap.GetTileByTilePosition(0, 0));
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
                    MapTile tile = CurrentMap.GetTileByTilePosition(suddenDeathTileX + (int)suddenDeathMovement.X,
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
                    Explosion ex = new Explosion(Explosion.ExplosionType.Mid, CurrentMap.GetTileByTilePosition(suddenDeathTileX, suddenDeathTileY));
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
            if (true /*GameSettings.gameServer.clients.Count <= GameSettings.GetCurrentMap().playerAmount*/)
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

        private void gameServer_BombPlacing(Client sender) //En client vill lägga en bomb
        {
            if (gameHasBegun)
            {
                if (sender.Player.CurrentBombAmount < sender.Player.TotalBombAmount)
                {
                    Vector2 cpos = sender.Player.GetCenterPosition();
                    MapTile pos = CurrentMap.GetTileByPosition(cpos.X, cpos.Y);
                    List<Player> players = GameSettings.gameServer.Clients.GetPlayersFromTile(pos, true);
                    if (players.Count < 2)
                    {
                        if (pos.Bombed == null && pos.walkable)
                        {
                            Bomb bomb = new Bomb(pos, sender.Player);
                            bomb.IsExploded += new Bomb.IsExplodedEventHandler(bomb_IsExploded);
                            bombs.Add(bomb);
                            sender.Player.CurrentBombAmount++;
                            pos.Bombed = bomb;
                            GameSettings.gameServer.SendPlayerPlacingBomb(sender.Player, pos.GetMapPos().X, pos.GetMapPos().Y);
                        }
                    }
                }
            }
        }

        private void bomb_IsExploded(Bomb bomb)
        //Bombs har precis exploderat, skickar till clienterna att den har exploderat och hur
        {
            CurrentMap.CalcBombExplosion(bomb);
            GameSettings.gameServer.SendBombExploded(bomb);
            bomb.Position.Bombed = null;

            //Kollar om bomben exploderar en annan bomb
            if (GameSettings.BombsExplodeBombs)
            {
                foreach (Explosion ex in bomb.Explosion)
                {
                    foreach (Bomb b in bombs)
                    {
                        if (b.Position == ex.Position)
                        {
                            if (b.Exploded == false)
                                b.Explode();
                        }
                    }
                }
            }
        }
        #endregion
    }
}

using System.Collections.Generic;
using System.IO;
using FBLibrary;
using Final_BomberServer.Core;
using Final_BomberServer.Core.Entities;
using Final_BomberServer.Core.WorldEngine;
using Lidgren.Network;
using Lidgren.Network.Xna;
using Microsoft.Xna.Framework;
using System.Linq;

namespace Final_BomberServer.Host
{
    partial class GameServer
    {
        public void SendGameInfo(Client client)
        {
            try
            {
                if (client.ClientConnection.Status == NetConnectionStatus.Connected)
                {
                    NetOutgoingMessage send = server.CreateMessage();
                    send.Write((byte)SMT.GameStartInfo);
                    send.Write(MapLoader.MapFileDictionary.Values.First());
                    Program.Log.Info("Sended game info map [" + MapLoader.MapFileDictionary.Values.First() + "]");
                    server.SendMessage(send, client.ClientConnection, NetDeliveryMethod.ReliableOrdered);
                }
            }
            catch (NetException e)
            {
                WriteOutput("NET EXCEPTION: " + e.ToString());
            }
        }

        public void SendCurrentMap(Client client)
        {
            if (client.ClientConnection.Status == NetConnectionStatus.Connected)
            {
                NetOutgoingMessage send = server.CreateMessage();
                send.Write((byte)SMT.Map);

                send.Write(HostGame.GameManager.CurrentMap.Name);
                send.Write(HostGame.GameManager.CurrentMap.GetMd5());

                string path = "Content/Maps/" + HostGame.GameManager.CurrentMap.Name;
                byte[] mapData = File.ReadAllBytes(path);
                
                send.Write(mapData.Length);
                foreach (var bt in mapData)
                {
                    send.Write(bt);
                }

                server.SendMessage(send, client.ClientConnection, NetDeliveryMethod.ReliableOrdered);
            }
        }

        public void SendStartGame(Client client, bool gameinProgress)
        {
            NetOutgoingMessage send = server.CreateMessage();
            send.Write((byte)SMT.StartGame);
            send.Write(gameinProgress);
            if (!gameinProgress)
            {
                send.Write(client.Player.Id);
                send.Write(client.Player.Speed);
                send.Write(999/*GameSettings.GetCurrentMap().suddenDeathTime*/);

                List<Wall> walls = HostGame.GameManager.WallList;
                send.Write(walls.Count);
                foreach (var wall in walls)
                {
                    send.Write(wall.CellPosition);
                }
            }

            server.SendMessage(send, client.ClientConnection, NetDeliveryMethod.ReliableOrdered);
        }

        // Send all players to this player
        public void SendPlayersToNew(Client client) 
        {
            foreach (Client player in Clients)
            {
                if (client != player)
                {
                    NetOutgoingMessage send = GetPlayerInfo(player);
                    server.SendMessage(send, client.ClientConnection, NetDeliveryMethod.ReliableOrdered);
                    SendPlayerPosition(player.Player, false);
                }
            }
        }

        // Send this player to all other available
        public void SendPlayerInfo(Client client) 
        {
            if (client.ClientConnection.Status == NetConnectionStatus.Connected)
            {
                NetOutgoingMessage send = GetPlayerInfo(client);
                server.SendToAll(send, client.ClientConnection, NetDeliveryMethod.ReliableOrdered, 0);
                SendPlayerPosition(client.Player, false);
            }
        }

        public void SendRemovePlayer(Player removePlayer)
        {
            NetOutgoingMessage send = server.CreateMessage();
            send.Write((byte)SMT.RemovePlayer);
            send.Write(removePlayer.Id);
            server.SendToAll(send, NetDeliveryMethod.ReliableOrdered);
        }

        #region GetPlayerInfo
        private NetOutgoingMessage GetPlayerInfo(Client client)
        {
            NetOutgoingMessage rtn = server.CreateMessage();
            rtn.Write((byte)SMT.PlayerInfo);
            rtn.Write(client.Player.Id);
            rtn.Write(client.Player.Speed);
            rtn.Write(client.Username);
            return rtn;
        }
        #endregion

        // Send the player's movement to all other players
        public void SendPlayerPosition(Player player, bool notDir)
        {
            NetOutgoingMessage send = server.CreateMessage();

            send.Write((byte)SMT.PlayerPosAndSpeed);

            send.Write(player.Position.X);
            send.Write(player.Position.Y);

            if (!notDir)
            {
                send.Write((byte)player.nextDirection);
            }
            else
            {
                send.Write((byte)255);
            }

            send.Write(player.Id);
            
            server.SendToAll(send, NetDeliveryMethod.ReliableOrdered);
            Program.Log.Info("Send Position [Player ID: " + player.Id + " => " + player.Position + "]");
        }

        // Send to all players that this player has placed a bomb
        public void SendPlayerPlacingBomb(Player player, float xPos, float yPos)
        {
            NetOutgoingMessage send = server.CreateMessage();
            send.Write((byte)SMT.PlayerPlacingBomb);
            send.Write(player.Id);
            send.Write(xPos);
            send.Write(yPos);
            server.SendToAll(send, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendBombExploded(Bomb bomb)//Skickar till alla spelare att bomben på denna position har exploderat och hur den exploderar
        {
            NetOutgoingMessage send = server.CreateMessage();
            send.Write((byte)SMT.BombExploded);
            send.Write(bomb.Position.GetMapPos().X);
            send.Write(bomb.Position.GetMapPos().Y);
            send.Write(bomb.Explosion.Count);
            foreach (Explosion ex in bomb.Explosion)
            {
                send.Write(ex.Position.GetMapPos().X);
                send.Write(ex.Position.GetMapPos().Y);
                send.Write((byte)ex.explosionType);
            }
            server.SendToAll(send, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendPlayerGotBurned(Player player)
        {
            NetOutgoingMessage send = server.CreateMessage();
            send.Write((byte)SMT.Burn);
            send.Write(player.Id);
            server.SendToAll(send, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendExplodeTile(int tilePos)
        {
            NetOutgoingMessage send = server.CreateMessage();
            send.Write((byte)SMT.ExplodeTile);
            send.Write(tilePos);
            server.SendToAll(send, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendPowerupDrop(MapTile tile)
        {
            NetOutgoingMessage send = server.CreateMessage();
            send.Write((byte)SMT.PowerupDrop);
            send.Write((byte)tile.Poweruped.Type);
            Vector2 pos = tile.GetMapPos();
            send.Write(pos.X);
            send.Write(pos.Y);
            server.SendToAll(send, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendPowerupPick(Player player, MapTile tile)
        {
            NetOutgoingMessage send = server.CreateMessage();
            send.Write((byte)SMT.PowerupPick);
            Vector2 pos = tile.GetMapPos();
            send.Write(pos.X);
            send.Write(pos.Y);
            send.Write(player.Id);
            send.Write(tile.Poweruped.GetPowerupValue());
            server.SendToAll(send, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendSuddenDeath()
        {
            NetOutgoingMessage send = server.CreateMessage();
            send.Write((byte)SMT.SuddenDeath);
            server.SendToAll(send, NetDeliveryMethod.ReliableOrdered);
            WriteOutput("SUDDEN DEATH!");
        }

        public void SendSDExplosion(Explosion ex)
        {
            NetOutgoingMessage send = server.CreateMessage();
            send.Write((byte)SMT.SDExplosion);
            send.Write(ex.Position.GetListPos());
            server.SendToAll(send, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendEnd(Client client)
        {
            if (client.ClientConnection.Status == NetConnectionStatus.Connected)
            {
                NetOutgoingMessage send = server.CreateMessage();
                send.Write((byte)SMT.End);
                send.Write(client.Player.IsAlive);
                server.SendMessage(send, client.ClientConnection, NetDeliveryMethod.ReliableOrdered);
            }
        }
    }
}

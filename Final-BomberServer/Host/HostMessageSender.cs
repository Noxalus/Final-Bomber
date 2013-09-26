using System;
using System.Collections.Generic;
using System.IO;
using FBLibrary;
using FBLibrary.Core;
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
                send.Write(GameConfiguration.SuddenDeathTimer.Milliseconds);

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

            send.Write((byte)player.CurrentDirection);

            send.Write(player.Id);

            server.SendToAll(send, NetDeliveryMethod.ReliableOrdered);
            Program.Log.Info("Send Position of player #" + player.Id + " !");
        }

        // Send to all players that this player has placed a bomb
        public void SendPlayerPlacingBomb(Player player, Point position)
        {
            NetOutgoingMessage send = server.CreateMessage();
            send.Write((byte)SMT.PlayerPlacingBomb);
            send.Write(player.Id);
            send.Write(position);
            server.SendToAll(send, NetDeliveryMethod.ReliableOrdered);
        }

        // Send to all players that a bomb has blow up
        public void SendBombExploded(Bomb bomb)
        {
            NetOutgoingMessage send = server.CreateMessage();

            send.Write((byte)SMT.BombExploded);
            send.Write(bomb.CellPosition);

            /*
            send.Write(bomb.Explosion.Count);

            foreach (Explosion ex in bomb.Explosion)
            {
                send.Write(ex.Position.GetMapPos().X);
                send.Write(ex.Position.GetMapPos().Y);
                send.Write((byte)ex.explosionType);
            }
            */

            server.SendToAll(send, NetDeliveryMethod.ReliableOrdered);

            Program.Log.Info("Send bomb exploded to everyone !");
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

        public void SendPowerUpDrop(PowerUp powerUp)
        {
            NetOutgoingMessage send = server.CreateMessage();

            send.Write((byte)SMT.PowerUpDrop);

            send.Write((byte)powerUp.Type);
            send.Write(powerUp.CellPosition);

            server.SendToAll(send, NetDeliveryMethod.ReliableOrdered);
            Program.Log.Info("Power up dropped !");
        }

        public void SendPowerUpPick(Player player, PowerUp powerUp)
        {
            NetOutgoingMessage send = server.CreateMessage();

            send.Write((byte)SMT.PowerUpPick);

            send.Write(player.Id);
            send.Write(powerUp.CellPosition);

            server.SendToAll(send, NetDeliveryMethod.ReliableOrdered);
            Program.Log.Info("Power up pick by player #" + player.Id + " !");
        }

        public void SendSuddenDeath()
        {
            NetOutgoingMessage send = server.CreateMessage();
            send.Write((byte)SMT.SuddenDeath);
            server.SendToAll(send, NetDeliveryMethod.ReliableOrdered);
            WriteOutput("SUDDEN DEATH!");
        }

        /*
        public void SendSDExplosion(Explosion ex)
        {
            NetOutgoingMessage send = server.CreateMessage();
            send.Write((byte)SMT.SDExplosion);
            send.Write(ex.Position.GetListPos());
            server.SendToAll(send, NetDeliveryMethod.ReliableOrdered);
        }
        */

        public void SendRoundEnd(Client client)
        {
            if (client.ClientConnection.Status == NetConnectionStatus.Connected)
            {
                NetOutgoingMessage send = server.CreateMessage();

                send.Write((byte)SMT.RoundEnd);

                server.SendMessage(send, client.ClientConnection, NetDeliveryMethod.ReliableOrdered);
                Program.Log.Info("Send 'RoundEnd' to player #" + client.Player.Id);
            }
        }

        public void SendEnd(Client client)
        {
            if (client.ClientConnection.Status == NetConnectionStatus.Connected)
            {
                NetOutgoingMessage send = server.CreateMessage();

                send.Write((byte)SMT.End);

                send.Write(client.Player.IsAlive);

                server.SendMessage(send, client.ClientConnection, NetDeliveryMethod.ReliableOrdered);
                Program.Log.Info("Send 'End' to player #" + client.Player.Id);
            }
        }
    }
}

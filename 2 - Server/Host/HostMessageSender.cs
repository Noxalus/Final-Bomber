using System;
using System.Collections.Generic;
using System.IO;
using FBLibrary;
using FBLibrary.Core;
using FBServer.Core;
using FBServer.Core.Entities;
using FBServer.Core.WorldEngine;
using Lidgren.Network;
using Lidgren.Network.Xna;
using Microsoft.Xna.Framework;
using System.Linq;

namespace FBServer.Host
{
    sealed partial class GameServer
    {
        public void SendGameInfo(Client client)
        {
            try
            {
                if (client.ClientConnection.Status == NetConnectionStatus.Connected)
                {
                    NetOutgoingMessage send = _server.CreateMessage();

                    send.Write((byte)SMT.GameStartInfo);
                    send.Write(MapLoader.MapFileDictionary.Values.First());

                    _server.SendMessage(send, client.ClientConnection, NetDeliveryMethod.ReliableOrdered);

                    Program.Log.Info("Sended game info map [" + MapLoader.MapFileDictionary.Values.First() + "]");
                }
            }
            catch (NetException e)
            {
                Program.Log.Info("NET EXCEPTION: " + e.ToString());
            }
        }

        public void SendCurrentMap(Client client)
        {
            if (client.ClientConnection.Status == NetConnectionStatus.Connected)
            {
                NetOutgoingMessage send = _server.CreateMessage();
                send.Write((byte)SMT.Map);

                send.Write(Instance.GameManager.CurrentMap.Name);
                send.Write(Instance.GameManager.CurrentMap.GetMd5());

                string path = "Content/Maps/" + Instance.GameManager.CurrentMap.Name;
                byte[] mapData = File.ReadAllBytes(path);

                send.Write(mapData.Length);
                foreach (var bt in mapData)
                {
                    send.Write(bt);
                }

                _server.SendMessage(send, client.ClientConnection, NetDeliveryMethod.ReliableOrdered);
                Program.Log.Info("Send the map to client #" + client.ClientId);
            }
        }

        public void SendStartGame(Client client, bool gameInProgress)
        {
            NetOutgoingMessage send = _server.CreateMessage();
            send.Write((byte)SMT.StartGame);
            send.Write(gameInProgress);
            if (!gameInProgress)
            {
                send.Write(client.Player.Id);
                send.Write(client.Player.Speed);
                send.Write(GameConfiguration.SuddenDeathTimer.Milliseconds);

                List<Wall> walls = GameServer.Instance.GameManager.WallList;
                send.Write(walls.Count);
                foreach (var wall in walls)
                {
                    send.Write(wall.CellPosition);
                }
            }

            _server.SendMessage(send, client.ClientConnection, NetDeliveryMethod.ReliableOrdered);
            Program.Log.Info("Send start game to client #" + client.ClientId);
        }

        // Send all players to this player
        public void SendPlayersToNew(Client client)
        {
            Program.Log.Info("Send the players to the new player (client #" + client.ClientId + ")");

            foreach (Client player in Clients)
            {
                if (client != player)
                {
                    NetOutgoingMessage send = GetPlayerInfo(player);
                    _server.SendMessage(send, client.ClientConnection, NetDeliveryMethod.ReliableOrdered);
                    SendPlayerPosition(player.Player, false);

                    Program.Log.Info("Send the player (client #" + player.ClientId + ") to the new player (client #" + client.ClientId + ")");
                }
            }

        }

        // Send this client to all other available
        public void SendClientInfo(Client client)
        {
            if (client.ClientConnection.Status == NetConnectionStatus.Connected)
            {
                NetOutgoingMessage send = _server.CreateMessage();

                send.Write((byte)SMT.PlayerInfo);
                send.Write(client.ClientId);
                
                _server.SendToAll(send, client.ClientConnection, NetDeliveryMethod.ReliableOrdered, 0);
                
                Program.Log.Info("Send the clients to the new client (client #" + client.ClientId + ")");
            }
        }

        // Send this player to all other available
        public void SendPlayerInfo(Client client)
        {
            if (client.ClientConnection.Status == NetConnectionStatus.Connected)
            {
                NetOutgoingMessage send = GetPlayerInfo(client);
                _server.SendToAll(send, client.ClientConnection, NetDeliveryMethod.ReliableOrdered, 0);
                SendPlayerPosition(client.Player, false);

                Program.Log.Info("Send the players to the new player (client #" + client.ClientId + ")");
            }
        }

        public void SendRemovePlayer(Player removedPlayer)
        {
            NetOutgoingMessage send = _server.CreateMessage();

            send.Write((byte)SMT.RemovePlayer);
            send.Write(removedPlayer.Id);

            _server.SendToAll(send, NetDeliveryMethod.ReliableOrdered);

            Program.Log.Info("Send that the player #" + removedPlayer.Id + " is dead !");
        }

        #region GetPlayerInfo
        private NetOutgoingMessage GetPlayerInfo(Client client)
        {
            NetOutgoingMessage rtn = _server.CreateMessage();
            
            rtn.Write((byte)SMT.PlayerInfo);

            rtn.Write(client.Player.Id);
            rtn.Write(client.Player.Speed);
            rtn.Write(client.Username);
            rtn.Write(client.Player.Stats.Score);

            return rtn;
        }
        #endregion

        // Send the player's movement to all other players
        public void SendPlayerPosition(Player player, bool notDir)
        {
            NetOutgoingMessage send = _server.CreateMessage();

            send.Write((byte)SMT.PlayerPosition);

            send.Write(player.Position.X);
            send.Write(player.Position.Y);

            send.Write((byte)player.CurrentDirection);

            send.Write(player.Id);

            _server.SendToAll(send, NetDeliveryMethod.ReliableOrdered);

            Program.Log.Info("Send position of player #" + player.Id + " !");
        }

        // Send to all players that this player has placed a bomb
        public void SendPlayerPlacingBomb(Player player, Point position)
        {
            NetOutgoingMessage send = _server.CreateMessage();

            send.Write((byte)SMT.PlayerPlacingBomb);
            send.Write(player.Id);
            send.Write(position);

            _server.SendToAll(send, NetDeliveryMethod.ReliableOrdered);

            Program.Log.Info("Send that player #" + player.Id + " has planted a bomb !");
        }

        // Send to all players that a bomb has blow up
        public void SendBombExploded(Bomb bomb)
        {
            NetOutgoingMessage send = _server.CreateMessage();

            send.Write((byte)SMT.BombExploded);
            send.Write(bomb.CellPosition);

            _server.SendToAll(send, NetDeliveryMethod.ReliableOrdered);

            Program.Log.Info("Send that bomb exploded to everyone ! (position: " + bomb.Position + ")");
        }

        public void SendPowerUpDrop(PowerUp powerUp)
        {
            NetOutgoingMessage send = _server.CreateMessage();

            send.Write((byte)SMT.PowerUpDrop);

            send.Write((byte)powerUp.Type);
            send.Write(powerUp.CellPosition);

            _server.SendToAll(send, NetDeliveryMethod.ReliableOrdered);

            Program.Log.Info("Power up dropped ! (type: " + powerUp.Type + "|position: " + powerUp.CellPosition + ")");
        }

        public void SendPowerUpPick(Player player, PowerUp powerUp)
        {
            NetOutgoingMessage send = _server.CreateMessage();

            send.Write((byte)SMT.PowerUpPick);

            send.Write(player.Id);
            send.Write(powerUp.CellPosition);

            _server.SendToAll(send, NetDeliveryMethod.ReliableOrdered);

            Program.Log.Info("Power up pick by player #" + player.Id + " !");
        }

        public void SendSuddenDeath()
        {
            NetOutgoingMessage send = _server.CreateMessage();
            send.Write((byte)SMT.SuddenDeath);
            _server.SendToAll(send, NetDeliveryMethod.ReliableOrdered);
            Program.Log.Info("SUDDEN DEATH!");
        }

        public void SendRoundEnd(Client client)
        {
            if (client.ClientConnection.Status == NetConnectionStatus.Connected)
            {
                NetOutgoingMessage send = _server.CreateMessage();

                send.Write((byte)SMT.RoundEnd);

                _server.SendMessage(send, client.ClientConnection, NetDeliveryMethod.ReliableOrdered);
                Program.Log.Info("Send 'RoundEnd' to player #" + client.Player.Id);
            }
        }

        public void SendEnd(Client client)
        {
            if (client.ClientConnection.Status == NetConnectionStatus.Connected)
            {
                NetOutgoingMessage send = _server.CreateMessage();

                send.Write((byte)SMT.End);

                send.Write(client.Player.IsAlive);

                _server.SendMessage(send, client.ClientConnection, NetDeliveryMethod.ReliableOrdered);
                Program.Log.Info("Send 'End' to player #" + client.Player.Id);
            }
        }
    }
}

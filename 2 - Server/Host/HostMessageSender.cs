using System;
using System.Collections.Generic;
using System.IO;
using FBLibrary;
using FBLibrary.Network;
using FBServer.Core;
using FBServer.Core.Entities;
using Lidgren.Network;
using Lidgren.Network.Xna;
using Microsoft.Xna.Framework;

namespace FBServer.Host
{
    sealed partial class GameServer
    {
        #region Pre game

        private void SendCurrentMap(Client client)
        {
            if (client.ClientConnection.Status == NetConnectionStatus.Connected)
            {
                try
                {
                    NetOutgoingMessage message = _server.CreateMessage();
                    message.Write((byte)MessageType.ServerMessage.Map);

                    message.Write(Instance.SelectedMapName);
                    message.Write(MapLoader.MapFileDictionary[Instance.SelectedMapName]);

                    string path = "Content/Maps/" + Instance.SelectedMapName;

                    byte[] mapData = File.ReadAllBytes(path);

                    message.Write(mapData.Length);
                    foreach (var bt in mapData)
                    {
                        message.Write(bt);
                    }

                    _server.SendMessage(message, client.ClientConnection, NetDeliveryMethod.ReliableOrdered);
                    Program.Log.Info("[SEND] Sent the map to client #" + client.ClientId);
                }
                catch (FileNotFoundException ex)
                {
                    throw new Exception("This map doesn't exist !");
                }

            }
        }

        private void SendGameWillStart()
        {
            NetOutgoingMessage message = _server.CreateMessage();

            message.Write((byte)MessageType.ServerMessage.GameWillStart);

            _server.SendToAll(message, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendStartGame(bool gameInProgress)
        {
            NetOutgoingMessage message = _server.CreateMessage();

            message.Write((byte)MessageType.ServerMessage.StartGame);
            message.Write(gameInProgress);

            if (!gameInProgress)
            {
                List<Wall> walls = Instance.GameManager.WallList;
                message.Write(walls.Count);

                foreach (var wall in walls)
                {
                    message.Write(wall.CellPosition);
                }
            }

            _server.SendToAll(message, NetDeliveryMethod.ReliableOrdered);

            Program.Log.Info("[SEND] Sent start game to all clients");

            // Send players postions
            SendPlayersPosition(false);
        }

        public void SendAvailableMaps(Client client)
        {
            NetOutgoingMessage message = _server.CreateMessage();

            message.Write((byte)MessageType.ServerMessage.AvailableMaps);

            // Number of maps
            message.Write(MapLoader.MapFileDictionary.Count);

            foreach (var map in MapLoader.MapFileDictionary)
            {
                message.Write(map.Key);
                message.Write(map.Value);
            }

            _server.SendMessage(message, client.ClientConnection, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendSelectedMap(Client client)
        {
            try
            {
                NetOutgoingMessage message = _server.CreateMessage();

                message.Write((byte)MessageType.ServerMessage.SelectedMap);

                string currentMapMd5 = MapLoader.MapFileDictionary[Instance.SelectedMapName];

                message.Write(currentMapMd5);

                _server.SendToAll(message, NetDeliveryMethod.ReliableOrdered);
            }
            catch (Exception)
            {
                throw new Exception("Error sending the md5 hash of the selected map !");
            }
        }

        /// <summary>
        /// Sends the id generate by the server to the corresponding client
        /// </summary>
        /// <param name="client">The corresponding client</param>
        public void SendClientId(Client client)
        {
            NetOutgoingMessage message = _server.CreateMessage();

            message.Write((byte)MessageType.ServerMessage.ClientId);
            message.Write(client.ClientId);

            _server.SendMessage(message, client.ClientConnection, NetDeliveryMethod.ReliableOrdered);

            Program.Log.Info("[SEND] Sent server generated id to its corresponding client (id: " + client.ClientId + ")");
        }

        private void SendIsReady(Client client, bool ready)
        {
            NetOutgoingMessage message = _server.CreateMessage();

            message.Write((byte)MessageType.ServerMessage.IsReady);
            message.Write(client.ClientId);
            message.Write(ready);

            _server.SendToAll(message, client.ClientConnection, NetDeliveryMethod.ReliableUnordered, 0);

            //Program.Log.Info("[SEND] Sent that this client is ready or not (id: " + client.ClientId + "|ready: " + ready + ")");
        }

        // Send all players to this player
        public void SendClientsToNew(Client client)
        {
            foreach (Client currentClient in Clients)
            {
                if (client != currentClient)
                {
                    NetOutgoingMessage message = GetClientInfo(currentClient);

                    _server.SendMessage(message, client.ClientConnection, NetDeliveryMethod.ReliableOrdered);

                    Program.Log.Info("[SEND] Sent the player (client #" + currentClient.ClientId + ") to the new player (client #" + client.ClientId + ")");
                }
            }

            Program.Log.Info("[SEND] Sent the players to the new player (client #" + client.ClientId + ")");
        }

        // Send this new player to already existing players
        public void SendNewClientInfo(Client client)
        {
            if (client.ClientConnection.Status == NetConnectionStatus.Connected)
            {
                NetOutgoingMessage message = GetClientInfo(client);

                _server.SendToAll(message, NetDeliveryMethod.ReliableOrdered);

                Program.Log.Info("[SEND] Sent the new player to all other players (client #" + client.ClientId + ")");
            }
        }

        public void SendRemovePlayer(Player removedPlayer)
        {
            NetOutgoingMessage message = _server.CreateMessage();

            message.Write((byte)MessageType.ServerMessage.RemovePlayer);
            message.Write(removedPlayer.Id);

            _server.SendToAll(message, NetDeliveryMethod.ReliableOrdered);

            Program.Log.Info("[SEND] Sent that the player #" + removedPlayer.Id + " is dead !");
        }

        #region GetPlayerInfo
        private NetOutgoingMessage GetClientInfo(Client client)
        {
            NetOutgoingMessage message = _server.CreateMessage();

            message.Write((byte)MessageType.ServerMessage.NewClientInfo);

            message.Write(client.ClientId);
            message.Write(client.Username);
            message.Write(client.IsReady);

            return message;
        }
        #endregion

        #endregion

        #region Game actions

        // Send the player's movement to all other players
        public void SendPlayerPosition(Client client, bool notDir, bool exceptHim)
        {
            NetOutgoingMessage message = _server.CreateMessage();

            message.Write((byte)MessageType.ServerMessage.PlayerPosition);

            message.Write(client.Player.Position.X);
            message.Write(client.Player.Position.Y);

            message.Write((byte)client.Player.CurrentDirection);

            message.Write(client.ClientId);

            string logMessage = "[SEND] Sent position of client #" + client.ClientId + " !";
            if (exceptHim)
            {
                _server.SendToAll(message, client.ClientConnection, NetDeliveryMethod.ReliableOrdered, 0);
                logMessage += " (except him)";
            }
            else
            {
                _server.SendToAll(message, NetDeliveryMethod.ReliableOrdered);
            }

            Program.Log.Info(logMessage);

        }

        private void SendPlayersPosition(bool exceptHim)
        {
            foreach (var client in Instance.Clients)
            {
                SendPlayerPosition(client, false, exceptHim);
            }
        }

        // Send to all players that this player has placed a bomb
        public void SendPlayerPlacingBomb(Client client, Point position)
        {
            NetOutgoingMessage message = _server.CreateMessage();

            message.Write((byte)MessageType.ServerMessage.PlayerPlacingBomb);
            message.Write(client.ClientId);
            message.Write(position);

            _server.SendToAll(message, NetDeliveryMethod.ReliableOrdered);

            Program.Log.Info("[SEND][Client #" + client.ClientId + "] Planted a bomb !");
        }

        // Send to all players that a bomb has blow up
        public void SendBombExploded(Bomb bomb)
        {
            NetOutgoingMessage message = _server.CreateMessage();

            message.Write((byte)MessageType.ServerMessage.BombExploded);
            message.Write(bomb.CellPosition);

            _server.SendToAll(message, NetDeliveryMethod.ReliableOrdered);

            Program.Log.Info("[SEND] Sent that bomb exploded to everyone ! (position: " + bomb.Position + ")");
        }

        public void SendPowerUpDrop(PowerUp powerUp)
        {
            NetOutgoingMessage message = _server.CreateMessage();

            message.Write((byte)MessageType.ServerMessage.PowerUpDrop);

            message.Write((byte)powerUp.Type);
            message.Write(powerUp.CellPosition);

            _server.SendToAll(message, NetDeliveryMethod.ReliableOrdered);

            Program.Log.Info("[SEND] Power up dropped ! (type: " + powerUp.Type + "|position: " + powerUp.CellPosition + ")");
        }

        public void SendPowerUpPickUp(Client client, PowerUp powerUp)
        {
            NetOutgoingMessage message = _server.CreateMessage();

            message.Write((byte)MessageType.ServerMessage.PowerUpPickUp);

            message.Write(client.ClientId);
            message.Write(powerUp.CellPosition);
            message.Write((byte)powerUp.Type);

            _server.SendToAll(message, NetDeliveryMethod.ReliableOrdered);

            Program.Log.Info("[SENT] Power up pick up by client #" + client.ClientId + " ! (type: " + powerUp.Type + "|position: " + powerUp.CellPosition + ")");
        }

        public void SendSuddenDeath()
        {
            NetOutgoingMessage message = _server.CreateMessage();

            message.Write((byte)MessageType.ServerMessage.SuddenDeath);

            _server.SendToAll(message, NetDeliveryMethod.ReliableOrdered);

            Program.Log.Info("[SEND] SUDDEN DEATH!");
        }

        public void SendRoundEnd(Client client)
        {
            if (client.ClientConnection.Status == NetConnectionStatus.Connected)
            {
                NetOutgoingMessage message = _server.CreateMessage();

                message.Write((byte)MessageType.ServerMessage.RoundEnd);

                _server.SendMessage(message, client.ClientConnection, NetDeliveryMethod.ReliableOrdered);

                Program.Log.Info("[SEND] Sent 'RoundEnd' to player #" + client.Player.Id);
            }
        }

        #endregion

        #region Post game actions

        public void SendEnd(Client client)
        {
            if (client.ClientConnection.Status == NetConnectionStatus.Connected)
            {
                NetOutgoingMessage message = _server.CreateMessage();

                message.Write((byte)MessageType.ServerMessage.End);
                message.Write(client.Player.IsAlive);

                _server.SendMessage(message, client.ClientConnection, NetDeliveryMethod.ReliableOrdered);
                Program.Log.Info("[SEND] 'End' to player #" + client.Player.Id);
            }
        }

        #endregion

        public void SendPings()
        {
            NetOutgoingMessage message = _server.CreateMessage();

            message.Write((byte)MessageType.ServerMessage.Pings);
            message.Write(Clients.Count);
            foreach (Client client in Clients)
            {
                message.Write(client.ClientId);
                message.Write(client.Ping);
            }

            _server.SendToAll(message, NetDeliveryMethod.ReliableOrdered);

            //Program.Log.Info("[SEND] Pings of all players to all players !");
        }
    }
}

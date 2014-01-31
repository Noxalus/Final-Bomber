using System.Diagnostics;
using FBLibrary;
using FBLibrary.Core;
using FBClient.WorldEngine;
using Lidgren.Network;
using Lidgren.Network.Xna;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace FBClient.Network
{
    sealed partial class GameServer
    {
        #region Receive methods

        private void ReceiveMyClientId(int clientId)
        {
            Instance.Clients.CreateMe(clientId);
        }

        private void ReceiveAvailableMaps(NetIncomingMessage message)
        {
            int mapNumber = message.ReadInt32();

            for (int i = 0; i < mapNumber; i++)
            {
                Instance.Maps.Add(message.ReadString(), message.ReadString());
            }
        }

        private void ReceiveSelectedMap(string md5)
        {
            Instance.SelectedMapMd5 = md5;
        }

        private void ReceiveMap(NetIncomingMessage message)
        {
            string mapName = message.ReadString();
            string md5 = message.ReadString();

            int mapBytes = message.ReadInt32();

            var data = new List<byte>();
            for (int i = 0; i < mapBytes; i++)
            {
                data.Add(message.ReadByte());
            }

            string localMapName = MapLoader.NewMap(mapName, data);

            var map = new Map();
            map.Parse(localMapName, GameManager);

            if (map.GetMd5() != md5)
            {
                throw new Exception("Map sended by the server is not the same !");
            }

            MapLoader.LoadMapFiles();

            SendHasMap();
        }

        private void ReceiveGameWillStart()
        {
            // Check that we have the selected map else ask to download it ! :)
            if (!MapLoader.MapFileDictionary.ContainsValue(Instance.SelectedMapMd5))
            {
                SendNeedMap();
            }
            else
            {
                SendHasMap();
            }
        }

        private void ReceiveStartGame(NetIncomingMessage message)
        {
            bool gameInProgress = message.ReadBoolean();

            if (!gameInProgress)
            {
                var wallPositions = new List<Point>();
                int wallNumber = message.ReadInt32();

                for (int i = 0; i < wallNumber; i++)
                {
                    wallPositions.Add(message.ReadPoint());
                }

                OnStartGame(false, wallPositions);
            }
            else
            {
                OnStartGame(true, null);
            }
        }

        private void ReceiveNewClientInfo(int clientId, string username, bool isReady)
        {
            OnNewClient(clientId, username, isReady);
        }

        private void ReceiveRemovePlayer(int playerId)
        {
            OnRemovePlayer(playerId);
        }

        private void ReceiveIsReady(int clientId, bool ready)
        {
            var client = Instance.Clients.GetClientById(clientId);

            if (client != null)
            {
                client.IsReady = ready;
            }
            else
            {
                throw new Exception("This client doesn't exist. (id: " + clientId + ")");
            }
        }

        private void ReceivePosition(float positionX, float positionY, byte direction, int clientId)
        {
            var arg = new MovePlayerArgs
            {
                Position = { X = positionX, Y = positionY },
                Direction = direction,
                ClientId = clientId
            };

            OnMovePlayerAction(arg);
        }

        private void ReceivePlacingBomb(int playerId, Point position)
        {
            OnPlacingBomb(playerId, position);
        }

        private void ReceiveBombExploded(NetIncomingMessage message)
        {
            var position = message.ReadPoint();

            /*
            int count = buffer.ReadInt32();
            List<Explosion> explosions = new List<Explosion>();
            for (int i = 0; i < count; i++)
            {
                explosions.Add(new Explosion(new Vector2(buffer.ReadFloat(), buffer.ReadFloat()), (Explosion.ExplosionType)buffer.ReadByte(), true));
            }
            */

            //OnBombExploded(position);
        }

        private void ReceivePlayerKill(int victimId, int killerId)
        {
            OnPlayerKilled(victimId, killerId);
        }

        private void ReceivePlayerSuicide(int suicidalId)
        {
            OnPlayerSuicide(suicidalId);
        }

        private void ReceivePowerupDrop(PowerUpType type, Point position)
        {
            OnPowerUpDrop(type, position);
        }

        private void ReceivePowerUpPickUp(int playerId, Point position, PowerUpType type)
        {
            OnPowerUpPickUp(playerId, position, type);
        }

        /*
        public void ReceiveSuddenDeath()
        {
            OnSuddenDeath();
        }
        */

        private void ReceiveRoundEnd()
        {
            GameServer.Instance.GameManager.GameEventManager.OnRoundEnd();
        }

        private void ReceiveEnd(bool won)
        {
            OnEnd(won);
        }

        public void ReceivePing(float ping)
        {
            OnPing(ping);
        }

        private void ReceivePings(NetIncomingMessage message)
        {
            int clientNumber = message.ReadInt32();

            for (int i = 0; i < clientNumber; i++)
            {
                int clientId = message.ReadInt32();
                float ping = message.ReadFloat();

                var client = Instance.Clients.GetClientById(clientId);

                if (client != null)
                {
                    client.Ping = ping;
                }
                else
                {
                    Debug.Print("This client doesn't exist here ! (id: " + clientId + ")");
                }
            }
        }

        #endregion


        #region Events

        #region StartGame
        
        public delegate void StartGameEventHandler(bool gameInProgress, List<Point> wallPositions);
        public event StartGameEventHandler StartGame;

        private void OnStartGame(bool gameInProgress, List<Point> wallPositions)
        {
            if (StartGame != null)
                StartGame(gameInProgress, wallPositions);
        }

        #endregion

        #region NewPlayer
        public delegate void NewClientEventHandler(int clientId, string username, bool isReady);
        public event NewClientEventHandler NewClient;

        private void OnNewClient(int clientId, string username, bool isReady)
        {
            if (NewClient != null)
                NewClient(clientId, username, isReady);
        }
        #endregion

        #region RemovePlayer
        public delegate void RemovePlayerEventHandler(int playerId);
        public event RemovePlayerEventHandler RemovePlayer;

        private void OnRemovePlayer(int playerId)
        {
            if (RemovePlayer != null)
                RemovePlayer(playerId);
        }
        #endregion

        #region MovePlayerEvent
        public delegate void MovePlayerEventHandler(object sender, MovePlayerArgs e);
        public event MovePlayerEventHandler MovePlayer;

        private void OnMovePlayerAction(MovePlayerArgs e)
        {
            if (MovePlayer != null)
                MovePlayer(this, e);
        }
        #endregion

        #region PlacingBomb
        public delegate void PlacingBombEventHandler(int playerId, Point position);
        public event PlacingBombEventHandler PlacingBomb;

        private void OnPlacingBomb(int playerId, Point position)
        {
            if (PlacingBomb != null)
                PlacingBomb(playerId, position);
        }
        #endregion

        #region BombExploded
        public delegate void BombExplodedEventHandler(Point position);
        public event BombExplodedEventHandler BombExploded;

        private void OnBombExploded(Point position)
        {
            if (BombExploded != null)
                BombExploded(position);
        }
        #endregion

        #region PlayerKilled
        public delegate void PlayerKilledEventHandler(int victimId, int killerId);
        public event PlayerKilledEventHandler PlayerKilled;

        private void OnPlayerKilled(int victimId, int killerId)
        {
            if (PlayerKilled != null)
                PlayerKilled(victimId, killerId);
        }
        #endregion

        #region PlayerSuicide
        public delegate void PlayerSuicideEventHandler(int suicidalId);
        public event PlayerSuicideEventHandler PlayerSuicide;

        private void OnPlayerSuicide(int suicidalId)
        {
            if (PlayerSuicide != null)
                PlayerSuicide(suicidalId);
        }
        #endregion

        #region PowerUpDrop
        public delegate void PowerUpDropEventHandler(PowerUpType type, Point position);
        public event PowerUpDropEventHandler PowerUpDrop;

        private void OnPowerUpDrop(PowerUpType type, Point position)
        {
            if (PowerUpDrop != null)
                PowerUpDrop(type, position);
        }
        #endregion

        #region PowerUpPickUp
        public delegate void PowerUpPickUpEventHandler(int playerId, Point position, PowerUpType type);
        public event PowerUpPickUpEventHandler PowerUpPickUp;
        private void OnPowerUpPickUp(int playerId, Point position, PowerUpType type)
        {
            if (PowerUpPickUp != null)
                PowerUpPickUp(playerId, position, type);
        }
        #endregion

        /*
        #region SuddenDeath
        public delegate void SuddenDeathEventHandler();
        public event SuddenDeathEventHandler SuddenDeath;
        protected virtual void OnSuddenDeath()
        {
            if (SuddenDeath != null)
                SuddenDeath();
        }
        #endregion
        */

        #region End
        public delegate void EndEventHandler(bool Won);
        public event EndEventHandler End;

        private void OnEnd(bool Won)
        {
            if (End != null)
                End(Won);
        }
        #endregion

        #region Ping
        public delegate void UpdatePingEventHandler(float ping);
        public event UpdatePingEventHandler UpdatePing;

        private void OnPing(float ping)
        {
            if (UpdatePing != null)
                UpdatePing(ping);
        }
        #endregion

        #endregion
    }

    public class MovePlayerArgs
    {
        public Vector2 Position = new Vector2();
        public byte Direction = 0;
        public int ClientId = 0;
    }
}

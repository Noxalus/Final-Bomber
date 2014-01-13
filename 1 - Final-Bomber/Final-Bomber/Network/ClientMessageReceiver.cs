using FBLibrary;
using FBLibrary.Core;
using FBClient.WorldEngine;
using Lidgren.Network;
using Lidgren.Network.Xna;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FBClient.Network
{
    sealed partial class GameServer
    {
        #region Recieve methods

        public void RecieveGameInfo(string mapMd5)
        {
            // Check that you have the map
            if (MapLoader.MapFileDictionary.ContainsValue(mapMd5))
            {
                GameSettings.CurrentMapName = MapLoader.MapFileDictionary.FirstOrDefault(x => x.Value == mapMd5).Key;
                OnStartInfo();
            }
            else
            {
                SendNeedMap();
            }
        }

        public void RecieveMap(NetIncomingMessage message)
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

            RecieveGameInfo(md5);
        }
        private void RecieveStartGame(NetIncomingMessage message)
        {
            bool gameInProgress = message.ReadBoolean();
            if (!gameInProgress)
            {
                int playerId = message.ReadInt32();
                float moveSpeed = message.ReadFloat();
                int suddenDeathTime = message.ReadInt32();
                int wallNumber = message.ReadInt32();
                var wallPositions = new List<Point>();

                for (int i = 0; i < wallNumber; i++)
                {
                    wallPositions.Add(message.ReadPoint());
                }

                OnStartGame(false, playerId, moveSpeed, suddenDeathTime, wallPositions);
            }
            else
            {
                OnStartGame(true, 0, 0, 0, null);
            }
        }

        #region New client
        public void RecieveClientInfo(int id)
        {
            var client = new Client(id);

            GameServer.Instance.OnConnectedClient(client, EventArgs.Empty);
        }
        #endregion

        public void RecievePlayerInfo(int playerId, float moveSpeed, string username, int score)
        {
            OnNewPlayer(playerId, moveSpeed, username, score);
        }

        public void RecieveRemovePlayer(int playerId)
        {
            OnRemovePlayer(playerId);
        }

        private void RecievePositionAndSpeed(float positionX, float positionY, byte direction, int playerId)
        {
            var arg = new MovePlayerArgs
            {
                Position = { X = positionX, Y = positionY },
                Direction = direction,
                PlayerId = playerId
            };

            OnMovePlayerAction(arg);
        }
        public void RecievePlacingBomb(int playerId, Point position)
        {
            OnPlacingBomb(playerId, position);
        }
        public void RecieveBombExploded(NetIncomingMessage message)
        {
            Point position = message.ReadPoint();

            /*
            int count = buffer.ReadInt32();
            List<Explosion> explosions = new List<Explosion>();
            for (int i = 0; i < count; i++)
            {
                explosions.Add(new Explosion(new Vector2(buffer.ReadFloat(), buffer.ReadFloat()), (Explosion.ExplosionType)buffer.ReadByte(), true));
            }
            */

            OnBombExploded(position);
        }

        public void RecievePowerupDrop(PowerUpType type, Point position)
        {
            OnPowerUpDrop(type, position);
        }

        /*
        public void RecievePowerupPick(float xPos, float yPos, int playerId, float amount)
        {
            OnPowerupPick(xPos, yPos, playerId, amount);
        }
        */

        /*
        public void RecieveSuddenDeath()
        {
            OnSuddenDeath();
        }
        */

        private void RecieveRoundEnd()
        {
            GameServer.Instance.GameManager.GameEventManager.OnRoundEnd();
        }

        public void RecieveEnd(bool won)
        {
            OnEnd(won);
        }

        public void RecievePing(float ping)
        {
            OnPing(ping);
        }

        #endregion





        #region Events

        #region StartInfo
        public delegate void StartInfoEventHandler();
        public event StartInfoEventHandler StartInfo;

        private void OnStartInfo()
        {
            if (StartInfo != null)
                StartInfo();
        }
        #endregion

        #region StartGame
        public delegate void StartGameEventHandler(bool gameInProgress, int playerId, float moveSpeed, int suddenDeathTime, List<Point> wallPositions);
        public event StartGameEventHandler StartGame;

        private void OnStartGame(bool gameInProgress, int playerId, float moveSpeed, int suddenDeathTime, List<Point> wallPositions)
        {
            if (StartGame != null)
                StartGame(gameInProgress, playerId, moveSpeed, suddenDeathTime, wallPositions);
        }
        #endregion

        #region NewPlayer
        public delegate void NewPlayerEventHandler(int playerId, float moveSpeed, string username, int score);
        public event NewPlayerEventHandler NewPlayer;

        private void OnNewPlayer(int playerId, float moveSpeed, string username, int score)
        {
            if (NewPlayer != null)
                NewPlayer(playerId, moveSpeed, username, score);
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

        #region PowerUpDrop
        public delegate void PowerUpDropEventHandler(PowerUpType type, Point position);
        public event PowerUpDropEventHandler PowerUpDrop;

        private void OnPowerUpDrop(PowerUpType type, Point position)
        {
            if (PowerUpDrop != null)
                PowerUpDrop(type, position);
        }
        #endregion

        /*
        #region PowerupPick
        public delegate void PowerupPickEventHandler(float xPos, float yPos, int playerId, float amount);
        public event PowerupPickEventHandler PowerupPick;
        protected virtual void OnPowerupPick(float xPos, float yPos, int playerId, float amount)
        {
            if (PowerupPick != null)
                PowerupPick(xPos, yPos, playerId, amount);
        }
        #endregion
        */

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
        public int PlayerId = 0;
    }
}

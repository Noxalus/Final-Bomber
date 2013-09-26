using FBLibrary;
using FBLibrary.Core;
using Final_Bomber.Core;
using Final_Bomber.Core.Entities;
using Final_Bomber.Screens;
using Final_Bomber.Utils;
using Final_Bomber.WorldEngine;
using Lidgren.Network;
using Lidgren.Network.Xna;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Final_Bomber.Network
{
    partial class GameServer
    {
        #region StartInfo
        public delegate void StartInfoEventHandler();
        public event StartInfoEventHandler StartInfo;
        protected virtual void OnStartInfo()
        {
            if (StartInfo != null)
                StartInfo();
        }
        #endregion
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
            map.Parse(localMapName, NetworkTestScreen.GameManager);

            if (map.GetMd5() != md5)
            {
                throw new Exception("Map sended by the server is not the same !");
            }

            MapLoader.LoadMapFiles();

            RecieveGameInfo(md5);
        }

        #region 

        public delegate void StartGameEventHandler(bool gameInProgress, int playerId, float moveSpeed, int suddenDeathTime, List<Point> wallPositions);
        public event StartGameEventHandler StartGame;
        protected virtual void OnStartGame(bool gameInProgress, int playerId, float moveSpeed, int suddenDeathTime, List<Point> wallPositions)
        {
            if (StartGame != null)
                StartGame(gameInProgress, playerId, moveSpeed, suddenDeathTime, wallPositions);
        }
        #endregion

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

        #region NewPlayer
        public delegate void NewPlayerEventHandler(int playerID, float moveSpeed, string username);
        public event NewPlayerEventHandler NewPlayer;
        protected virtual void OnNewPlayer(int playerID, float moveSpeed, string username)
        {
            if (NewPlayer != null)
                NewPlayer(playerID, moveSpeed, username);
        }
        #endregion
        public void RecievePlayerInfo(int playerID, float moveSpeed, string username)
        {
            OnNewPlayer(playerID, moveSpeed, username);
        }

        #region RemovePlayer
        public delegate void RemovePlayerEventHandler(int playerID);
        public event RemovePlayerEventHandler RemovePlayer;
        protected virtual void OnRemovePlayer(int playerID)
        {
            if (RemovePlayer != null)
                RemovePlayer(playerID);
        }
        #endregion

        public void RecieveRemovePlayer(int playerID)
        {
            OnRemovePlayer(playerID);
        }

        #region MovePlayerEvent
        public delegate void MovePlayerEventHandler(object sender, MovePlayerArgs e);
        public event MovePlayerEventHandler MovePlayer;
        protected virtual void OnMovePlayerAction(MovePlayerArgs e)
        {
            if (MovePlayer != null)
                MovePlayer(this, e);
        }
        #endregion

        private void RecievePositionAndSpeed(float positionX, float positionY, byte action, int playerID)
        {
            var arg = new MovePlayerArgs
            {
                Position = { X = positionX, Y = positionY },
                Action = action,
                PlayerID =  playerID
            };

            OnMovePlayerAction(arg);
        }

        #region PlacingBomb
        public delegate void PlacingBombEventHandler(int playerId, Point position);
        public event PlacingBombEventHandler PlacingBomb;
        protected virtual void OnPlacingBomb(int playerId, Point position)
        {
            if (PlacingBomb != null)
                PlacingBomb(playerId, position);
        }
        #endregion
        public void RecievePlacingBomb(int playerId, Point position)
        {
            OnPlacingBomb(playerId, position);
        }

        #region BombExploded
        public delegate void BombExplodedEventHandler(Point position);
        public event BombExplodedEventHandler BombExploded;
        protected virtual void OnBombExploded(Point position)
        {
            if (BombExploded != null)
                BombExploded(position);
        }
        #endregion
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
        /*
        #region Burn
        public delegate void BurnEventHandler(int playerId);
        public event BurnEventHandler Burn;
        protected virtual void OnBurn(int playerId)
        {
            if (Burn != null)
                Burn(playerId);
        }
        #endregion
        public void RecieveBurn(int playerId)
        {
            OnBurn(playerId);
        }

        #region ExplodeTile
        public delegate void ExplodeTileEventHandler(int tilePos);
        public event ExplodeTileEventHandler ExplodeTile;
        protected virtual void OnExplodeTile(int tilePos)
        {
            if (ExplodeTile != null)
                ExplodeTile(tilePos);
        }
        #endregion
        public void RecieveExplodeTile(int tilePos)
        {
            OnExplodeTile(tilePos);
        }
        */

        #region PowerUpDrop
        public delegate void PowerUpDropEventHandler(PowerUpType type, Point position);
        public event PowerUpDropEventHandler PowerUpDrop;
        protected virtual void OnPowerUpDrop(PowerUpType type, Point position)
        {
            if (PowerUpDrop != null)
                PowerUpDrop(type, position);
        }
        #endregion
        public void RecievePowerupDrop(PowerUpType type, Point position)
        {
            OnPowerUpDrop(type, position);
        }

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
        public void RecievePowerupPick(float xPos, float yPos, int playerId, float amount)
        {
            OnPowerupPick(xPos, yPos, playerId, amount);
        }
        */

        #region SuddenDeath
        public delegate void SuddenDeathEventHandler();
        public event SuddenDeathEventHandler SuddenDeath;
        protected virtual void OnSuddenDeath()
        {
            if (SuddenDeath != null)
                SuddenDeath();
        }
        #endregion
        public void RecieveSuddenDeath()
        {
            OnSuddenDeath();
        }

        #region SDExplosion
        public delegate void SDExplosionEventHandler(int tilePos);
        public event SDExplosionEventHandler SDExplosion;
        protected virtual void OnSDExplosion(int tilePos)
        {
            if (SDExplosion != null)
                SDExplosion(tilePos);
        }
        #endregion
        public void RecieveSDExplosion(int tilePos)
        {
            OnSDExplosion(tilePos);
        }

        #region Round End
        public delegate void RoundEndEventHandler();
        public event RoundEndEventHandler RoundEnd;
        protected virtual void OnRoundEnd()
        {
            if (RoundEnd != null)
                RoundEnd();
        }
        #endregion

        public void RecieveRoundEnd()
        {
            OnRoundEnd();
        }

        #region End
        public delegate void EndEventHandler(bool Won);
        public event EndEventHandler End;
        protected virtual void OnEnd(bool Won)
        {
            if (End != null)
                End(Won);
        }
        #endregion

        public void RecieveEnd(bool Won)
        {
            OnEnd(Won);
        }


        #region Ping
        public delegate void UpdatePingEventHandler(float ping);
        public event UpdatePingEventHandler UpdatePing;
        protected virtual void OnPing(float ping)
        {
            if (UpdatePing != null)
                UpdatePing(ping);
        }
        #endregion

        public void RecievePing(float ping)
        {
            OnPing(ping);
        }
    }

    public class MovePlayerArgs
    {
        public Vector2 Position = new Vector2();
        public byte Action = 0;
        public int PlayerID = 0;
    }
}

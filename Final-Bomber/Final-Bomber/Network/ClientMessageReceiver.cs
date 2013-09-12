using Lidgren.Network;
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
        public void RecieveGameInfo(Int64 mapId)
        {
            GameSettings.currentMap = mapId;
            // Check that you have the path
            if (true /*GameSettings.Maps.GetMapById(GameSettings.currentMap) != null*/) 
            {
                OnStartInfo();
            }
            else
            {
                SendNeedMap();
            }
        }
        
        public void RecieveMap()
        {
            /*
            db_FileIO db = new db_FileIO();
            string mapName = buffer.ReadString();
            int mapBytes = buffer.ReadInt32();
            List<byte> data = new List<byte>();
            for (int i = 0; i < mapBytes; i++)
            {
                data.Add(buffer.ReadByte());
            }
            GameSettings.Maps.NewMap(mapName, data);
            #region debug
            if (GameSettings.Maps.GetMapById(GameSettings.currentMap) == null) //kollar så att man har banan
            {
                throw new Exception("SendMap funkade inte!");
            }
            #endregion
            RecieveGameInfo(GameSettings.currentMap);
            */
        }

        #region StartGame
        public delegate void StartGameEventHandler(bool gameInProgress, int playerId, float moveSpeed, int suddenDeathTime);
        public event StartGameEventHandler StartGame;
        protected virtual void OnStartGame(bool gameInProgress, int playerId, float moveSpeed, int suddenDeathTime)
        {
            if (StartGame != null)
                StartGame(gameInProgress, playerId, moveSpeed, suddenDeathTime);
        }
        #endregion
        public void RecieveStartGame(NetIncomingMessage message)
        {
            bool gameInProgress = message.ReadBoolean();
            if (!gameInProgress)
            {
                int playerId = message.ReadInt32();
                float moveSpeed = message.ReadFloat();
                int suddenDeathTime = message.ReadInt32();

                OnStartGame(gameInProgress, playerId, moveSpeed, suddenDeathTime);
            }
            else
            {
                OnStartGame(gameInProgress, 0, 0, 0);
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
            Console.WriteLine("Receive position from server !");

            var arg = new MovePlayerArgs
            {
                Position = { X = positionX, Y = positionY },
                Action = action,
                PlayerID =  playerID
            };

            OnMovePlayerAction(arg);
        }

        #region PlacingBomb
        public delegate void PlacingBombEventHandler(int playerId, float xPos, float yPos);
        public event PlacingBombEventHandler PlacingBomb;
        protected virtual void OnPlacingBomb(int playerId, float xPos, float yPos)
        {
            if (PlacingBomb != null)
                PlacingBomb(playerId, xPos, yPos);
        }
        #endregion
        public void RecievePlacingBomb(int playerId, float xPos, float yPos)
        {
            OnPlacingBomb(playerId, xPos, yPos);
        }

        /*
        #region BombExploded
        public delegate void BombExplodedEventHandler(float xPos, float yPos, List<Explosion> explosions);
        public event BombExplodedEventHandler BombExploded;
        protected virtual void OnBombExploded(float xPos, float yPos, List<Explosion> explosions)
        {
            if (BombExploded != null)
                BombExploded(xPos, yPos, explosions);
        }
        #endregion
        public void RecieveBombExploded()
        {
            float xPos = buffer.ReadFloat();
            float yPos = buffer.ReadFloat();
            int count = buffer.ReadInt32();
            List<Explosion> explosions = new List<Explosion>();
            for (int i = 0; i < count; i++)
            {
                explosions.Add(new Explosion(new Vector2(buffer.ReadFloat(), buffer.ReadFloat()), (Explosion.ExplosionType)buffer.ReadByte(), true));
            }
            OnBombExploded(xPos, yPos, explosions);
        }

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

        #region PowerupDrop
        public delegate void PowerupDropEventHandler(Powerup.PowerupType type, float xPos, float yPos);
        public event PowerupDropEventHandler PowerupDrop;
        protected virtual void OnPowerupDrop(Powerup.PowerupType type, float xPos, float yPos)
        {
            if (PowerupDrop != null)
                PowerupDrop(type, xPos, yPos);
        }
        #endregion
        
        public void RecievePowerupDrop(Powerup.PowerupType type, float xPos, float yPos)
        {
            OnPowerupDrop(type, xPos, yPos);
        }

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

        #region EndTile
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
    }

    public class MovePlayerArgs
    {
        public Vector2 Position = new Vector2();
        public byte Action = 0;
        public int PlayerID = 0;
    }
}

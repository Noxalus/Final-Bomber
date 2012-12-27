using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Final_Bomber.Net
{
    public partial class GameServer
    {
        // Login
        public class PlayerStatsEventArgs
        {
            public int Kills, ExplodeHits, Burns, SelfExplodeHits, SelfKills, TilesBlowned, PowerupsPicked, TileWalkDistance, Wins, Defeats;
        }

        #region LoggedIn
        public delegate void LoggedInEventHandler(byte status);
        public event LoggedInEventHandler LoggedIn;
        protected virtual void OnLoggedIn(byte status)
        {
            if (LoggedIn != null)
                LoggedIn(status);
        }
        #endregion

        public void RecieveLoggedIn(byte status)
        {
            OnLoggedIn(status);
        }

        public string RecieveCreatedAccount(int status)
        {
            switch (status)
            {
                case 0:
                    return("Account created successfully!");
                case 1:
                    return ("Username already exist, please try again");
                default:
                    return ("Account creation error (problem with switch case)");
            }
        }

        #region HostedGames
        public delegate void HostedGamesEventHandler(string ip, string name, string map, float ping, int maxplayers, int players);
        public event HostedGamesEventHandler HostedGames;
        protected virtual void OnHostedGames(string ip, string name, string map, float ping, int maxplayers, int players)
        {
            if (HostedGames != null)
                HostedGames(ip, name, map, ping, maxplayers, players);
        }
        #endregion
        public void RecieveHostedGames(string game)
        {
            if (game != "END")
            {
                OnHostedGames(game, msgIn.ReadString(), msgIn.ReadString(), msgIn.ReadFloat(), msgIn.ReadInt32(), msgIn.ReadInt32());
            }
        }

        public bool RecieveCurrentVersion(string version)
        {
            /*
            string VERSION = Program.VERSION;
            if (version != VERSION)
            {
                //("A new version of Final-Bomber has been released, please download the latest patch:\nYour Patch Version: 
                // " + VERSION + "\nNew Patch Version: " + version);
                this.EndMainConnection("bye");
            }
            */
            return false;
        }

        #region Stats
        public delegate void StatsEventHandler(PlayerStatsEventArgs args);
        public event StatsEventHandler Stats;
        protected virtual void OnStats(PlayerStatsEventArgs args)
        {
            if (Stats != null)
                Stats(args);
        }
        #endregion
        public void RecieveStats()
        {
            PlayerStatsEventArgs args = new PlayerStatsEventArgs();
            args.Burns = msgIn.ReadInt32();
            args.Defeats = msgIn.ReadInt32();
            args.ExplodeHits = msgIn.ReadInt32();
            args.Kills = msgIn.ReadInt32();
            args.PowerupsPicked = msgIn.ReadInt32();
            args.SelfExplodeHits = msgIn.ReadInt32();
            args.SelfKills = msgIn.ReadInt32();
            args.TilesBlowned = msgIn.ReadInt32();
            args.TileWalkDistance = msgIn.ReadInt32();
            args.Wins = msgIn.ReadInt32();
            OnStats(args);
        }

        #region Ranking
        public delegate void RankingEventHandler(int ranknr, string username, int elo, bool clearRank);
        public event RankingEventHandler Ranking;
        protected virtual void OnRanking(int ranknr, string username, int elo, bool clearRank)
        {
            if (Ranking != null)
                Ranking(ranknr, username, elo, clearRank);
        }
        #endregion
        public void RecieveRanking()
        {
            OnRanking(0, "", 0, true);
            int ranknr = 1;
            int count = msgIn.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                OnRanking(ranknr, msgIn.ReadString(), msgIn.ReadInt32(), false);
                ranknr++;
            }
        }

        // Game
        #region StartInfo
        public delegate void StartInfoEventHandler();
        public event StartInfoEventHandler StartInfo;
        protected virtual void OnStartInfo()
        {
            if (StartInfo != null)
                StartInfo();
        }
        #endregion

        #region StartGame
        public delegate void StartGameEventHandler(bool gameInProgress, int playerId, float moveSpeed, int suddenDeathTime);
        public event StartGameEventHandler StartGame;
        protected virtual void OnStartGame(bool gameInProgress, int playerId, float moveSpeed, int suddenDeathTime)
        {
            if (StartGame != null)
                StartGame(gameInProgress, playerId, moveSpeed, suddenDeathTime);
        }
        #endregion

        public void RecieveStartGame(bool gameInProgress)
        {
            if (!gameInProgress)
            {
                OnStartGame(gameInProgress, msgIn.ReadInt32(), msgIn.ReadFloat(), msgIn.ReadInt32());
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

        public void RecievePositionAndSpeed(float posx, float posy, byte action, int playerID)
        {
            MovePlayerArgs arg = new MovePlayerArgs();
            arg.pos.X = posx;
            arg.pos.Y = posy;
            arg.action = action;
            arg.playerID = playerID;
            OnMovePlayerAction(arg);
        }

        #region PlantingBomb
        public delegate void PlantingBombEventHandler(int playerId, float xPos, float yPos);
        public event PlantingBombEventHandler PlantingBomb;
        protected virtual void OnPlantingBomb(int playerId, float xPos, float yPos)
        {
            if (PlantingBomb != null)
                PlantingBomb(playerId, xPos, yPos);
        }
        #endregion

        public void RecievePlacingBomb(int playerId, float xPos, float yPos)
        {
            OnPlantingBomb(playerId, xPos, yPos);
        }
    }

    public class MovePlayerArgs
    {
        public Vector2 pos = new Vector2();
        public byte action = 0;
        public int playerID = 0;
    }
}

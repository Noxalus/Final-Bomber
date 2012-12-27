using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Final_Bomber.Net
{
    public partial class GameServer
    {
        public enum RMT
        {
            // Login
            CreatedAccount = 0,
            LoggedIn = 1,
            SendHostedGames = 2,
            CurrentVersion = 3,
            SendStats = 4,
            SendRanking = 5,

            // Game
            GameStartInfo = 6,
            Map = 7,
            StartGame = 8,
            PlayerInfo = 9,
            PlayerPosAndSpeed = 10,
            RemovePlayer = 11,
            PlayerPlantingBomb = 12,
            PowerupPick = 13,
            SuddenDeath = 14,
            End = 15,
        }

        public enum SMT
        {
            // Login
            CreateAccount = 1,
            LogIn = 2,
            GetHostedGames = 3,
            GetRanking = 9,
            GetStats = 10,

            // Game
            NeedMap = 6,
            Ready = 7,
            MoveLeft = 0,
            MoveUp = 1,
            MoveRight = 2,
            MoveDown = 3,
            Standing = 4,
            PlaceBomb = 5,
        }
    }
}

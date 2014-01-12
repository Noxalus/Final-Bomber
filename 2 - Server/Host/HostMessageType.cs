using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBServer.Host
{
    partial class GameServer
    {
        public enum RMT
        {
            NeedMap = 6, // Player need to download the map
            Ready = 7, // Player is ready to start
            MoveLeft = 0, // Player wants to move to the left
            MoveUp = 1, // Player wants to move up
            MoveRight = 2, // Player wants to move to the right
            MoveDown = 3, // Player wants to move down
            Standing = 4, // Player wants to stay in place
            PlaceBomb = 5, // Player wants to plant a bomb
        }

        public enum SMT
        {
            GameStartInfo = 0, // Sends the map MD5 value
            Map = 1, // Sends map data (if client didn't already have it)
            StartGame = 9, // Tells to client that the game will start (also sends generated wall positions) 
            PlayerInfo = 3, // Sends player's information to other players (used for example when adding new players)
            PlayerPosition = 2, // Sends the player position
            RemovePlayer = 4, // Sends that a disconnected player has been removed
            PlayerPlacingBomb = 5, // Says that a player has planted a bomb
            BombExploded = 6, // Sends that a bomb has exploded
            PowerUpDrop = 11, // Sends that a power up has dropped
            PowerUpPick = 12, // Sens that a player has picked up a power up
            SuddenDeath = 13, // Tells to players that the sudden death began
            End = 10, // Sends that the game is finished (with the winner)
            RoundEnd = 15, // Sends that the round is finished
            ClientInfo = 16
        }
    }
}

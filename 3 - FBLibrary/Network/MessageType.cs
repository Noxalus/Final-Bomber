
namespace FBLibrary.Network
{
    public static class MessageType
    {
        public enum ClientMessage
        {
            MoveLeft = 0, // Player wants to move to the left
            MoveUp = 1, // Player wants to move up
            MoveRight = 2, // Player wants to move to the right
            MoveDown = 3, // Player wants to move down
            Standing = 4, // Player wants to stay in place
            PlaceBomb = 5, // Player wants to plant a bomb
            NeedMap = 6, // Player need to download the map
            Ready = 7, // Player is ready to start ?
            Credentials = 8, // Client credentials info (username, password, etc...)
            WantToStartGame = 9, // Host want to start game
            MapSelection = 10, // Host has selected a map to play
            HasMap = 11, // Player has already the selected map, no need to download it
        }

        public enum ServerMessage
        {
            GameStartInfo = 0, // Sends the map MD5 value
            Map = 1, // Sends map data (if client didn't already have it)
            PlayerPosition = 2, // Sends the player position
            NewClientInfo = 3, // Sends new client's information to other clients
            RemovePlayer = 4, // Sends that a disconnected player has been removed
            PlayerPlacingBomb = 5, // Says that a player has planted a bomb
            BombExploded = 6, // Sends that a bomb has exploded
            StartGame = 9, // Tells to client that the game will start (also sends generated wall positions) 
            End = 10, // Sends that the game is finished (with the winner)
            PowerUpDrop = 11, // Sends that a power up has dropped
            PowerUpPickUp = 12, // Sens that a player has picked up a power up
            SuddenDeath = 13, // Tells to players that the sudden death began
            RoundEnd = 15, // Sends that the round is finished
            ClientId = 16, // Sends the server generated id to its corresponding client
            IsReady = 17, // Sends that a specific client is ready or not
            AvailableMaps = 18, // Sends the list of available maps on the server
            SelectedMap = 19, // Sends the md5 of the map selected by the host
            Pings = 20, // Sends players pings to all players
            GameWillStart = 21, // Sends to all players that the game is going to start
        }
    }
}

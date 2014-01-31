
namespace FBLibrary.Network
{
    public static class MessageType
    {
        public enum ClientMessage
        {
            MoveLeft, // Player wants to move to the left
            MoveUp, // Player wants to move up
            MoveRight, // Player wants to move to the right
            MoveDown, // Player wants to move down
            Standing, // Player wants to stay in place
            PlaceBomb, // Player wants to plant a bomb
            NeedMap, // Player need to download the map
            Ready, // Player is ready to start ?
            Credentials, // Client credentials info (username, password, etc...)
            WantToStartGame, // Host want to start game
            MapSelection, // Host has selected a map to play
            HasMap, // Player has already the selected map, no need to download it
        }

        public enum ServerMessage
        {
            GameStartInfo, // Sends the map MD5 value
            Map, // Sends map data (if client didn't already have it)
            PlayerPosition, // Sends the player position
            NewClientInfo, // Sends new client's information to other clients
            RemovePlayer, // Sends that a disconnected player has been removed
            PlayerPlacingBomb, // Says that a player has planted a bomb
            BombExploded, // Sends that a bomb has exploded
            PlayerKill, // Sends that a player has been killed
            PlayerSuicide, // Sends that a player committed a suicide
            StartGame, // Tells to client that the game will start (also sends generated wall positions) 
            End, // Sends that the game is finished (with the winner)
            PowerUpDrop, // Sends that a power up has dropped
            PowerUpPickUp, // Sens that a player has picked up a power up
            SuddenDeath, // Tells to players that the sudden death began
            RoundEnd, // Sends that the round is finished
            ClientId, // Sends the server generated id to its corresponding client
            IsReady, // Sends that a specific client is ready or not
            AvailableMaps, // Sends the list of available maps on the server
            SelectedMap, // Sends the md5 of the map selected by the host
            Pings, // Sends players pings to all players
            GameWillStart, // Sends to all players that the game is going to start
        }
    }
}

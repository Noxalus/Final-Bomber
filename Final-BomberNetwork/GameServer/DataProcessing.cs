namespace Final_BomberNetwork.GameServer
{
    public partial class GameServer
    {
        public void DataProcessing(byte data)
        {
            switch (data)
            {
                // Login
                case (byte)RMT.LoggedIn:
                    RecieveLoggedIn(msgIn.ReadByte());
                    break;
                case (byte)RMT.CreatedAccount:
                    RecieveCreatedAccount(msgIn.ReadInt32());
                    break;
                case (byte)RMT.SendHostedGames:
                    RecieveHostedGames(msgIn.ReadString());
                    break;
                case (byte)RMT.CurrentVersion:
                    RecieveCurrentVersion(msgIn.ReadString());
                    break;
                case (byte)RMT.SendStats:
                    RecieveStats();
                    break;
                case (byte)RMT.SendRanking:
                    RecieveRanking();
                    break;

                // Game
                case (byte)RMT.StartGame:
                    RecieveStartGame(msgIn.ReadBoolean());
                    break;
                case (byte)RMT.PlayerPosAndSpeed:
                    RecievePositionAndSpeed(msgIn.ReadFloat(), msgIn.ReadFloat(), msgIn.ReadByte(), msgIn.ReadInt32());
                    break;
                case (byte)RMT.PlayerInfo:
                    RecievePlayerInfo(msgIn.ReadInt32(), msgIn.ReadFloat(), msgIn.ReadString());
                    break;
                case (byte)RMT.RemovePlayer:
                    RecieveRemovePlayer(msgIn.ReadInt32());
                    break;
                case (byte)RMT.PlayerPlantingBomb:
                    RecievePlacingBomb(msgIn.ReadInt32(), msgIn.ReadFloat(), msgIn.ReadFloat());
                    break;
            }
        }
    }
}

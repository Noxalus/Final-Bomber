namespace Final_BomberNetwork.GameServer
{
    partial class Server
    {
        private void DataProcessing(byte data, ref Client client)
        {
            switch (data)
            {
                case (byte)RMTClient.CreateAccount:
                    RecieveCreateAccount(client, msg.ReadString(), msg.ReadString());
                    break;

                case (byte)RMTClient.LogIn:
                    RecieveLogin(client, msg.ReadString(), msg.ReadString());
                    break;

                case (byte)RMTServer.IsHosting:
                    RecieveHosting(ref client, msg.ReadString(), msg.ReadString(), msg.ReadInt32());
                    break;

                case (byte)RMTClient.GetHostedGames:
                    RecieveGetHostedGames(client);
                    break;

                case (byte)RMTServer.GetCurrentPlayerAmount:
                    RecieveGetCurrentPlayerAmount(client, msg.ReadInt32());
                    break;

                case (byte)RMTServer.GetServerPort:
                    RecieveGetServerPort(client, msg.ReadInt32());
                    break;

                case (byte)RMTServer.GetCheckIfOnline:
                    RecieveCheckIfOnline(client, msg.ReadString(), msg.ReadString());
                    break;

                case (byte)RMTServer.NextMap:
                    RecieveNextMap(client, msg.ReadString());
                    break;

                case (byte)RMTServer.SendStats:
                    RecievePlayerStats();
                    break;

                case (byte)RMTClient.GetStats:
                    SendStats(client);
                    break;

                case (byte)RMTClient.GetRanking:
                    SendRanking(client);
                    break;
            }
        }
    }
}

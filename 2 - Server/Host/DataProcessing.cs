using FBLibrary.Core;
using FBLibrary.Network;
using Lidgren.Network;

namespace FBServer.Host
{
    sealed partial class GameServer
    {
        void DataProcessing(NetIncomingMessage message, ref Client client)
        {
            switch (message.ReadByte())
            {
                case (byte)MessageType.ClientMessage.NeedMap:
                    Program.Log.Info("[Client #" + client.ClientId + "] Need map !");
                    ReceiveNeedMap(client);
                    break;
                case (byte)MessageType.ClientMessage.Credentials:
                    Program.Log.Info("[Client #" + client.ClientId + "] Sends its credientials info !");
                    ReceiveCredentials(client, message.ReadString(), message.ReadString());
                    break;
                case (byte)MessageType.ClientMessage.Ready:
                    Program.Log.Info("[Client #" + client.ClientId + "] Sends ready message !");
                    ReceiveReady(client, message.ReadBoolean());
                    break;
                case (byte)MessageType.ClientMessage.MoveDown:
                    Program.Log.Info("[Client #" + client.ClientId + "] Want to move down !");
                    ReceiveMovePlayer(client, LookDirection.Down);
                    break;
                case (byte)MessageType.ClientMessage.MoveLeft:
                    Program.Log.Info("[Client #" + client.ClientId + "] Want to move left !");
                    ReceiveMovePlayer(client, LookDirection.Left);
                    break;
                case (byte)MessageType.ClientMessage.MoveRight:
                    Program.Log.Info("[Client #" + client.ClientId + "] Want to move right !");
                    ReceiveMovePlayer(client, LookDirection.Right);
                    break;
                case (byte)MessageType.ClientMessage.MoveUp:
                    Program.Log.Info("[Client #" + client.ClientId + "] Want to move up !");
                    ReceiveMovePlayer(client, LookDirection.Up);
                    break;
                case (byte)MessageType.ClientMessage.Standing:
                    Program.Log.Info("[Client #" + client.ClientId + "] Want to stay here !");
                    ReceiveMovePlayer(client, LookDirection.Idle);
                    break;
                case (byte)MessageType.ClientMessage.PlaceBomb:
                    Program.Log.Info("[Client #" + client.ClientId + "] Want to place bomb !");
                    ReceiveBombPlacing(client);
                    break;
            }
        }
    }
}

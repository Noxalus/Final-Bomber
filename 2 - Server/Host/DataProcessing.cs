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
                    Program.Log.Info("[Client #" + client.ClientId + "] Need map ! Let's give it to him :)");
                    ReceiveNeedMap(client);
                    break;
                case (byte)MessageType.ClientMessage.MapSelection:
                    Program.Log.Info("[Client #" + client.ClientId + "] Has selected a map to play ! :)");
                    ReceiveMapSelection(client, message.ReadString());
                    break;
                case (byte)MessageType.ClientMessage.Credentials:
                    Program.Log.Info("[Client #" + client.ClientId + "] Sent its credientials info !");
                    ReceiveCredentials(client, message.ReadString(), message.ReadString());
                    break;
                case (byte)MessageType.ClientMessage.Ready:
                    Program.Log.Info("[Client #" + client.ClientId + "] Sent Ready message !");
                    ReceiveReady(client, message.ReadBoolean());
                    break;
                case (byte)MessageType.ClientMessage.WantToStartGame:
                    Program.Log.Info("[Client #" + client.ClientId + "] Sent WantToStartGame message !");
                    ReceiveWantToStartGame();
                    break;
                case (byte)MessageType.ClientMessage.HasMap:
                    Program.Log.Info("[Client #" + client.ClientId + "] Sent HasMap message !");
                    ReceiveHasMap(client);
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

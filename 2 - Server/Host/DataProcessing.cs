using FBLibrary.Core;
using FBLibrary.Network;
using Lidgren.Network;

namespace FBServer.Host
{
    sealed partial class GameServer
    {
        // TODO: Merge opcodes for move direction, something like that: Move(LookDirection)
        void DataProcessing(NetIncomingMessage message, ref Client client)
        {
            switch (message.ReadByte())
            {
                case (byte)MessageType.ClientMessage.NeedMap:
                    Program.Log.Info("[RECEIVE][Client #" + client.ClientId + "] Need map ! Let's give it to him :)");
                    ReceiveNeedMap(client);
                    break;
                case (byte)MessageType.ClientMessage.MapSelection:
                    Program.Log.Info("[RECEIVE][Client #" + client.ClientId + "] Has selected a map to play ! :)");
                    ReceiveMapSelection(client, message.ReadString());
                    break;
                case (byte)MessageType.ClientMessage.Credentials:
                    ReceiveCredentials(client, message.ReadString(), message.ReadString());
                    break;
                case (byte)MessageType.ClientMessage.Ready:
                    Program.Log.Info("[RECEIVE][Client #" + client.ClientId + "] Ready message !");
                    ReceiveReady(client, message.ReadBoolean());
                    break;
                case (byte)MessageType.ClientMessage.WantToStartGame:
                    Program.Log.Info("[RECEIVE][Client #" + client.ClientId + "] WantToStartGame message !");
                    ReceiveWantToStartGame();
                    break;
                case (byte)MessageType.ClientMessage.HasMap:
                    Program.Log.Info("[RECEIVE][Client #" + client.ClientId + "] HasMap message !");
                    ReceiveHasMap(client);
                    break;
                case (byte)MessageType.ClientMessage.MoveDown:
                    Program.Log.Info("[RECEIVE][Client #" + client.ClientId + "] Want to move down !");
                    ReceiveMovePlayer(client, LookDirection.Down);
                    break;
                case (byte)MessageType.ClientMessage.MoveLeft:
                    Program.Log.Info("[RECEIVE][Client #" + client.ClientId + "] Want to move left !");
                    ReceiveMovePlayer(client, LookDirection.Left);
                    break;
                case (byte)MessageType.ClientMessage.MoveRight:
                    Program.Log.Info("[RECEIVE][Client #" + client.ClientId + "] Want to move right !");
                    ReceiveMovePlayer(client, LookDirection.Right);
                    break;
                case (byte)MessageType.ClientMessage.MoveUp:
                    Program.Log.Info("[RECEIVE][Client #" + client.ClientId + "] Want to move up !");
                    ReceiveMovePlayer(client, LookDirection.Up);
                    break;
                case (byte)MessageType.ClientMessage.Standing:
                    Program.Log.Info("[RECEIVE][Client #" + client.ClientId + "] Want to stay here !");
                    ReceiveMovePlayer(client, LookDirection.Idle, message.ReadTime(client.ClientConnection, true));
                    break;
                case (byte)MessageType.ClientMessage.PlaceBomb:
                    Program.Log.Info("[RECEIVE][Client #" + client.ClientId + "] Want to place bomb !");
                    ReceiveBombPlacing(client);
                    break;
            }
        }
    }
}
